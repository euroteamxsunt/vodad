namespace VodadModel.Utilities
{
    public static class Constants
    {
        public class AlertMessages
        {
            public const string Success = "success";
            public const string Failed = "failed";
        }

        public class OrdersStatuses
        {
            public const string Open = "open";
            public const string Closed = "closed";
            public const string Active = "active";
            public const string Deleted = "deleted";
            public const string Deny = "deny";
            public const string Block = "block";
            public const string Unblock = "unblock";
            public const string Pay = "pay";
        }

        public class TicketStatuses
        {
            public const string Verifying = "verifying";
            public const string Answered = "answered";
            public const string Closed = "closed";
        }

        public class CredentialCheckStatuses
        {
            public const string Verified = "verified";
            public const string Unverified = "unverified";
            public const string Failed = "failed";
            public const string Notpassed = "notpassed";
            public const string Started = "started";
            public const string Deleted = "deleted";
        }

        public class StatisticsStatus
        {
            public const string Processing = "processing";
            public const string Created = "created";
            public const string Finished = "finished";
            public const string Failed = "failed";
        }

        public class VerificationStatuses
        {
            public const string Inaction = "inaction";
            public const string Verifying = "verifying";
            public const string Complete = "complete";
            public const string Request = "request";
        }

        public class VideoStatuses
        {
            public const string NoLength = "nolength";
            public const string Correct = "correct";
            public const string NotProcessed = "notprocessed";
        }

        public class ImageStatuses
        {
            public const string Contains = "contains";
            public const string Notcontains = "notcontains";
        }

        public class OnlineStatuses
        {
            public const string Online = "online";
            public const string Offline = "offline";
        }

        public class ActionOrderPerformedStatus
        {
            public const string CanBeDeleted = "canBeDeleted";
            public const string IsNotLiked = "isNotLiked";
        }

        public class MoneyTransferAction
        {
            public const string Income = "income";
            public const string Outcome = "outcome";
        }

        public class BannerParameters
        {
            public const decimal MaxWidth = 1920;
            public const decimal MaxHeight = 1080;
        }

        public class UserMerchantAccountStatus
        {
            public const string Active = "active";
            public const string Waiting = "waiting";
            public const string Changing = "changing";
            public const string Blocked = "blocked";
        }

        public class WithdrawalStatus
        {
            public const string Waiting = "waiting";
            public const string Processing = "processing";
            public const string Complete = "complete";
            public const string Failed = "failed";
        }

        public class CertificatesStatus
        {
            public const string Waiting = "waiting";
            public const string Activated = "activated";
        }

        public class UserRoles
        {
            public const string Administrator = "Administrator";
            public const string Advertiser = "Advertiser";
            public const string Banned = "Banned";
            public const string Helper = "Helper";
            public const string Performer = "Performer";
        }

        public class ContentType
        {
            public const string Image = "Image";
            public const string Video = "Video";
        }

        public class AdminConstants
        {
            public const string Email = "vodadadm@gmail.com";
        }

        public class YouTube
        {
            public const string Devkey = "AIzaSyC_1NlTpZJ3rQq931hmNNDPHB7XUS9v670";
            public const string Username = "vodadadm@gmail.com";
            public const string Password = "san557641";
            public const string AppName = "vodadtestproject";
        }
    }
}
