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
    
    public partial class Timezone
    {
        public Timezone()
        {
            this.User = new HashSet<User>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<User> User { get; set; }
    }
}
