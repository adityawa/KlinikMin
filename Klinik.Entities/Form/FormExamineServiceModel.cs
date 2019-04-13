using System;

namespace Klinik.Entities.Form
{
    public class FormExamineServiceModel : BaseModel
    {
        public long? FormExamineID { get; set; }
        public int? ServiceID { get; set; }
    }
}
