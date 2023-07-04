using Amazon.S3;
using Amazon.S3.Transfer;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Infrastructure.Services;

public sealed class S3Service : IS3Service
{
    private readonly S3Settings _s3Settings;
    private readonly IAmazonS3 _client;
    private readonly ILogger<S3Service> _logger;

    public S3Service(IAmazonS3 client, IOptions<S3Settings> s3Settings, ILogger<S3Service> logger)
    {
        _s3Settings = s3Settings.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task UploadStream(MemoryStream stream, string destination, CancellationToken cancellationToken)
    {
        try
        {
            using var transferUtility = new TransferUtility(_client);
            await transferUtility.UploadAsync(stream, $"{_s3Settings.BucketName}", $"{destination}", cancellationToken);
            _logger.LogInformation("File was uploaded to s3 successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError("AWS S3 upload error: {ExInnerException}", ex.InnerException);
            throw;
        }
    }

    public async Task<Stream> DownloadStream(string destination, CancellationToken cancellationToken)
    {
        try
        {
            using var transferUtility = new TransferUtility(_client);
            return await transferUtility.OpenStreamAsync($"{_s3Settings.BucketName}", $"{destination}", cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError("AWS S3 download error: {ExInnerException}", ex.InnerException);
            throw;
        }
    }
}
