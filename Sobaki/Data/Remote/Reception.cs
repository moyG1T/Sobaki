//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sobaki.Data.Remote
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reception
    {
        public int Id { get; set; }
        public int DogId { get; set; }
        public int VeterinarId { get; set; }
        public Nullable<System.DateTime> Timestamp { get; set; }
        public string Disease { get; set; }
        public string Comment { get; set; }
        public string Recommendation { get; set; }
    
        public virtual Dog Dog { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
