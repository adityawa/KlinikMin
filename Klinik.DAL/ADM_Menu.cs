//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Klinik.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ADM_Menu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ADM_Menu()
        {
            this.ADM_ModuleAuthorization = new HashSet<ADM_ModuleAuthorization>();
        }
    
        public int Id { get; set; }
        public string Description { get; set; }
        public Nullable<int> ParentMenuID { get; set; }
        public string PageLink { get; set; }
        public int SortIndex { get; set; }
        public Nullable<bool> HasChild { get; set; }
        public bool IsMenu { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADM_ModuleAuthorization> ADM_ModuleAuthorization { get; set; }
    }
}