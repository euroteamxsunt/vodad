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
    
    public partial class Messages
    {
        public long Id { get; set; }
        public Nullable<long> FromUserId { get; set; }
        public Nullable<long> ToUserId { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string MessageText { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public string MessageTitle { get; set; }
        public Nullable<bool> IsDeletedForAuthor { get; set; }
        public Nullable<bool> IsDeletedForReciever { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
