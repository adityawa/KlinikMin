using System;

namespace Klinik.Entities.PoliSchedules
{
    public class PoliScheduleModel : BaseModel
    {
        public long ClinicID { get; set; }
        public string ClinicName { get; set; }
        public int DoctorID { get; set; }
        public string DoctorName { get; set; }
        public int PoliID { get; set; }
        public string PoliName { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateStr { get; set; }
        public DateTime EndDate { get; set; }
        public string EndDateStr { get; set; }
        public long ReffID { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public string Remark { get; set; }
    }
}
