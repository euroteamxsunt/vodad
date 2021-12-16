using System;
using System.ComponentModel.DataAnnotations;

namespace Vodad.Models
{
    public class SendMessageModel
    {
        [Required]
        [Display(Name = "To:")]
        public string ToUserId { get; set; }

        [Required]
        [Display(Name = "Message text::")]
        public string MessageText { get; set; }
    }

    public class ReadMessageModel
    {
        [Required]
        [Display(Name = "From:")]
        public string FromUserEmail { get; set; }

        [Required]
        [Display(Name = "User id:")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Message text:")]
        public string MessageText { get; set; }

        [Required]
        [Display(Name = "Creation date:")]
        public string DateTime { get; set; }

        [Required]
        [Display(Name = "Creation date:")]
        public string DateTimeForSorting { get; set; }
    }

    public class MessengersModel
    {
        [Required]
        [Display(Name = "From:")]
        public string FromUserEmail { get; set; }

        [Required]
        [Display(Name = "User id:")]
        public int? UserId { get; set; }

        [Required]
        [Display(Name = "Companion id:")]
        public int? CompanionId { get; set; }

        [Required]
        [Display(Name = "Messages count:")]
        public int? MessagesCount { get; set; }

        [Required]
        [Display(Name = "Unread:")]
        public int? MessagesUnreadCount { get; set; }

        [Required]
        [Display(Name = "Last message date:")]
        public DateTime? LastMessageDateTime { get; set; }
    }
}
