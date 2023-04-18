using LStorage.Dao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LStorage.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class InfoController : ControllerBase
{
    private IFileSystemRepository _fileSystemRepository;

    public InfoController(IFileSystemRepository fileSystemRepository)
    {
        _fileSystemRepository = fileSystemRepository;
    }

    [HttpGet]
    [Authorize]
    public int GetFreeSpace(CancellationToken token)
    {
        var result  = _fileSystemRepository.GetFreeSpace(token);
        return result;

    }

    [HttpGet]
    [Authorize]
    public int GetOccupedSpace(CancellationToken token)
    {
        return _fileSystemRepository.GetOccupedSpace(token);
       
    }
    
    [HttpGet]
    [Authorize]
    public int GetTotalSpace(CancellationToken token)
    {
        return _fileSystemRepository.GetTotalSpace(token);
    }
}