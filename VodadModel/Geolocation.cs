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
    
    public partial class Geolocation
    {
        public Geolocation()
        {
            this.GeolocationPlatformPercentage = new HashSet<GeolocationPlatformPercentage>();
        }
    
        public long Id { get; set; }
        public string CountryName { get; set; }
        public string ISO2 { get; set; }
        public string LongCountryName { get; set; }
        public string ISO3 { get; set; }
        public string NumCode { get; set; }
        public string UNMemberState { get; set; }
        public string CallingCode { get; set; }
        public string CCTLD { get; set; }
    
        public virtual ICollection<GeolocationPlatformPercentage> GeolocationPlatformPercentage { get; set; }
    }
}
