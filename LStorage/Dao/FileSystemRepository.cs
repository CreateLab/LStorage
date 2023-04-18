using Microsoft.AspNetCore.Mvc;

namespace LStorage.Dao;

public class FileSystemRepository : IFileSystemRepository
{
    private const string _defaultSettingWithRootPath = "./Root.txt";
    private const string bin = "bin";
    private string _rootPath;
    private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

    public FileSystemRepository()
    {
        _rootPath = File.ReadAllText(_defaultSettingWithRootPath);
    }

    public void UpdateRootPath()
    {
        try
        {
            _semaphoreSlim.Wait();
            _rootPath = File.ReadAllText(_defaultSettingWithRootPath);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task<string> CreateProject(string projectName)
    {
        try
        {
            await _semaphoreSlim.WaitAsync();
            var path = Path.Combine(_rootPath, projectName);
            if (Directory.Exists(path))
                throw new Exception("Project already exists");
            Directory.CreateDirectory(path);
            var binPath = Path.Combine(path, bin);
            Directory.CreateDirectory(binPath);
            return path;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public int GetFreeSpace(CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(_rootPath);
        var currentDrive = Path.GetPathRoot(fullPath);

        var drives = DriveInfo.GetDrives();

        return (from drive in drives
            where drive.Name == currentDrive
            select (int)(drive.TotalFreeSpace / 1024 / 1024 / 1024)).FirstOrDefault();
    }

    public int GetOccupedSpace(CancellationToken cancellationToken)
    {
       var fullPath = Path.GetFullPath(_rootPath);
              var currentDrive = Path.GetPathRoot(fullPath);

        var drives = DriveInfo.GetDrives();

        return (from drive in drives
            where drive.Name == currentDrive
            select (int)((drive.TotalSize - drive.AvailableFreeSpace) / 1024 / 1024 / 1024)).FirstOrDefault();
    }

    public int GetTotalSpace(CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(_rootPath);
        var currentDrive = Path.GetPathRoot(fullPath);


        var drives = DriveInfo.GetDrives();

        return (from drive in drives
            where drive.Name == currentDrive
            select (int)(drive.TotalSize / 1024 / 1024 / 1024)).FirstOrDefault();
    }

    public async Task<string> StoreFile(string fileName, Stream openReadStream, string projectPath,
        CancellationToken cancellationToken)
    {
        try
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);
            // Set the buffer size to 4MB (adjust as necessary)
            var bufferSize = 4 * 1024 * 1024;

            // Create a buffer to hold the file data
            var buffer = new byte[bufferSize];
            var fn = Path.Combine(projectPath, fileName);
            if (File.Exists(fn))
            {
                var newFileName = Path.GetFileNameWithoutExtension(fn);
                var newFileExtension = Path.GetExtension(fn);
                var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                fn = Path.Combine(projectPath, $"{newFileName}-{date}{newFileExtension}");
            }
            await using FileStream fileStream =
                new FileStream(fn, FileMode.Create, FileAccess.Write);
            // Read the file data in chunks and write them to the file stream
            int bytesRead;
            while ((bytesRead = await openReadStream.ReadAsync(buffer, 0, bufferSize,cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead,cancellationToken);
            }

            return fn;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public Task DeleteFile(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            _semaphoreSlim.Wait(cancellationToken);
            var path = Path.GetDirectoryName(filePath);
            var binPath = Path.Combine(path, bin);
            if (Directory.Exists(binPath))
            {
                var newFilePath = Path.Combine(binPath, Path.GetFileName(filePath));
                if (File.Exists(newFilePath))
                {
                    var newFileName = Path.GetFileNameWithoutExtension(newFilePath);
                    var newFileExtension = Path.GetExtension(newFilePath);
                    var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                    newFilePath = Path.Combine(binPath, $"{newFileName}-{date}{newFileExtension}");
                }
                File.Move(filePath, newFilePath);
            }
            return Task.CompletedTask;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public FileStreamResult DownloadFile(string filePath, CancellationToken cancellationToken)
    {
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var result = new FileStreamResult(stream, "application/octet-stream")
        {
            FileDownloadName = Path.GetFileName(filePath),
            EnableRangeProcessing = true,
        };

        return result;
    }
}