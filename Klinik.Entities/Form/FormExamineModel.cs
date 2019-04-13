using System;

namespace Klinik.Entities.Form
{
    public class FormExamineModel : BaseModel
    {
        public long? FormMedicalID { get; set; }
        public int? PoliID { get; set; }
        public DateTime? TransDate { get; set; }
        public int? DoctorID { get; set; }
        public string Diagnose { get; set; }
        public string Therapy { get; set; }
        public string Remark { get; set; }
        public string ICDInformation { get; set; }
        public string Result { get; set; }
    }
}
