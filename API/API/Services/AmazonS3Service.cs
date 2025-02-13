using Amazon.S3.Model;
using Amazon.S3;
using API.Configurations;

namespace API.Services
{
    public interface IAmazonS3Service
    {
        public Task<string> UploadFileAsync(string key, IFormFile? file);
        public Task<List<string>> UploadFilesAsync(string key, List<IFormFile>? files);
        public Task<bool> DoesFileExistAsync(string fileKey);
        public Task<object> DeleteFileAsync(string fileKey);
        public Task<bool> DeleteFolderAsync(string folderKey);

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
        public async Task<string> UploadFileAsync(string key, IFormFile? file)
        {
            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = file.OpenReadStream(),
                    ContentType = file.ContentType,
                };

                var response = await _s3Client.PutObjectAsync(putRequest);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK
                    ? $"https://{_bucketName}.s3.amazonaws.com/{key}"
                    : "";
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while uploading to S3! " + ex.Message);
            }
        }

        public async Task<List<string>> UploadFilesAsync(string key, List<IFormFile>? files)
        {
            var uploadedUrls = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)  
                {
                    try
                    {
                        var url = await UploadFileAsync($"{key}/{file.FileName}", file);
                        if (!string.IsNullOrEmpty(url))
                        {
                            uploadedUrls.Add(url);   
                        }
                        else throw new Exception($"Failed to upload file: {file.FileName}");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"An error occurred while uploading file {file.FileName}: {ex.Message}");
                    }
                }
            }
            return uploadedUrls;
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

        public async Task<bool> DeleteFolderAsync(string folderKey)
        {
            try
            {
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = folderKey  
                };

                var listResponse = await _s3Client.ListObjectsV2Async(listRequest);
                if (listResponse.S3Objects.Count > 0)
                {
                    var deleteObjectsRequest = new DeleteObjectsRequest
                    {
                        BucketName = _bucketName,
                        Objects = listResponse.S3Objects
                            .Select(o => new KeyVersion { Key = o.Key })
                            .ToList()
                    };

                    var deleteResponse = await _s3Client.DeleteObjectsAsync(deleteObjectsRequest);
                    return deleteResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
                }
                return true; // Nothing to delete
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting folder: " + ex.Message);
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