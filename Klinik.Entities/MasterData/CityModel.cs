using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.MasterData
{
    public class CityModel :BaseModel
    {
        public string Province { get; set; }
        public string City { get; set; }
        public string Kelurahan { get; set; }
        public string Kecamatan { get; set; }
    }
}
