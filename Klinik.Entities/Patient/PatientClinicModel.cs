using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Patient
{
    public class PatientClinicModel : BaseModel
    {
        public long PatientID { get; set; }
        public long clinicID { get; set; }
        public string TempAddress { get; set; }
        public int TempCityId { get; set; }
        public string TempCity { get; set; }
        public string RefferencePerson { get; set; }
        public string RefferenceNumber { get; set; }
        public int RefferenceRelation { get; set; }
        public string RefferenceRelationDesc { get; set; }
        public string OldMRNumber { get; set; }
        public long photoID { get; set; }
    }
}
