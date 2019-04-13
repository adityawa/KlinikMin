namespace Klinik.Entities.MasterData
{
    public class PoliFlowTemplateModel : BaseModel
    {
        public string Code { get; set; }
        public short PoliTypeID { get; set; }
        public short PoliTypeIDTo { get; set; }
    }
}
