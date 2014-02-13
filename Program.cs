using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Security.Cryptography;


namespace CreateSAS
{
    class Program
    {
        static void Main(string[] args)
        {

            //////Console.WriteLine(GetFileContentMD5(@"C:\Program Files (x86)\Microsoft SDKs\Windows Azure\AzCopy\MikeLong\d3plot06"));
            //Console.WriteLine(GetFileMD5Hash(@"C:\Program Files (x86)\Microsoft SDKs\Windows Azure\AzCopy\MikeLong\d3plot06"));

            //Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            
            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference("sascontainer");
            //container.CreateIfNotExists();

            Console.WriteLine("Container SAS URI: " + GetContainerSasUri(container));
            Console.WriteLine();

            //Require user input before closing the console window.
            Console.ReadLine();

        }

        static string GetContainerSasUri(CloudBlobContainer container)
        {
            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(4);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List;

            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return container.Uri + sasContainerToken;
        }


        public static string GetFileContentMD5(string filename)
        {
            using (FileStream fs = File.Open(filename, FileMode.Open))
            {
                MD5 md5 = MD5.Create();
                byte[] md5Hash = md5.ComputeHash(fs);
                return Convert.ToBase64String(md5Hash);
            }
        }

        // File MD5
        public static string GetFileMD5Hash(string filename)
        {

            using (FileStream fs = File.Open(filename, FileMode.Open))
            {
                MD5 md5 = MD5.Create();
                byte[] md5Hash = md5.ComputeHash(fs);


                StringBuilder sb = new StringBuilder();
                foreach (byte b in md5Hash)
                {
                    sb.Append(b.ToString("x2").ToLower());
                }

                return sb.ToString();

            }

        }


    }
}
