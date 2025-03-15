using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using TestTask.Entities;

namespace TestTask.Services;

public class AzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _imageContainerName;
    private readonly string _jsonContainerName;

    public AzureBlobService(IConfiguration configuration)
    {
        string connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _imageContainerName = configuration["AzureBlobStorage:ImageContainerName"];
        _jsonContainerName = configuration["AzureBlobStorage:JsonContainerName"];
        
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_imageContainerName);
        await container.CreateIfNotExistsAsync();

        var blobClient = container.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, overwrite: true);

        return blobClient.Uri.ToString();
    }

    
    public async Task<string> UploadFormAsync(SaveForm form)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_jsonContainerName);
        await container.CreateIfNotExistsAsync();
        
        var blobClient = container.GetBlobClient(Guid.NewGuid().ToString() + ".json");


        string jsonString = JsonSerializer.Serialize(form, new JsonSerializerOptions { WriteIndented = true });
        byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
        using var stream = new MemoryStream(byteArray);

        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString(); 
    }
}