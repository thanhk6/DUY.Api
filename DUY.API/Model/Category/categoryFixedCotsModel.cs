using System.ComponentModel.DataAnnotations;

namespace C.Tracking.API.Model.Category
{
    public class categoryFixedCotsModel
    {
        public long id { set; get; }
        public string name { get; set; }
        public string code { get; set; }
        public decimal price { get; set; }
        public string note { get; set; }
    }
}
