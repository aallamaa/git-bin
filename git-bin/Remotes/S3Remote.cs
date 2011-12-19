﻿using System.IO;
using System.Linq;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace GitBin.Remotes
{
    public class S3Remote : IRemote
    {
        private readonly IConfigurationProvider _configurationProvider;
        private AmazonS3 _client;

        public S3Remote(
            IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string[] ListFiles()
        {
            var client = GetClient();

            var listRequest = new ListObjectsRequest();
            listRequest.BucketName = _configurationProvider.S3Bucket;

            var listResponse = client.ListObjects(listRequest);

            var keys = listResponse.S3Objects.Select(o => o.Key);

            return keys.ToArray();
        }

        public void UploadFile(string fullPath, string key)
        {
            var client = GetClient();

            var putRequest = new PutObjectRequest();
            putRequest.BucketName = _configurationProvider.S3Bucket;
            putRequest.FilePath = fullPath;
            putRequest.Key = key;
                //putRequest.WithSubscriber()

            using (var putResponse = client.PutObject(putRequest))
            {
                
            }
            // TODO - error checking, progress reporting
        }

        public void DownloadFile(string fullPath, string key)
        {
            var client = GetClient();

            var getRequest = new GetObjectRequest();
            getRequest.BucketName = _configurationProvider.S3Bucket;
            getRequest.Key = key;

            using (var getResponse = client.GetObject(getRequest))
            {
                getResponse.WriteResponseStreamToFile(fullPath);
            }
            // TODO - error checking, progress reporting
        }

        private AmazonS3 GetClient()
        {
            if (_client == null)
            {
                _client = AWSClientFactory.CreateAmazonS3Client(
                    _configurationProvider.S3Key,
                    _configurationProvider.S3SecretKey);
            }

            return _client;
        }
    }
}