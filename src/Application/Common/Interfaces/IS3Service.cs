namespace CleanArchitecture.Application.Common.Interfaces;

public interface IS3Service
{
    Task UploadStream(MemoryStream stream, string destination, CancellationToken cancellationToken);
    Task<Stream> DownloadStream(string destination, CancellationToken cancellationToken);
}
