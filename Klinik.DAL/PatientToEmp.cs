//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Klinik.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class PatientToEmp
    {
        public int id { get; set; }
        public Nullable<int> PatientID { get; set; }
        public Nullable<int> EmpID { get; set; }
        public Nullable<short> RowStatus { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
    }
}
