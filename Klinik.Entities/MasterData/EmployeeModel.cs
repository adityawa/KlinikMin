using Klinik.Data.DataRepository;
using Klinik.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Klinik.Entities.MasterData
{
    public class EmployeeModel : BaseModel
    {
        
        [Display(Name = "Employee ID")]
        public string EmpID { get; set; }
        [Required(ErrorMessage = "Please enter Employee Name"), MaxLength(100)]
        [Display(Name = "Name")]
        public string EmpName { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$|^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Please Fill BirthDate")]
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        public string BirthdateStr { get; set; }

        public string ReffEmpID { get; set; }

        public string Gender { get; set; }

        [Required(ErrorMessage = "Please enter Employment Type")]
        public short EmpType { get; set; }

        public string EmpTypeDesc { get; set; }

        public string KTPNumber { get; set; }
        public string HPNumber { get; set; }

       

        public string StatusCode { get; set; }

        public short Status { get; set; }

        //employee assignment

        [Required(ErrorMessage = "Please Fill Join Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }

        public string Department { get; set; }

        public string Region { get; set; }

        public string BussinesUnit { get; set; }

        public short EmpStatus { get; set; }

        public string EmpStatusDesc { get; set; }

        public string LastEmpId { get; set; }

        public string Grade { get; set; }

        public bool IsFromAPI { get; set; }

    }
}