using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Klinik.Entities.MasterData
{
    public class EmployeeAssignmentModel : BaseModel
    {
        public long EmployeeID { get; set; }

        //[Required(ErrorMessage = "Please Fill Join Date")]
        //[DataType(DataType.Date)]
        //public DateTime StartDate { get; set; }

        //[DataType(DataType.Date)]
        //public DateTime EndDate { get; set; }

        //public string StartDateStr { get; set; }
        //public string EndDateStr { get; set; }

        //public string Department { get; set; }

        //public string Region { get; set; }

        //public string BussinesUnit { get; set; }

        //public short EmpStatus { get; set; }

        //public string EmpStatusDesc { get; set; }

        //public string LastEmpId { get; set; }
    }
}
