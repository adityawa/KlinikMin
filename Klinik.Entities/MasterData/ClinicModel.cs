using Klinik.Entities;
using System;
using System.ComponentModel.DataAnnotations;
namespace Klinik.Entities.MasterData
{
    public class ClinicModel : BaseModel
    {
        [Required(ErrorMessage = "Please fill code")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Please fill Name")]
        public string Name { get; set; }
        public string Address { get; set; }
        public string LegalNumber { get; set; }
        public DateTime LegalDate { get; set; }
        public string LegalDateDesc { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public float Long { get; set; }
        public float Lat { get; set; }
        public short CityId { get; set; }
        public string CityDesc { get; set; }
        public short ClinicType { get; set; }

        public string ClinicTypeDesc { get; set; }
        public int ReffID { get; set; }
    }
}