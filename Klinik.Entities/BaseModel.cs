using Klinik.Entities.Account;
using System;

namespace Klinik.Entities
{
    public class BaseModel
    {
        public long Id { get; set; }
        public short RowStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public AccountModel Account { get; set; }
    }
}