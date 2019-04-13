using System;
using System.ComponentModel.DataAnnotations;

namespace Klinik.Entities.PoliSchedules
{
    public class PoliScheduleMasterModel : BaseModel
    {
        [Required]
        public long ClinicID { get; set; }
        public string ClinicName { get; set; }
        [Required]
        public int DoctorID { get; set; }
        public string DoctorName { get; set; }
        [Required]
        public int PoliID { get; set; }
        public string PoliName { get; set; }
        [Required]
        public int Day { get; set; }
        public string DayName { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        public string StartTimeStr { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        public string EndTimeStr { get; set; }
    }
}
