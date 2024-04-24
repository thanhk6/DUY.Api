namespace C.Tracking.API.Extensions
{
    public class RequestBodyModel
    {
        public string to { get; set; }
        public string from { get; set; }
        public string message { get; set; }
        public string scheduled { get; set; }
        public string requestId { get; set; }=string.Empty;
        public int useUnicode { get; set; }
        
       public List<string> ext { get; set; }=new List<string>();
    
    }
}
