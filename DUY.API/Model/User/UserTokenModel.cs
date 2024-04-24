using DUY.API.Entities;

namespace DUY.API.Model.User
{
    public class UserTokenModel
    {
        public long id { get; set; }
        public string username { get; set; }
        public string token { get; set; }
        public string full_name { get; set; }
        public List<string> roles { get; set; }
        public Entities.File? image { get; set; }

    }

    public class LoginModel
    {
        public string username { set; get; }
        public string password { set; get; }
    }

    public class ChangePassModel
    {
        public long id { set; get; }
        public string passwordOld { set; get; }
        public string passwordNew { set; get; }
    }
}
