using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.MasterData
{
    public class EmployeeStatusModel : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public string Description { get; set; }
    }
}
