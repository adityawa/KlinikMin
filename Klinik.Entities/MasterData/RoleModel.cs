using System.ComponentModel.DataAnnotations;

namespace Klinik.Entities.MasterData
{
    public class RoleModel : BaseModel
    {
        [Required]
        public long OrgID { get; set; }

        public string OrganizationName { get; set; }

        [Required(ErrorMessage = "Please enter Role Name"), MaxLength(30)]
        public string RoleName { get; set; }
    }
}