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
    
    public partial class WhiteList
    {
        public long Id { get; set; }
        public Nullable<long> OwnerId { get; set; }
        public Nullable<long> UserId { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
