using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NAudio;
using Azure.Core.Extensions;
using Azure.Storage;
using System.IO;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using Microsoft.AspNetCore.Http;

namespace AudioContentApp.AudioConvertor
{
    public class AudioWaveConvertor : IAudioConvertor
    {
        private readonly IConfiguration config;
        
        public AudioWaveConvertor(IConfiguration _config )
        {
            config = _config;
            
        }

        public async Task<FileStream> ConvertAudioMp3(IFormFile fileToUpload)
        {
            string file_extension,
            filename_withExtension,
            storageAccount_connectionString;
            string _azureContinerName = config["AzureContinerName"];
            FileStream file;

            //Copy the storage account connection string from Azure portal     
            storageAccount_connectionString = config["BlobConnectionEndPoint"];

            // << reading the file as filestream from local machine >>    

            Stream stream = fileToUpload.OpenReadStream();
            string _wavFile = Path.GetTempFileName() + "_" + ".wav";

            using (Mp3FileReader mp3 = new Mp3FileReader(stream))
            {
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    WaveFileWriter.CreateWaveFile(_wavFile, pcm);
                }
            }

            file = new FileStream(_wavFile, FileMode.Open);

            CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(storageAccount_connectionString);
            CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_azureContinerName);

            //checking the container exists or not  
            if (container.CreateIfNotExists())
            {

              await  container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess =
                  BlobContainerPublicAccessType.Blob
                });

            }

            //reading file name & file extention    
            file_extension = Path.GetExtension(_wavFile);
            filename_withExtension = Path.GetFileName(_wavFile);

            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filename_withExtension);
            cloudBlockBlob.Properties.ContentType = file_extension;

            await cloudBlockBlob.UploadFromStreamAsync(file); // << Uploading the file to the blob >>  
            return file;
          

        }
    }
}
