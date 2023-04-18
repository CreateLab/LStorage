using System.Security.Claims;
using LStorage.Dao;
using LStorage.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace LStorage.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class FileController : ControllerBase
{
    private readonly IFileRepository _fileRepository;

    public FileController(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }


    /*
     using (var client = new HttpClient())
   {
       using (var content = new MultipartFormDataContent())
       {
           // Create a file stream for the file to be uploaded
           using (var fileStream = new FileStream("path/to/source/file", FileMode.Open, FileAccess.Read))
           {
               // Create a stream content from the file stream
               var fileContent = new StreamContent(fileStream);
   
               // Create a form data content for the file content
               var formDataContent = new ByteArrayContent(await fileContent.ReadAsByteArrayAsync());
   
               // Add the form data content to the multipart form data
               content.Add(formDataContent, "file", "filename.ext");
   
               // Send the multipart form data as a POST request to the ASP.NET Core endpoint
               var response = await client.PostAsync("api/UploadFile", content);
   
               // Handle the response as needed
           }
       }
   }
     */


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string projectId, CancellationToken token)
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
        var isAdmin = role == "admin";
        await _fileRepository.StoreFile(file.FileName, file.OpenReadStream(), projectId, email, isAdmin, token);
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<IEnumerable<FileDto>> GetFiles(string projectId, CancellationToken token)
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
        var isAdmin = role == "admin";
        return await _fileRepository.GetFiles(projectId, email,isAdmin, token);
    }

    [HttpGet]
    [Authorize]
    public async Task<FileStreamResult> DownloadFile(string fileId, string projectId, CancellationToken token)
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
        var isAdmin = role == "admin";
        var ulongFileId = ulong.Parse(fileId);
        return await _fileRepository.DownloadFile(ulongFileId, projectId, email, isAdmin, token);
    }

    [HttpGet]
    [Authorize]
    public async Task DeleteFile(string fileName, string projectName, CancellationToken token)
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        await _fileRepository.DeleteFile(fileName, projectName, email, token);
    }


    /*[Host]
    [Authorize(Roles = "admin")]
    public Task DeleteFiles(string fileNames)
    {
       return _fileRepository.DeleteFiles(fileNames);
    }
    
    public Task<string> GetFileList()
    {
        return _fileRepository.GetFileList();
    }*/
}