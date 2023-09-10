using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;

namespace GeneratePreSignedURL
{
    class Program
    {
        private const string bucketName = "batchreportbucket";
        private const string objectKey = "homework.pdf";
        private const string filePath = "C:\\AshishS\\TEMPUPLOADTEST";
        // Specify how long the presigned URL lasts, in hours
        private const double timeoutDuration = 12;
        // Specify your bucket region (an example region is shown).
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
        private static IAmazonS3 s3Client;

        static void Main(string[] args)
        {
            s3Client = new AmazonS3Client(bucketRegion);
            var url = GeneratePreSignedURL(timeoutDuration);
            Console.WriteLine("You Pre-Signed URL is: " + url);
            Console.ReadLine(); 
            //UploadObject(url);
        }

        private static string GeneratePreSignedURL(double duration)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddHours(duration)
            };

            string url = s3Client.GetPreSignedURL(request);
            return url;
        }
    }
}
