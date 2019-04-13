using System.ComponentModel.DataAnnotations;

namespace Klinik.Entities.MasterData
{
    public class PrivilegeModel : BaseModel
    {
        [Required(ErrorMessage = "Please enter Organization Code"), MaxLength(150)]
        [Display(Name = "Privilege Name")]
        public string Privilige_Name { get; set; }

        [Display(Name = "Privilege Desc")]
        public string Privilege_Desc { get; set; }

        public long MenuID { get; set; }

        public string MenuDesc { get; set; }
    }
}