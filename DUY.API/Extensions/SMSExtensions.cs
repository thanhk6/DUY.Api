using C.Tracking.API.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace C.Tracking.API.Extensions
{
    public class SMSExtensions
    {
        const string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c24iOiJzbWFydGdhcCIsInNpZCI6IjRkYWExYWM3LTBmNDYt" +
            "NGQ4Mi05MjEzLTEwYmFkNDExNjBkNCIsIm9idCI6IiIsIm9iaiI6IiIsIm5iZiI6MTY1OTc2NzA2MCwiZXhwIjoxNjU5NzcwN" +
            "jYwLCJpYXQiOjE2NTk3NjcwNjB9.dWcrt_-eHKmqSfSqSL8_vnq10qRyq1jZhyF-fsDys2Q";
        const string resourcePath = "/SMSBrandname/SendSMS";
        public async Task<string> SendOTPRegister(string phone_number, string otp)
        {
            string sms_Url = "http://api.brandsms.vn/api";
            OTP_BodyModel bodyModel = new OTP_BodyModel();
            //bodyModel.message = "Ma xac nhan dang ky he thong Smartgap Contract cua ban la " + otp + " co hieu luc trong vong 5phut. Vui long khong chia se voi bat ky ai";
            bodyModel.message = "[Smartgap.vn] Ma xac nhan dang ky he thong https://smartgapcontract.vn/ cua ban la " + otp + " co hieu luc trong vong 5phut. Vui long khong chia se voi bat ky ai.";
            bodyModel.to = phone_number;
            try
            {  // Define a resource path
                string resourcePath = "/SMSBrandname/SendSMS";
                var body = JsonConvert.SerializeObject(bodyModel);

                // Define a client
                var client = new RestClient(sms_Url);

                // Define a request
                var request = new RestRequest(resourcePath, Method.Post);
                // Add headers
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/json");
                request.AddHeader("authorization", "Bearer " + token);
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                var response = client.Execute(request);
                string content = response.Content ?? "";
                var data = JsonConvert.DeserializeObject<responseModel>(content);
                if (data.errorCode == "000")
                    return bodyModel.message;
                else
                    return "0";
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return message;
            }

        }
        public async Task<string> SendOTPLogin(string phone_number, string otp)
        {
            string sms_Url = "http://api.brandsms.vn/api";
            OTP_BodyModel bodyModel = new OTP_BodyModel();
            //bodyModel.message = " Ma dang nhap he thong Smartgap Contract cua ban la " + otp + " co hieu luc trong vong 3p.Vui long khong chia se voi bat ky ai.";
            bodyModel.message = "Smartgap.vn] Ma dang nhap he thong https://smartgapcontract.vn/ cua ban la " + otp + " co hieu luc trong vong 3p. Vui long khong chia se voi bat ky ai. ";
            bodyModel.to = phone_number;
            try
            {  // Define a resource path
                var body = JsonConvert.SerializeObject(bodyModel);

                // Define a client
                var client = new RestClient(sms_Url);

                // Define a request
                var request = new RestRequest(resourcePath, Method.Post);
                // Add headers
                request.AddHeader("Content-Type", "application/json");

                request.AddHeader("Accept", "application/json");
                // request.AddHeader("token",  token);
                request.AddHeader("authorization", "Bearer " + token);
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                var response = client.Execute(request);
                string content = response.Content ?? "";
                var data = JsonConvert.DeserializeObject<responseModel>(content);
                if (data.errorCode == "000")
                    return bodyModel.message;
                else
                    return "0";
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return message;
            }
        }
        public async Task<string> SendOTPContract(string phone_number, string otp,string contract_code)
        {
            string sms_Url = "http://api.brandsms.vn/api";
            OTP_BodyModel bodyModel = new OTP_BodyModel();
            //bodyModel.message = "Ma xac nhan hop dong he thong Smartgap Contract cua ban la " + otp + " co hieu luc trong vong 5phut. Vui long khong chia se voi bat ky ai.";
            bodyModel.message = "[Smartgap.vn] Ma xac nhan hop dong ma so "+ contract_code+" tren https://smartgapcontract.vn/ cua ban la "+ otp +" co hieu luc trong vong 5phut. Vui long khong chia se voi bat ky ai.";
            //bodyModel.message = "Ma xac nhan hop dong so " + SOHOPDONG +" tren he thong Smartgap Contract cua ban la " + otp + " co hieu luc trong vong 5phut.Vui long khong chia se voi bat ky ai.";
            bodyModel.to = phone_number;
            try
            {  // Define a resource path
                var body = JsonConvert.SerializeObject(bodyModel);
                // Define a client
                var client = new RestClient(sms_Url);
                // Define a request
                var request = new RestRequest(resourcePath, Method.Post);
                // Add headers
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/json");
                // request.AddHeader("token",  token);
                request.AddHeader("authorization", "Bearer " + token);
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                var response = client.Execute(request);
                string content = response.Content ?? "";
                var data = JsonConvert.DeserializeObject<responseModel>(content);
                if (data.errorCode == "000")
                    return bodyModel.message;
                else
                    return "0";
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return message;
            }

        }
    }
}
