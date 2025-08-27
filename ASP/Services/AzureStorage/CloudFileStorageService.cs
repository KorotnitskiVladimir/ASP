using Azure.Storage.Queues;

namespace ASP.Services.AzureStorage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class CloudFileStorageService : ICloudStorageService
{
    private const string directoryName = "C:\\storage\\";
    private const string connectionString = "";

    private const string containerName = "original-images";
    private const string queueName = "images";
    private const string smallImagesContainer = "small-images";

    private BlobServiceClient blobService;
    private BlobContainerClient blobContainer;
    private BlobContainerClient smallImagesBlobContainer;

    public void ConnectCloud()
    {
        blobService = new BlobServiceClient(connectionString);
        blobContainer = blobService.GetBlobContainerClient(containerName);
        blobContainer.CreateIfNotExists();
        blobContainer.SetAccessPolicy(PublicAccessType.BlobContainer);
        smallImagesBlobContainer = blobService.GetBlobContainerClient(smallImagesContainer);
        smallImagesBlobContainer.CreateIfNotExists();
        smallImagesBlobContainer.SetAccessPolicy(PublicAccessType.BlobContainer);
    }
    
    public string GetPath(string name)
    {
        ConnectCloud();
        BlobClient blob = blobContainer.GetBlobClient(name);
        return blob.Uri.AbsoluteUri + name;
    }
    
    public string SaveFile(IFormFile formFile)
    {
        ConnectCloud();
        string savedName = formFile.FileName;
        string fullName = directoryName + savedName;
        BlobClient createdBlob = blobContainer.GetBlobClient(savedName);
        QueueServiceClient queueService = new QueueServiceClient(connectionString);
        QueueClient queueClient = queueService.GetQueueClient(queueName);
        queueClient.CreateIfNotExists();
        queueClient.SendMessage(savedName);
        using (FileStream fs = File.OpenRead(fullName))
        {
            createdBlob.Upload(fs);
        }

        BlobClient smallBlob = smallImagesBlobContainer.GetBlobClient(savedName);
        return smallBlob.Uri.AbsoluteUri;
    }
}