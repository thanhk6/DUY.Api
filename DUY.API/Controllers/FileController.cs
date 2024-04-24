using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace C.Tracking.API.Controllers
{

    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                if (form == null)
                {
                    return BadRequest();
                }
                var files = form.Files;
                var folderName = @"/data/duy_files";
                //var folderName = @"D:\Folder";
                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (files.Count() > 0)
                {
                    List<FileModel> newFiles = new List<FileModel>();
                    foreach (var file in files)
                    {
                        FileModel item = new();
                        try
                        {
                            item.name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').Trim(' ');

                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, $"lỗi chỗ này: {ex}");
                        }
                        var fileType = item.name.Split('.');
                        item.file_type = fileType[fileType.Count() - 1];
                        item.name_guid = Guid.NewGuid().ToString() + "." + item.file_type;
                        string fullPath = Path.Combine(pathToSave, item.name_guid);
                        item.path = Path.Combine(folderName, item.name_guid);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        newFiles.Add(item);
                    }

                    return Ok(new { newFiles });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpPost("download")]
        public IActionResult GetBlobDownload([FromBody] FileDowloadModel model)
        {

            var net = new System.Net.WebClient();
            var data = net.DownloadData(model.url_file);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = model.file_name;
            return File(content, contentType, fileName);
        }
        [AllowAnonymous]
        [HttpGet("view-audio")]
        public IActionResult ViewImage(string url_file)
        {
            var fileType = url_file.Split('/');
            string type = "";
            if (fileType[fileType.Count() - 1] != "mp3")
                type = "mp3/" + fileType[fileType.Count() - 1];
            var image = System.IO.File.OpenRead(url_file);
            return File(image, type);
        }
        [AllowAnonymous]
        [HttpGet("view-pdf")]
        public IActionResult ViewPDF(string url_file)
        {
            byte[] pdfBytes = System.IO.File.ReadAllBytes(url_file);
            MemoryStream ms = new MemoryStream(pdfBytes);
            return new FileStreamResult(ms, "application/pdf");
        }
    }
    public class FileDowloadModel
    {
        public string file_name { get; set; }
        public string url_file { set; get; }
    }
    public class FileModel
    {
        public string name_guid { set; get; } = string.Empty;
        public string name { set; get; } = string.Empty;
        public byte type { set; get; }
        public string path { set; get; } = string.Empty;
        public string file_type { set; get; } = string.Empty;

    }
}
