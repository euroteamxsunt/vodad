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
    
    public partial class Order
    {
        public Order()
        {
            this.OrderContent = new HashSet<OrderContent>();
            this.OrderThemes = new HashSet<OrderThemes>();
        }
    
        public long Id { get; set; }
        public Nullable<long> UserId { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
        public string Status { get; set; }
        public Nullable<long> RegionId { get; set; }
        public string Name { get; set; }
    
        public virtual Regions Regions { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderContent> OrderContent { get; set; }
        public virtual ICollection<OrderThemes> OrderThemes { get; set; }
    }
}