namespace Klinik.Entities.Form
{
    public class FormExamineLabModel : BaseModel
    {
        public long? FormExamineID { get; set; }
        public string LabType { get; set; }
        public int? LabItemID { get; set; }
        public string Result { get; set; }
        public string ResultIndicator { get; set; }
    }
}
