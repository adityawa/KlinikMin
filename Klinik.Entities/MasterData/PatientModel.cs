using Klinik.Entities.Patient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Klinik.Entities.Document;
namespace Klinik.Entities.MasterData
{
    public class PatientModel : BaseModel
    {
        public long EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public long familyRelationshipID { get; set; }
        public string familyRelationshipDesc { get; set; }
        public string MRNumber { get; set; }
        [Required(ErrorMessage = "Please Enter a name")]
        public string Name { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Please Enter a birthdate")]
        public string BirthDateStr { get; set; }

        public string KTPNumber { get; set; }
        [Required(ErrorMessage = "Please Enter an Address")]
        public string Address { get; set; }
        public int CityID { get; set; }
        public string CityNm { get; set; }
        public short Type { get; set; }
        public string TypeDesc { get; set; }
        public string BPJSNumber { get; set; }
        public string BloodType { get; set; }

        public string PatientKey { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase file { get; set; }
        public DocumentModel Photo { get; set; }
        public bool? IsUseExistingData { get; set; }
        public PatientClinicModel PatientClinic { get; set; }
        public bool IsFromRegistration { get; set; }
    }
}
