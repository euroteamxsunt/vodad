using VodadModel;
using VodadModel.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Vodad.Models;

namespace Vodad.Controllers
{
    public class StatsForAdminController : BaseController
    {
        //
        // GET: /StatsForAdmin/

        public ViewResult ActivatedUsersList()
        {
            Repository<User> allUsersRepository = new Repository<User>(Entities);

            ViewBag.ActivatedusersCount = allUsersRepository.GetAll(a => a.IsActivated).Count();
            ViewBag.ActivatedUsersList = GetActivatedUsersList();

            return View();
        }

        private List<Users> GetActivatedUsersList()
        {
            Repository<User> allUsersRepository = new Repository<User>(Entities);

            var allUsers = allUsersRepository.GetAll(a => a.IsActivated).ToList();

            List<Users> activatedUsersList = new List<Users>();

            foreach (var a in allUsers)
            {
                var users = new Users
                {
                    RegisterDate = a.RegistrationDate,
                    Name = a.Name,
                    Role = a.Roles.RoleName,
                    IsActivated = a.IsActivated
                };

                activatedUsersList.Add(users);
            }

            return activatedUsersList;
        }

        public ViewResult ActiveUsersDuringPeriod(DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null)
                startdate = DateTime.Today;

            if (enddate == null)
                enddate = DateTime.Today;

            Repository<User> allUsersRepository = new Repository<User>(Entities);

            ViewBag.ActiveUsersDuringPeriod = GetActiveUsersList(startdate, enddate);
            ViewBag.ActiveUsersSum =
                allUsersRepository.GetAll(a => a.LastLoginDate >= startdate && a.LastLoginDate <= enddate).Count();

            return View();
        }

        private List<Users> GetActiveUsersList(DateTime? startdate, DateTime? enddate)
        {
            Repository<User> allUsersRepository = new Repository<User>(Entities);

            var allUsers = allUsersRepository.GetAll(a => a.LastLoginDate >= startdate && a.LastLoginDate <= enddate).ToList();

            List<Users> activeUsersList = new List<Users>();

            foreach (var a in allUsers)
            {
                var users = new Users
                {
                    LastActiveDate = a.LastLoginDate,
                    Name = a.Name,
                    Role = a.Roles.RoleName,
                    IsActivated = a.IsActivated
                };

                activeUsersList.Add(users);
            }

            return activeUsersList;
        }

        public ViewResult AddedPlatforms()
        {
            Repository<PerformerPlatform> allPlatforms = new Repository<PerformerPlatform>(Entities);

            ViewBag.AllPlatformsCount = allPlatforms.GetAll().Count();
            ViewBag.VerifiedPlatformsCount = allPlatforms.GetAll(a => a.Verified == VodadModel.Utilities.Constants.CredentialCheckStatuses.Verified).Count();

            ViewBag.AddedPlatforms = GetAddedPlatformsList();

            return View();
        }

        private List<Platforms> GetAddedPlatformsList()
        {
            Repository<PerformerPlatform> allPlatforms = new Repository<PerformerPlatform>(Entities);

            var singlePlatformPlatforms = allPlatforms.GetAll().ToList();

            List<Platforms> singlePlatformPlatformsList = new List<Platforms>();

            foreach (var c in singlePlatformPlatforms)
            {
                var platform = new Platforms
                {
                    UserName = c.User.Name,
                    HyperLink = c.Link,
                    Verified = c.Verified
                };

                singlePlatformPlatformsList.Add(platform);
            }

            return singlePlatformPlatformsList;
        }

