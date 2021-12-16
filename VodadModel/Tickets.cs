//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VodadModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tickets
    {
        public Tickets()
        {
            this.Tickets1 = new HashSet<Tickets>();
        }
    
        public long Id { get; set; }
        public string Title { get; set; }
        public Nullable<long> ThemeId { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public Nullable<long> ImageId { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<System.DateTime> AnswerDate { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }
        public Nullable<long> ParentTicketId { get; set; }
        public string AdminAnswer { get; set; }
        public string AdminCloseComment { get; set; }
        public Nullable<long> CreatorId { get; set; }
        public Nullable<long> AnswerAdminId { get; set; }
        public Nullable<long> CloseAdminId { get; set; }
        public string Status { get; set; }
    
        public virtual Image Image { get; set; }
        public virtual ICollection<Tickets> Tickets1 { get; set; }
        public virtual Tickets Tickets2 { get; set; }
        public virtual TicketThemes TicketThemes { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}
