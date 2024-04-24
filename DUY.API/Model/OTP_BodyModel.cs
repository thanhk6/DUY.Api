namespace C.Tracking.API.Model
{
    public class OTP_BodyModel
    {
        public string to { get; set; }
        public string from { get; set; } = "Smartgap.vn";
        public string message { get; set; }
        public string scheduled { get; set; } = "";
        public string telco { get; set; } = "";
        public string requestId { get; set; } = "";
        public string packageCode { get; set; } = "";
        public int Unicode { get; set; } = 0;
        public int type { get; set; } = 1;


    }
    public class responseModel
    {
        public OTP_BodyModel sendMessage { get; set; }
        public int msgLength { set; get; }
        public int mtCount { set; get; }
        public string? account { set; get; }
        public string? errorCode { set; get; }
        public string? errorMessage { set; get; }
        public string? referentId { set; get; }
    }
    public class AppsetingUrl
    {
        public string PosUrl { set; get; }
        public string CategoryUrl { set; get; }
    }
}
