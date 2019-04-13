using System.Collections.Generic;

namespace Klinik.Entities.MappingMaster
{
    public class ClinicPoliModel : BaseModel
    {
        public long ClinicID { get; set; }
        public int PoliID { get; set; }
        public string Code { get; set; }
        public string ClinicName { get; set; }
        public List<int> ListPoliId { get; set; }

        public ClinicPoliModel()
        {
            ListPoliId = new List<int>();
        }
    }
}
