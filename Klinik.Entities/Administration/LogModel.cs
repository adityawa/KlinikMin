using System;

namespace Klinik.Entities.Administration
{
    public class LogModel
    {
        public DateTime Start { get; set; }

        public string StartStr { get; set; }

        public string Module { get; set; }

        public string Command { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string Status { get; set; }

        public string UserName { get; set; }

        public string Organization { get; set; }

        public long Account { get; set; }

        public long Id { get; set; }

        public short RowStatus { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public int Type { get; set; }
    }
}
