using System.Collections.Generic;

namespace Klinik.Entities
{
    public class BaseResponse<T> where T : class
    {
        public string Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
        public List<T> Data { get; set; }
        public T Entity { get; set; }
        public bool Status { get; set; } = true;
        public bool IsNeedConfirmation { get; set; }
        public string Message { get; set; }
    }
}