

using DUY.API.Entities;
using DUY.API.Model.Contract;
using File = DUY.API.Entities.File;

namespace DUY.API.Model.Song
{
    public class SongComment
    {
        public long id { set; get; }
        public string name { get; set; }
        public string description { get; set; }

        public File? audio { get; set; }
        public List<ComMentModel>? comments { get; set;}
    }
}
