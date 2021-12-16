using System;
using System.Linq;
using VodadModel.Repository;
using System.Net.Mail;
using System.Net;
using System.Transactions;
using System.Web;

namespace VodadModel.Helpers
{
    public class UserHelper
    {
        private static VodadEntities entities = new VodadEntities();
        public static NLog.Logger Logger = NLogWrapper.Logger;

        public static User GetUserById(int? id)
        {
            Repository<User> repo = new Repository<User>(entities);

            var user = repo.GetSingle(w => w.Id == id);

            return user;
        }

        public static User GetUserByEmail(string email)
        {
            Repository<User> repo = new Repository<User>(entities);

            var user = repo.GetSingle(w => w.Email == email);

            return user;
        }

        public static string GetUserNameByEmail(string email)
        {
            Repository<User> repo = new Repository<User>(entities);

            var result = repo.GetSingle(u => u.Email == email);

            if (result != null)
                if (result.Name != null)
                    return result.Name;
                else
                    return result.Email;
            else
                return "";
        }

        public static bool IsUsersOrderPerformed(int? opid, int userId)
        {
            var orderPerformedRepository = new Repository<OrderPerformed>(entities);

            var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == opid);

            return orderPerformed.PerformerPlatform.User.Id == userId || orderPerformed.OrderContent.Order.User.Id == userId;
        }

        public static void SendMessage(string MailTo, string NameTo, string MessageBody, string MessageSubject)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            //SmtpClient client = new SmtpClient("173.248.175.154", 25);
            client.EnableSsl = true;
            MailAddress from = new MailAddress(Utilities.Constants.AdminConstants.Email, "Vodad");
            MailAddress to = new MailAddress(MailTo, NameTo);
            MailMessage message = new MailMessage(from, to);
            message.Body = MessageBody;
            message.Subject = MessageSubject;
            NetworkCredential myCreds = new NetworkCredential(Utilities.Constants.AdminConstants.Email, "san557641", "");
            client.Credentials = myCreds;
            try
            {
                client.Send(message);
            }
            catch (Exception)
            {
                Console.WriteLine("Exception is:");
            }
            Console.WriteLine("Goodbye.");
        }

        public static bool BanUser(int userIdToBan, bool canBeUnbanned, string reason, string adminEmail)
        {
            Repository<Ban> banRepository = new Repository<Ban>(entities);
            Repository<User> userRepository = new Repository<User>(entities);
            Repository<Roles> rolesRepository = new Repository<Roles>(entities);

            var user = userRepository.GetSingle(w => w.Id == userIdToBan);

            if (user != null && !user.Email.Equals(Utilities.Constants.AdminConstants.Email))
            {
                using (var transaction = new TransactionScope())
                {
                    Ban ban = new Ban();

                    ban.UserId = userIdToBan;
                    ban.RoleId = user.RoleId;
                    ban.BanDateTime = DateTime.Now;
                    ban.BanReason = HttpUtility.HtmlEncode(reason);
                    ban.CanBeUnbanned = !canBeUnbanned;

                    //Закрыть все кампании и ордера для Advertiser'а, PerformerPlatform'ы для Performer'а и OrderPerformed, связанные с пользователем

                    if (user.Roles.RoleName.Equals(Utilities.Constants.UserRoles.Performer))
                    {
                        var orderPerformed = user.OrderPerformed.ToList();

                        foreach (var op in orderPerformed)
                            OrderHelper.ChangeOrderPerformedStatus((int)op.Id,
                                                                                Utilities.Constants.OrdersStatuses.
                                                                                    Pay, (int)op.User.Id);
                    }

                    var orders = user.Order.ToList();
                    var performerPlatform = user.PerformerPlatform.ToList();

                    foreach (var o in orders)
                        OrderHelper.ChangeOrderStatus(user.Email, (int)o.Id,
                                                                Utilities.Constants.OrdersStatuses.Closed);

                    foreach (var pp in performerPlatform)
                        PerformerPlatformHelper.ChangePerformerPlatformStatus((int) pp.Id,
                                                                             Utilities.Constants.
                                                                                 OrdersStatuses.Deleted, user.Email);


                    user.RoleId = rolesRepository.GetSingle(w => w.RoleName.Equals(Utilities.Constants.UserRoles.Banned)).Id;

                    var admin = GetUserByEmail(adminEmail);

                    ban.AdminBanId = admin.Id;

                    userRepository.Save();

                    banRepository.Add(ban);
                    banRepository.Save();

                    Logger.Info("User {0} {1} has been banned by {2} {3}", user.Id, user.Email, admin.Id, admin.Email);

                    transaction.Complete();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
