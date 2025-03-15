using System.Security.Cryptography;
using System.Text;
using TestTask.Entities;

namespace TestTask.Services;

public class FormService
{
    private readonly AzureBlobService _azureBlobService;

    public FormService(AzureBlobService azureBlobService)
    {
        _azureBlobService = azureBlobService;
    }

    public async Task<string> SaveForm(UploadFormDto formDto)
    {
        await using var stream = formDto.Avatar.OpenReadStream();
        var newFileName = $"{Guid.NewGuid().ToString()}.{formDto.Avatar.FileName.Split('.').Last()}";
        var avatarUrl = await _azureBlobService.UploadImageAsync(stream, newFileName);

        var mappedSaveForm = new SaveForm
        {
            FirstName = formDto.FirstName,
            LastName = formDto.LastName,
            Email = formDto.Email,
            AvatarUrl = avatarUrl,
            Gender = formDto.Gender,
            Password = ComputeSha256Hash(formDto.Password),
        };

        var resultSaveForm = await _azureBlobService.UploadFormAsync(mappedSaveForm);

        return resultSaveForm;
    }

    private string ComputeSha256Hash(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}