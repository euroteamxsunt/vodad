using System.Linq;
using VodadModel.Repository;

namespace VodadModel.Helpers
{
    public static class PerformerPlatformHelper
    {
        static VodadEntities Entities = new VodadEntities();
        public static NLog.Logger Logger = NLogWrapper.Logger;

        public static bool ChangePerformerPlatformStatus(int ppid, string status, string userEmail)
        {
            if (ppid != null)
            {
                if (status.Equals(Utilities.Constants.OrdersStatuses.Deleted))
                {
                    int userId = (int)UserHelper.GetUserByEmail(userEmail).Id;

                    if (IsUsersPerformerPlatform(ppid, userId))
                    {
                        var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);
                        var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

                        var performerPlatform = performerPlatformRepository.GetSingle(w => w.Id == ppid);

                        if (performerPlatform != null)
                        {
                            if (performerPlatform.OrderPerformed.Any(w => w.Status.Equals(Utilities.Constants.VerificationStatuses.Inaction)))
                            {
                                var inactionOrderPerformed = orderPerformedRepository.GetAll(w => w.PerformerPlatformId == ppid && w.Status.Equals(Utilities.Constants.VerificationStatuses.Inaction));

                                foreach (var op in inactionOrderPerformed)
                                {
                                    OrderHelper.ChangeOrderPerformedStatus((int)op.Id, Utilities.Constants.OrdersStatuses.Pay, (int)op.PerformerPlatform.User.Id);
                                }
                            }
                            else
                            {
                                DeletePerformerPlatform(ppid, userId);
                                Logger.Info(string.Format("PerformerPlatform id = {0} has been deleted", ppid));
                            }
                        }
                        else
                        {
                            DeletePerformerPlatform(ppid, userId);
                            Logger.Info(string.Format("PerformerPlatform id = {0} has been deleted", ppid));
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsUsersPerformerPlatform(int? ppid, int userId)
        {
            var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);
            var userRepository = new Repository<User>(Entities);

            var performerPlatform = performerPlatformRepository.GetSingle(w => w.Id == ppid);
            var user = userRepository.GetSingle(w => w.Id == userId);

            if (performerPlatform.PerformerId == user.Id)
                return true;
            else
                return false;
        }

        private static void DeletePerformerPlatform(int ppid, int userId)
        {
            if (IsUsersPerformerPlatform(ppid, userId))
            {
                var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

                var performerPlatform = performerPlatformRepository.GetSingle(w => w.Id == ppid);

                performerPlatformRepository.Delete(performerPlatform);
                performerPlatformRepository.Save();
                Logger.Info(string.Format("PerformerPlatform id = {0} has been deleted", performerPlatform.Id));
            }
        }
    }
}
