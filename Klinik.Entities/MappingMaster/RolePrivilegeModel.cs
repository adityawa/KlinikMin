using System.Collections.Generic;

namespace Klinik.Entities.MappingMaster
{
    public class RolePrivilegeModel : BaseModel
    {
        public long RoleID { get; set; }
        public long PrivilegeID { get; set; }
        public string RoleDesc { get; set; }
        public string PrivilegeName { get; set; }
        public string PrivilegeDesc { get; set; }
        public List<long> PrivilegeIDs { get; set; }

        public RolePrivilegeModel()
        {
            PrivilegeIDs = new List<long>();
        }
    }
}