using Klinik.Entities;
using System.Collections.Generic;

namespace Klinik.Entities.MappingMaster
{
    public class UserRoleModel : BaseModel
    {
        public long UserID { get; set; }
        public long RoleID { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public List<long> RoleIds { get; set; }

        public UserRoleModel()
        {
            RoleIds = new List<long>();
        }
    }
}