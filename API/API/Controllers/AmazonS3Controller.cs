using Amazon.S3;
using Amazon.S3.Model;
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
            var fileUrl = await _s3Service.UploadFileAsync(fileKey, file.OpenReadStream(), file.ContentType);

            if (fileUrl == null)
                return StatusCode(500, "Failed to upload file.");

            return Ok(new { FileUrl = fileUrl });
        }

        // Download file from S3
        [HttpGet("download")]
        public async Task DownloadFileFromS3(string fileKey)
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
                        Console.WriteLine("File downloaded successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }
        }
    }
}