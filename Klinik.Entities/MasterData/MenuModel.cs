namespace Klinik.Entities.MasterData
{
    public class MenuModel : BaseModel
    {
        public string Description { get; set; }
        public long ParentMenuId { get; set; }
        public string PageLink { get; set; }
        public int SortIndex { get; set; }
        public bool? HasChild { get; set; }
        public bool? IsMenu { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int Level { get; set; }
        public string icon { get; set; }
    }
}