        public ViewResult AllAboutMoney()
        {
            Repository<Wallet> allWallets = new Repository<Wallet>(Entities);

            ViewBag.MoneyInTransfers = allWallets.GetAll().Sum(a => a.Transfer) / 2;
            ViewBag.MoneyOnAccounts = allWallets.GetAll().Sum(a => a.Account);
            ViewBag.MoneyOnPerformerAccounts =
                allWallets.GetAll(a => a.User.Roles.RoleName == VodadModel.Utilities.Constants.UserRoles.Performer).Sum(a => a.Account);
            ViewBag.MoneyOnAdvertiserAccounts =
                allWallets.GetAll(a => a.User.Roles.RoleName == VodadModel.Utilities.Constants.UserRoles.Advertiser).Sum(a => a.Account);

            Repository<Transactions> allTransactions = new Repository<Transactions>(Entities);

            ViewBag.SpentInSystemMoney = allTransactions.GetAll(a => a.DateTime == DateTime.Today).Sum(a => a.Amount);

            ViewBag.EarnedMoney =
                allTransactions.GetAll(a => a.DateTime == DateTime.Today && a.User.ReferrerId == null)
                    .Sum(a => a.Amount) * (decimal)0.25 +
                allTransactions.GetAll(
                    a =>
                        a.DateTime == DateTime.Today && a.User.Roles.RoleName == VodadModel.Utilities.Constants.UserRoles.Performer && a.User.ReferrerId != null)
                    .Sum(a => a.Amount) * (decimal)0.24 +
                allTransactions.GetAll(
                    a =>
                        a.DateTime == DateTime.Today && a.User.Roles.RoleName == VodadModel.Utilities.Constants.UserRoles.Advertiser && a.User.ReferrerId != null)
                    .Sum(a => a.Amount) * (decimal)0.23;

            return View();
        }

        public ViewResult ChargedMoney(DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null)
                startdate = DateTime.Today;

            if (enddate == null)
                enddate = DateTime.Today;

            Repository<MoneyTransfers> allMoneyTransfers = new Repository<MoneyTransfers>(Entities);

