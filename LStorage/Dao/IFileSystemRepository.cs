using Microsoft.AspNetCore.Mvc;

namespace LStorage.Dao;

public interface IFileSystemRepository
{
   public void UpdateRootPath();

   public Task<string> CreateProject(string projectName);

   int GetFreeSpace(CancellationToken cancellationToken);
   int GetOccupedSpace(CancellationToken cancellationToken);
   int GetTotalSpace(CancellationToken cancellationToken);
   Task<string> StoreFile(string fileName, Stream openReadStream, string projectPath,
      CancellationToken cancellationToken);
   Task DeleteFile(string filePath, CancellationToken cancellationToken);
   FileStreamResult DownloadFile(string filePath, CancellationToken cancellationToken);
}