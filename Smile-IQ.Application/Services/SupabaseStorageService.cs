using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Supabase;
using System;
namespace Smile_IQ.Application.Services
{
    public class SupabaseStorageService
    {
        private readonly Client _client;
        private readonly ILogger<SupabaseStorageService> _logger;
        public SupabaseStorageService(IConfiguration config, ILogger<SupabaseStorageService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var url = config["Supabase:Url"];
            var key = config["Supabase:Key"];
            // (Optional) log url/key prefix here using _logger
            _client = new Client(url, key);
            _client.InitializeAsync().Wait();
        }

        //public async Task<string> UploadAsync(byte[] fileBytes, string originalFileName)
        //{
        //        var bucketName = "smile-image-bucket";
        //        var fileName = $"{Guid.NewGuid()}_{originalFileName}";

        //        // Upload the file to the bucket
        //        await _client.Storage.From(bucketName).Upload(fileBytes, fileName);

        //        // Return the public URL
        //        return _client.Storage.From(bucketName).GetPublicUrl(fileName);
        //}

        public async Task<string> UploadAsync(byte[] fileBytes, string originalFileName)
        {
            _logger.LogInformation($"client info from upload image: {_client}");
            var bucket = "smile-image-bucket";
            var fileName = $"{Guid.NewGuid()}_{originalFileName}";
            await _client.Storage.From(bucket).Upload(fileBytes, fileName);
            return _client.Storage.From(bucket).GetPublicUrl(fileName);
        }
    }
}

