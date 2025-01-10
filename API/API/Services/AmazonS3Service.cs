using Amazon.S3.Model;
using Amazon.S3;
using API.Configurations;
using Azure;
using static System.Net.WebRequestMethods;

namespace API.Services
{
    public interface IAmazonS3Service
    {
        public Task<string> UploadFileAsync(string key, Stream fileStream, string contentType);
        public Task<bool> DoesFileExistAsync(string fileKey);
        public Task<object> DeleteFileAsync(string fileKey);

    }

    public class AmazonS3Service : IAmazonS3Service
    {
        private readonly IAmazonS3 _s3Client;
        #region Config
        private readonly string _bucketName = ConfigManager.gI().AWSBucketName;
        private readonly string _AwsAssessKey = ConfigManager.gI().AWSAccessKey;
        private readonly string _AwsSercetKey = ConfigManager.gI().AWSSecretKey;
        private readonly string _AwsRegion = ConfigManager.gI().AWSRegion;
        #endregion

        public AmazonS3Service(IConfiguration configuration)
        {
            _s3Client = new AmazonS3Client(
                _AwsAssessKey, _AwsSercetKey,
                Amazon.RegionEndpoint.GetBySystemName(_AwsRegion)
            );
        }

        public async Task<string> UploadFileAsync(string key, Stream fileStream, string contentType)
        {
            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = fileStream,
                    ContentType = contentType,
                    //  CannedACL = S3CannedACL.PublicRead
                };

                var response = await _s3Client.PutObjectAsync(putRequest);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK
                    ? $"Success: https://{_bucketName}.s3.amazonaws.com/{key}"
                    : "Response error while upload file in S3service";
            }
            catch (Exception ex)
            {
                throw new Exception("Catched an error happened while upload file in S3Service!" + ex.Message);
            }
        }

        public async Task<object> DeleteFileAsync(string fileKey)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileKey
                };
                var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest);

                return response;
            }
            catch (AmazonS3Exception ex)
            {
                return $"Error deleting file from S3: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<bool> DoesFileExistAsync(string fileKey)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = fileKey
                };

                var response = await _s3Client.GetObjectMetadataAsync(request);
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                throw new Exception(ex.Message);
            }
        }
    }
}