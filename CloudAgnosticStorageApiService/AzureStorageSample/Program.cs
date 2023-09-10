
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //var fileToStore = @"C:\AshishS\TEMPUPLOADTEST\AZURETEST\homework.pdf";

            Console.WriteLine("Uploading a File from memory");

            //var filename = GenerateblobFileName("homework.pdf");
            var filename = "homework.pdf";
            string containerName = "batchtestcontainer";
            GetSASUrlForBlob(containerName, filename);

            //Console.WriteLine(AsyncUploadFile(containerName, GetFileStream(), filename, "application/pdf").Result);
            Console.Read();

        }

        private static void GetSASUrlForBlob(string containerName, string filename)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=ashishsbatchreport;AccountKey=jwtqx9APttoEoprw02VuSt4jm54+GNghSlG6O+xZq0wQ2nrzaoNWxk880sNL6VwQ5yJGpzolMVH/tvRhUwk3Fw==;EndpointSuffix=core.windows.net";

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(filename);
            var uri = GetServiceSasUriForBlob(blobClient);
        }

        #region GetFileStream : Get Byte Array for the pdf file
        public static byte[] GetFileStream()
        {
            //var basepath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            var filepath = @"C:\AshishS\TEMPUPLOADTEST\AZURETEST\homework.pdf";
            byte[] fileData = File.ReadAllBytes(filepath);
            return fileData;
        }
        #endregion 

        #region AsyncUploadFile: Method to upload a pdf in Azure blob 
        public static async Task<string> AsyncUploadFile(string containerName, byte[] arr, string filename, string filetype)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=ashishsbatchreport;AccountKey=jwtqx9APttoEoprw02VuSt4jm54+GNghSlG6O+xZq0wQ2nrzaoNWxk880sNL6VwQ5yJGpzolMVH/tvRhUwk3Fw==;EndpointSuffix=core.windows.net";           

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // container name which we created
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(filename);

            var blobHttpHeader = new BlobHttpHeaders();

            blobHttpHeader.ContentType = filetype;

            using (MemoryStream ms = new MemoryStream(arr))
            {
                await blobClient.UploadAsync(ms, blobHttpHeader);
            }
            return blobClient.Uri.AbsoluteUri;
        }
        #endregion

        private static Uri GetServiceSasUriForBlob(BlobClient blobClient, string storedPolicyName = null)
        {
            // Check whether this BlobClient object has been authorized with Shared Key.
            if (blobClient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read |
                        BlobSasPermissions.Write);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                Console.WriteLine("SAS URI for blob is: {0}", sasUri);
                Console.WriteLine();

                return sasUri;
            }
            else
            {
                Console.WriteLine(@"BlobClient must be authorized with Shared Key 
                          credentials to create a service SAS.");
                return null;
            }
        }



    }
}
