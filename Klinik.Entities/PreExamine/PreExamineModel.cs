using Klinik.Entities.Loket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.PreExamine
{
    public class PreExamineModel : BaseModel
    {
        public long FormMedicalID { get; set; }
        public DateTime TransDate { get; set; }
        public string strTransDate { get; set; }
        public int DoctorID { get; set; }
        public float Temperature { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
        public float Respitory { get; set; }
        public int Pulse { get; set; }
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public string Others { get; set; }
        public string RightEye { get; set; }
        public string LeftEye { get; set; }
        public string ColorBlind { get; set; }
        public DateTime MenstrualDate { get; set; }
        public DateTime KBDate { get; set; }
        public string strMenstrualDate { get; set; }
        public string strKBDate { get; set; }
        public string DailyGlasses { get; set; }
        public string ExamineGlasses { get; set; }

        public LoketModel LoketData { get; set; }
    }
}
