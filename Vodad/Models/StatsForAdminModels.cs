using System;

namespace Vodad.Models
{
    public class Money
    {
        public DateTime? OperationDate { get; set; }
        public string User { get; set; }
        public string Role { get; set; }
        public decimal? Amount { get; set; }
    }

    public class MyOrders
    {
        public DateTime? CreationDate { get; set; }
        public string AuthorName { get; set; }
        public decimal? MoneyPaid { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class Platforms
    {
        public string UserName { get; set; }
        public string PlatformName { get; set; }
        public string HyperLink { get; set; }
        public string Verified { get; set; }
    }

    public class Users
    {
        public DateTime? RegisterDate { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public bool IsActivated { get; set; }
        public DateTime? LastActiveDate { get; set; }
    }
}