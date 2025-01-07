using Amazon.S3.Model;
using Amazon.S3;
using API.Configurations;

namespace API.Services
{
    public interface IAmazonS3Service
    {
        public Task<string> UploadFileAsync(string key, Stream fileStream, string contentType);
        public Task<Stream> GetFileAsync(string key);
    }

    public class AmazonS3Service : IAmazonS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = ConfigManager.gI().AWSBucketName;
        private readonly string _AwsAssessKey = ConfigManager.gI().AWSAccessKey;
        private readonly string _AwsSercetKey = ConfigManager.gI().AWSSecretKey;
        private readonly string _AwsRegion = ConfigManager.gI().AWSRegion; // Lấy Region từ cấu hình

        public AmazonS3Service(IConfiguration configuration)
        {
            _s3Client = new AmazonS3Client(
                _AwsAssessKey, _AwsSercetKey,
                Amazon.RegionEndpoint.GetBySystemName(_AwsRegion)
            );
        }

        // Upload file to S3
        public async Task<string> UploadFileAsync(string key, Stream fileStream, string contentType)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            var response = await _s3Client.PutObjectAsync(putRequest);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK
                ? $"https://{_bucketName}.s3.amazonaws.com/{key}"
                : null;
        }

        // Get file from S3
        public async Task<Stream> GetFileAsync(string key)
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(getRequest);
            return response.ResponseStream;
        }
    }
}