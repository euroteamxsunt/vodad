using System.ComponentModel.DataAnnotations;
using System.Web;
using VodadModel;

namespace Vodad.Models
{
    public class AddNewTicketModel
    {
        [Required]
        [Display(Name = "Title:")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Select message theme:")]
        public int? ThemeId { get; set; }

        [Required]
        [Display(Name = "Message text:")]
        public string Text { get; set; }

        [Display(Name = "Select screenshot(optional):")]
        public HttpPostedFileBase ImageData { get; set; }

        public int? ParentTicketId { get; set; }
    }

    public class ManageTicketsModel
    {
        public int? TicketId { get; set; }

        public string TicketTheme { get; set; }

        public string TicketTitle { get; set; }

        public string TicketShortTitle { get; set; }

        public string TicketText { get; set; }

        public string TicketStatus { get; set; }

        public int? TicketImageId { get; set; }

        public string TicketCreationDate { get; set; }

        public string TicketAnswerDate { get; set; }

        public string TicketCloseDate { get; set; }

        public int? TicketParentTicketId { get; set; }

        public string TicketParentTicketShortTitle { get; set; }

        public string TicketAdminAnswer { get; set; }

        public string TicketAdminCloseComment { get; set; }

        public int? TicketCreatorId { get; set; }

        public int? TicketAnswerAdminId { get; set; }

        public int? TicketCloseAdminId { get; set; }

        public string TicketCreatorName { get; set; }

        public string TicketAnswerAdminName { get; set; }

        public string TicketCloseAdminName { get; set; }
    }

    public class TicketDetailsModel
    {
        public int? TicketId { get; set; }

        public string TicketTitle { get; set; }

        public string TicketTheme { get; set; }

        public int? TicketCreatorId { get; set; }

        public string TicketCreatorName { get; set; }

        public int? TicketCloseAdminId { get; set; }

        public string TicketCloseAdminName { get; set; }

        public string TicketAdminCloseComment { get; set; }

        public string TicketCloseDate { get; set; }

        public string TicketAnswer { get; set; }

        public string TicketNewStatus { get; set; }

        [Display(Name = "Select screenshot(optional):")]
        public HttpPostedFileBase ImageData { get; set; }
    }

    public class TicketHistoryModel
    {
        public int? TicketId { get; set; }

        public string TicketTitle { get; set; }

        public string TicketText { get; set; }

        public int? TicketImageId { get; set; }

        public string TicketCreationDate { get; set; }

        public int? TicketCreatorId { get; set; }

        public string TicketCreatorName { get; set; }

        public string TicketAnswerDate { get; set; }

        public string TicketAdminAnswerName { get; set; }

        public int? TicketAdminAnswerId { get; set; }

        public string TicketAnswer { get; set; }
    }
}
