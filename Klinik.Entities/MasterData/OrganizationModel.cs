using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Klinik.Entities.MasterData
{
    public class OrganizationModel : BaseModel
    {
        [Required(ErrorMessage = "Please enter Organization Code"), MaxLength(30)]
        [Display(Name = "Organization Code")]
        public string OrgCode { get; set; }

        [Required(ErrorMessage = "Please enter Organization Name"), MaxLength(50)]
        [Display(Name = "Organization Name")]
        public string OrgName { get; set; }

        [Display(Name = "Klinik")]
        public int KlinikID { get; set; }

        public IEnumerable<SelectListItem> ClinicLists { get; set; }
    }
}