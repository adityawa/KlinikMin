using System;

namespace Klinik.Entities.Loket
{
    public class LoketModel : BaseModel
    {
        public int ClinicID { get; set; }
        public string ClinicName { get; set; }
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public string MRNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionDateStr { get; set; }
        public int Type { get; set; }
        public string TypeStr { get; set; }
        public int AppointmentID { get; set; }
        public int SortNumber { get; set; }
        public string SortNumberCode { get; set; }
        public int PoliFromID { get; set; }
        public int CurrentPoliID { get; set; }
        public string PoliFromName { get; set; }
        public int PoliToID { get; set; }
        public string PoliToName { get; set; }
        public string Remark { get; set; }
        public int ReffID { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public int DoctorID { get; set; }
        public string DoctorStr { get; set; }
        public bool IsFromDashboard { get; set; }
        public string PatientBirthDateStr { get; set; }
        public string PatientHPNumber { get; set; }
        public string PatientGender { get; set; }
        public string PatientAddress { get; set; }
        public string PatientKTPNumber { get; set; }
        public string PatientBPJSNumber { get; set; }
        public string PatientBloodType { get; set; }
        public string PatientType { get; set; }
        public int NecessityType { get; set; }
        public int PaymentType { get; set; }
        public string PaymentNumber { get; set; }
        public bool IsPreExamine { get; set; }
        public string strIsPreExamine { get; set; }
    }
}
