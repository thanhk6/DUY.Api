namespace C.Tracking.API.Model.Category
{
    public class CategoryContTypeModel
    {
        public long id { get; set; }
        public string name { get; set; }
        public string note { get; set; }    
        public bool is_delete { set; get; } = false; 
    }
}
