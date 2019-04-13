using System;

namespace Klinik.Entities.Form
{
    public class FormMedicalModel : BaseModel
    {
        public long? ClinicID { get; set; }
        public long? PatientID { get; set; }
        public string Necessity { get; set; }
        public string PaymentType { get; set; }
        public string Number { get; set; }
        public string ClaimNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public double? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? BenefitPaid { get; set; }
        public string BenefitPlan { get; set; }
        public string Remark { get; set; }
    }
}
