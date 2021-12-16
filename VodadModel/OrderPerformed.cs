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
    
    public partial class OrderPerformed
    {
        public long Id { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> MoneyPaid { get; set; }
        public Nullable<long> OrderContentId { get; set; }
        public long AuthorId { get; set; }
        public Nullable<bool> IsLiked { get; set; }
        public Nullable<long> PerformerPlatformId { get; set; }
        public string VideoLink { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> LastStatusChangeDateTime { get; set; }
    
        public virtual OrderContent OrderContent { get; set; }
        public virtual PerformerPlatform PerformerPlatform { get; set; }
        public virtual User User { get; set; }
    }
}