            ViewBag.ChargedMoneySum = allMoneyTransfers.GetAll(a => a.Action == VodadModel.Utilities.Constants.MoneyTransferAction.Income && a.DateTime >= startdate && a.DateTime <= enddate).Sum(w => w.Amount);
            ViewBag.ChargedMoney = GetChargedMoneyList(startdate, enddate);
            return View();
        }

        private List<Money> GetChargedMoneyList(DateTime? startdate, DateTime? enddate)
        {
            Repository<MoneyTransfers> allMoneyTransfers = new Repository<MoneyTransfers>(Entities);

            var chargedMoney = allMoneyTransfers.GetAll(a => a.Action == VodadModel.Utilities.Constants.MoneyTransferAction.Income && a.DateTime >= startdate && a.DateTime <= enddate).ToList();

            List<Money> chargedMoneyList = new List<Money>();

            foreach (var a in chargedMoney)
            {
                var money = new Money
                {
                    OperationDate = a.DateTime,
                    User = a.User.Name,
                    Role = a.User.Roles.RoleName,
                    Amount = a.Amount
                };

                chargedMoneyList.Add(money);
            }

            return chargedMoneyList;
        }

        public ViewResult InActionOrders()
        {
            ViewBag.InActionOrders = GetInActionOrdersList();

            return View();
        }

        private List<MyOrders> GetInActionOrdersList()
        {
            Repository<Order> allOrders = new Repository<Order>(Entities);

            var inActionOrders = allOrders.GetAll(a => a.Status == VodadModel.Utilities.Constants.OrdersStatuses.Active).ToList();

            List<MyOrders> inActionOrdersList = new List<MyOrders>();

            foreach (var a in inActionOrders)
            {
                var orders = new MyOrders();

                orders.AuthorName = a.User.Name;
                orders.OrderStatus = a.Status;

                inActionOrdersList.Add(orders);
            }

            return inActionOrdersList;
        }

        public ViewResult OrdersCreated(DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null)
                startdate = DateTime.Today;

            if (enddate == null)
                enddate = DateTime.Today;

            ViewBag.CreatedOrders = GetCreatedOrdersList(startdate, enddate);

            return View();
        }

        private List<MyOrders> GetCreatedOrdersList(DateTime? startdate, DateTime? enddate)
        {
            Repository<Order> allOrders = new Repository<Order>(Entities);

            var createdOrders = allOrders.GetAll(a => a.CreationDate >= startdate && a.CreationDate <= enddate).ToList();

            ViewBag.CreatedOrdersCount =
                allOrders.GetAll(a => a.CreationDate >= startdate && a.CreationDate <= enddate).Count();

            List<MyOrders> createdOrdersList = new List<MyOrders>();

            foreach (var a in createdOrders)
            {
                var orders = new MyOrders
                {
                    CreationDate = a.CreationDate,
                    AuthorName = a.User.Name,
                    OrderStatus = a.Status
                };

                createdOrdersList.Add(orders);
            }

            return createdOrdersList;
        }

        public ViewResult PerformedOrders(DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null)
                startdate = DateTime.Today;

            if (enddate == null)
                enddate = DateTime.Today;

            Repository<OrderPerformed> allOrders = new Repository<OrderPerformed>(Entities);

            ViewBag.PerformedOrdersList = GetPerformedOrdersList(startdate, enddate);

            ViewBag.PerformedOrdersCount = allOrders.GetAll(a => a.StartDate >= startdate && a.StartDate <= enddate).Count();

            return View();
        }

        private List<MyOrders> GetPerformedOrdersList(DateTime? startdate, DateTime? enddate)
        {
            Repository<OrderPerformed> allOrders = new Repository<OrderPerformed>(Entities);

            var performedOrders = allOrders.GetAll(a => a.StartDate >= startdate && a.StartDate <= enddate).ToList();

            List<MyOrders> performedOrdersList = new List<MyOrders>();

            foreach (var a in performedOrders)
            {
                var orders = new MyOrders
                {
                    AuthorName = a.User.Name,
                    MoneyPaid = a.MoneyPaid,
                    EndDate = a.StartDate
                };

                performedOrdersList.Add(orders);
            }

            return performedOrdersList;
        }

        public ViewResult RegisteredUsers(DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null)
                startdate = DateTime.Today;

            if (enddate == null)
                enddate = DateTime.Today;

            Repository<User> allUsersRepository = new Repository<User>(Entities);

            ViewBag.RegisteredUsers = GetAllRegisteredUsersList(startdate, enddate);
            ViewBag.RegisteredUsersCount =
                allUsersRepository.GetAll(a => a.RegistrationDate >= startdate && a.RegistrationDate <= enddate).Count();

            return View();
        }

        private List<Users> GetAllRegisteredUsersList(DateTime? startdate, DateTime? enddate)
        {
            Repository<User> allUsersRepository = new Repository<User>(Entities);

            var allUsers = allUsersRepository.GetAll(a => a.RegistrationDate >= startdate && a.RegistrationDate <= enddate).ToList();

            List<Users> activatedUsersList = new List<Users>();

            foreach (var a in allUsers)
            {
                var users = new Users
                {
                    RegisterDate = a.RegistrationDate,
                    Name = a.Name,
                    Role = a.Roles.RoleName,
                    IsActivated = a.IsActivated
                };

                activatedUsersList.Add(users);
            }

            return activatedUsersList;
        }

        public ViewResult WithdrawedMoney(DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null)
                startdate = DateTime.Today;

            if (enddate == null)
                enddate = DateTime.Today;

            Repository<MoneyTransfers> allMoneyTransfers = new Repository<MoneyTransfers>(Entities);

            ViewBag.WithdrawedMoneySum = allMoneyTransfers.GetAll(a => a.Action == VodadModel.Utilities.Constants.MoneyTransferAction.Outcome && a.DateTime >= startdate && a.DateTime <= enddate).Sum(w => w.Amount);
            ViewBag.WithdrawedMoney = GetWithdrawedMoneyList(startdate, enddate);

            return View();
        }

        private List<Money> GetWithdrawedMoneyList(DateTime? startdate, DateTime? enddate)
        {
            Repository<MoneyTransfers> allMoneyTransfers = new Repository<MoneyTransfers>(Entities);

            var withdrawedMoney = allMoneyTransfers.GetAll(a => a.Action == VodadModel.Utilities.Constants.MoneyTransferAction.Outcome && a.DateTime >= startdate && a.DateTime <= enddate).ToList();

            List<Money> withdrawedMoneyList = new List<Money>();

            foreach (var a in withdrawedMoney)
            {
                var money = new Money
                {
                    OperationDate = a.DateTime,
                    User = a.User.Name,
                    Role = a.User.Roles.RoleName,
                    Amount = a.Amount
                };

                withdrawedMoneyList.Add(money);
            }

            return withdrawedMoneyList;
        }

    }
}
