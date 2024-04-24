namespace C.Tracking.API.Model.User
{
    public class ClaimModel
    {
        public long id { get; set; }
        public string username { get; set; } = string.Empty;


        public string email { get; set; } = string.Empty;

        public string full_name { get; set; } = string.Empty;


        public byte type { set; get; }
      public  List<string> roles { get; set; }
    }
}
