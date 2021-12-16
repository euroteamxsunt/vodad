using System.ComponentModel.DataAnnotations;
using VodadModel;

namespace Vodad.Models
{
    public class BlackAndWhiteListsModel
    {
        [Required]
        [Display(Name = "User id:")]
        public int? UserId { get; set; }

        [Required]
        [Display(Name = "User Email:")]
        public string UserEmail { get; set; }
    }
}
