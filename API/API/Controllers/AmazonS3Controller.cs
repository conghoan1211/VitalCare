using Amazon.S3;
using Amazon.S3.Model;
using API.Common;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmazonS3Controller : ControllerBase
    {
        private readonly IAmazonS3Service _s3Service;
        private readonly IAmazonS3 _s3Client;
        private const string BucketName = "photo-vitalcare";

        public AmazonS3Controller(IAmazonS3Service s3Service, IAmazonS3 s3Client)
        {
            _s3Service = s3Service;
            _s3Client = s3Client;
        }

        // Upload file to S3
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            var fileKey = $"uploads/{file.FileName}";
            var fileUrl = await _s3Service.UploadFileAsync(fileKey, file);

            if (fileUrl == null)
                return StatusCode(500, "Failed to upload file.");

            return Ok(new { FileUrl = fileUrl });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile(string filekey)
        {
            if (string.IsNullOrEmpty(filekey))
                return BadRequest("File key is required.");

            var msg = await _s3Service.DeleteFileAsync(filekey);

            return Ok(msg);  
        }

        [HttpGet("get")]
        public async Task<IActionResult> DoesFileExistAsync(string filekey)
        {
            if (filekey.IsEmpty())
                return BadRequest("File is required.");

            bool obj = await _s3Service.DoesFileExistAsync(filekey);

            if (obj == false)
                return BadRequest($"File is not exit");

            return Ok(obj);
        }


        // Download file from S3
        [HttpGet("download")]
        public async Task<IActionResult> DownloadFileFromS3(string fileKey)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = BucketName,
                    Key = fileKey
                };

                using (var response = await _s3Client.GetObjectAsync(request))
                using (var stream = response.ResponseStream)
                {
                    // Lưu tệp xuống máy chủ hoặc trả về stream cho client
                    var filePath = Path.Combine("D:\\Dowload", fileKey);
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await stream.CopyToAsync(fileStream);
                        return Ok("File downloaded successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error downloading file: {ex.Message}");
            }
        }
    }
}