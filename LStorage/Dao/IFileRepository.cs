using LStorage.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LStorage.Dao;

public interface IFileRepository
{
    Task StoreFile(string fileName, Stream openReadStream, string projectId, string? email,
        bool isAdmin,
        CancellationToken cancellationToken);
    Task<IEnumerable<FileDto>> GetFiles(string projectId, string? email, bool isAdmin,
        CancellationToken cancellationToken);
    Task<FileStreamResult> DownloadFile(ulong fileId, string projectId, string? email, bool isAdmin,
        CancellationToken cancellationToken);
    Task DeleteFile(string fileId, string projectId, string? email, CancellationToken cancellationToken);
}