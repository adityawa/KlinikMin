using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Klinik.Entities.MappingMaster
{
    public class OrganizationPrivilegeModel : BaseModel
    {
        [Required(ErrorMessage = "Organization ID must be filled")]
        [Display(Name = "Organization ID")]
        public long OrgID { get; set; }

        [Required(ErrorMessage = "Privilege ID must be filled")]
        public long PrivilegeID { get; set; }

        public List<long> PrivilegeIDs { get; set; }

        public string OrganizationName { get; set; }

        [Display(Name = "PrivilegeName")]
        public string PrivileveName { get; set; }

        public string PrivilegeDesc { get; set; }

        public OrganizationPrivilegeModel()
        {
            PrivilegeIDs = new List<long>();
        }
    }
}