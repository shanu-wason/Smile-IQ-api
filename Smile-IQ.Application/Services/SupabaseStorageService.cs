using Microsoft.Extensions.Configuration; 
using Supabase;
namespace Smile_IQ.Application.Services
{
    public class SupabaseStorageService
    {
        private readonly Client _client;

        public SupabaseStorageService(IConfiguration config)
        {
            _client = new Client(
                config["Supabase:Url"],
                config["Supabase:Key"]
            );

            _client.InitializeAsync().Wait();
        }
        public async Task<string> UploadAsync(byte[] fileBytes, string originalFileName)
        {
                var bucketName = "smile-image-bucket";
                var fileName = $"{Guid.NewGuid()}_{originalFileName}";

                // Upload the file to the bucket
                await _client.Storage.From(bucketName).Upload(fileBytes, fileName);

                // Return the public URL
                return _client.Storage.From(bucketName).GetPublicUrl(fileName);
        }
    }
}

