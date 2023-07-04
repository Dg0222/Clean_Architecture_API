using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using MySql.Data.MySqlClient;
using Ionic.Zip;
using Ionic.Zlib;

namespace CleanArchitecture.Infrastructure.Jobs;

public class BackupDbJob : IJob
{
    private readonly IConfiguration _configuration;
    private readonly IS3Service _s3Service;
    private readonly ILogger<BackupDbJob> _logger;
    public BackupDbJob(IConfiguration configuration, IS3Service s3Service, ILogger<BackupDbJob> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _s3Service = s3Service ?? throw new ArgumentNullException(nameof(s3Service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var destination = $"MySQLBackups/CleanArchitectureBackup_{DateTime.Now:MM-dd-yyyy}.zip";

            using (var backupMemoryStream = new MemoryStream())
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ExportToMemoryStream(backupMemoryStream);
                            await conn.CloseAsync();
                        }
                    }
                }

                _logger.LogInformation("MySql backup was created successfully!");

                using (var zipMemoryStream = new MemoryStream())
                {
                    using (var zip = new ZipFile())
                    {
                        zip.CompressionLevel = CompressionLevel.BestCompression;

                        backupMemoryStream.Position = 0;

                        zip.AddEntry($"HallDashBackup_{DateTime.Now:MM-dd-yyyy}.sql", backupMemoryStream);

                        zip.Save(zipMemoryStream);
                    }
                    _logger.LogInformation("Zip file has been created successfully!");

                    await _s3Service.UploadStream(zipMemoryStream, destination, context.CancellationToken);
                }
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"DbBackupJob Error: {ex.InnerException}");
            throw;
        }

    }
}

