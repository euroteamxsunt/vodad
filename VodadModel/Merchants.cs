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
    
    public partial class Merchants
    {
        public Merchants()
        {
            this.MoneyTransfers = new HashSet<MoneyTransfers>();
            this.UserMerchants = new HashSet<UserMerchants>();
        }
    
        public long Id { get; set; }
        public string MerchantName { get; set; }
    
        public virtual ICollection<MoneyTransfers> MoneyTransfers { get; set; }
        public virtual ICollection<UserMerchants> UserMerchants { get; set; }
    }
}
