namespace Klinik.Entities.Form
{
    public class FormExamineMedicineModel : BaseModel
    {
        public long? FormExamineID { get; set; }
        public string TypeID { get; set; }
        public int? ProductID { get; set; }
        public double? Qty { get; set; }
        public string ConcoctionMedicine { get; set; }
        public string RemarkUse { get; set; }
    }
}
