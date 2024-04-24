using DUY.API.Entities;

namespace DUY.API.Model.Category
{
    public class Category_StoreModel
    {
        public long id { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public double capital_value { get; set; }
        public double brand_value { get; set; }
        public double acreage { get; set; }

        public string? lat { get; set; }
        public string? lng { get; set; }
        public string? note { set; get; }
        public int type { set; get; } = 0; // 0 kho, 1 van phong , 2 cua hang
        public int status { set; get; } = 0; // 0 dang xay dung, 1 dang hoat dong, 2 dung hoat dong
        public DateTime? opening_day { set; get; }
        public string? status_name { set; get; }
        public long userAdded { set; get; }
        public long? userUpdated { set; get; }
        public List<Entities.File>? files { get; set; }
    }

    public class Category_StoreViewModel
    {
        public long id { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public double capital_value { get; set; }
        public double brand_value { get; set; }
        public double acreage { get; set; }

        public string? lat { get; set; }
        public string? lng { get; set; }
        public string? note { set; get; }
        public int type { set; get; } = 0; // 0 kho, 1 van phong , 2 cua hang
        public int status { set; get; } = 0; // 0 dang xay dung, 1 dang hoat dong, 2 dung hoat dong
        public DateTime? opening_day { set; get; }
        public string? status_name { set; get; }
        public long userAdded { set; get; }
        public long? userUpdated { set; get; }
        public Entities.File? file{ get; set; }
    }
}
