using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SharedClientAcessUrls.Lib
{
    public class AzureBlobSA
    {
        public static IEnumerable<IListBlobItem> GetContainerFiles(string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageAccountConnectionString"));
            CloudBlobClient storageClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer storageContainer = storageClient.GetContainerReference(containerName);
            return storageContainer.ListBlobs();

        }
        public static string GetSasUrl(string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExist();

            BlobContainerPermissions containerPermissions = new BlobContainerPermissions();

            containerPermissions.SharedAccessPolicies.Add("twominutepolicy", new SharedAccessPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(2),
                Permissions = SharedAccessPermissions.Write | SharedAccessPermissions.Read
            });

            containerPermissions.PublicAccess = BlobContainerPublicAccessType.Off;
            container.SetPermissions(containerPermissions);
            string sas = container.GetSharedAccessSignature(new SharedAccessPolicy(), "twominutepolicy");
            return sas;
        }

        public static string GetSasBlobUrl(string containerName, string fileName, string sas)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);

            StorageCredentialsSharedAccessSignature sasCreds = new StorageCredentialsSharedAccessSignature(sas);
            CloudBlobClient sasBlobClient = new CloudBlobClient(storageAccount.BlobEndpoint,
            new StorageCredentialsSharedAccessSignature(sas));

            CloudBlob blob = sasBlobClient.GetBlobReference(containerName + @"/" + fileName);
            return blob.Uri.AbsoluteUri + sas;
        }

        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}