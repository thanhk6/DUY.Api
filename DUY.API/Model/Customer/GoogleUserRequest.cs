using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DUY.API.Model.Customer
{
    public class GoogleUserRequest
    {
        public string IdToken { get; set; }
    }
}
