using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Entities.Document
{
    public class DocumentModel : BaseModel
    {
        public string ActualPath { get; set; }
        public string ActualName { get; set; }
        public string SourceTable { get; set; }
        public string TypeDoc { get; set; }
        
    }
}
