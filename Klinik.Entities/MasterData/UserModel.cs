using System;
using System.ComponentModel.DataAnnotations;

namespace Klinik.Entities.MasterData
{
    public class UserModel : BaseModel
    {
        [Display(Name = "Organization")]
        [Required(ErrorMessage = "Please enter Organization")]
        public long OrgID { get; set; }

        [Display(Name = "Organization")]
        public string OrganizationName { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Please enter User Name"), MaxLength(50)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter Password"), MaxLength(250)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter Employee")]
        public long EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Expired Date")]
        public DateTime? ExpiredDate { get; set; }

        public string ExpiredDateStr { get; set; }

        public string StatusDesc { get; set; }

        public bool Status { get; set; }

        public long RoleID { get; set; }
    }
}