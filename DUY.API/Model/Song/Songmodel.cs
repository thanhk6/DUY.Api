using DUY.API.Entities;
using File = DUY.API.Entities.File;

namespace DUY.API.Model.Song
{
    public class Songmodel
    {
        public long id { set; get; }
        public string name { get; set; }
        public string description { get; set; }
        public string author { get; set; }
        public File? audio { get; set; }

    }
}
