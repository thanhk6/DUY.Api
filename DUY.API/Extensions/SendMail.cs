using System.Net.Mail;

namespace C.Tracking.API.Extensions
{
    public class SendMail
    { public static void SendMailForgotPassword(string emailFrom, string subject, string passNew)
        {
            int port = 587;
            string emailSend = "visaotoikhoc2708@gmail.com";
            string passEmailSend = "phannghia1995";
            //string emailTo = "anhvt@tima.vn,";
          //  emailFrom = "thevisao1295@gmail.com";
            string content = "bạn có yêu cầu thay đổi mật khẩu, mật khẩu mới của bạn là : "+ passNew;

          //  string emailCC = "trungnq@tima.vn";
            try
            {
                string smtp = "smtp.gmail.com";
                var mail = new MailMessage();
                var smtpServer = new SmtpClient(smtp);
                mail.From = new MailAddress(emailSend, "Thông báo mật khẩu mới của bạn");
                mail.To.Add(emailFrom);
               // if (!string.IsNullOrEmpty(emailCC))
                //{
                 //   mail.CC.Add(emailCC);
               // }
                mail.Subject = subject;
                mail.Body = content;
                mail.IsBodyHtml = true;
                smtpServer.Port = port;
                smtpServer.Credentials = new System.Net.NetworkCredential(emailSend, passEmailSend);
                smtpServer.EnableSsl = true;
                smtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine("Có lỗi xảy ra khi gửi mail:" + e.Message);
            }
        }
    }
}
