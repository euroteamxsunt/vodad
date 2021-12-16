using System;
using System.ComponentModel.DataAnnotations;
using VodadModel;

namespace Vodad.Models
{
    public class AddNewsModel
    {
        [Required]
        [Display(Name = "Title:")]
        public string NewsTitle { get; set; }

        [Required]
        [Display(Name = "Text:")]
        public string NewsText { get; set; }
    }

    public class EditNewsModel
    {
        [Required]
        [Display(Name = "News id:")]
        public int? NewsId { get; set; }

        [Required]
        [Display(Name = "Title:")]
        public string NewsTitle { get; set; }

        [Required]
        [Display(Name = "Text:")]
        public string NewsText { get; set; }
    }

    public class NewsList
    {
        [Required]
        [Display(Name = "News id:")]
        public int? NewsId { get; set; }

        [Required]
        [Display(Name = "Author:")]
        public string AuthorName { get; set; }

        [Required]
        [Display(Name = "Creation date:")]
        public DateTime? CreationDate { get; set; }

        [Required]
        [Display(Name = "Title:")]
        public string NewsTitle { get; set; }

        [Required]
        [Display(Name = "Anons:")]
        public string NewsAnons { get; set; }

        [Required]
        [Display(Name = "Text:")]
        public string NewsText { get; set; }
    }
}
