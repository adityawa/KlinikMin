namespace Klinik.Entities
{
    public class BaseRequest<T> where T : class
    {
        public string Draw { get; set; }
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public string SortColumn { get; set; }
        public string SortColumnDir { get; set; }
        public string SearchValue { get; set; }
        public string Action { get; set; }
        public T Data { get; set; }
    }
}