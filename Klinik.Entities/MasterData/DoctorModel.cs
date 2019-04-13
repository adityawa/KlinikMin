using System;

namespace Klinik.Entities.MasterData
{
    public class DoctorModel : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? SpecialistID { get; set; }
        public int TypeID { get; set; }
        public long EmployeeID { get; set; }
        public string TypeName { get; set; }
        public string SpecialistName { get; set; }
        public string KTPNumber { get; set; }
        public string STRNumber { get; set; }
        public DateTime? STRValidFrom { get; set; }
        public DateTime? STRValidTo { get; set; }
        public string STRValidFromStr { get; set; }
        public string STRValidToStr { get; set; }
        public string Address { get; set; }
        public string HPNumber { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
        public int OrganizationID { get; set; }
        public string Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
