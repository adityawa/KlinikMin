using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.MasterData
{
    public class LabItemCategoryModel : BaseModel
    {
        public string LabType { get; set; }
        public int? PoliID { get; set; }
        public string PoliName { get; set; }
        public string Name { get; set; }
    }
}
