using System.Collections.Generic;
using System.Threading.Tasks;
using LStorage.Models.Auth;
using LStorage.Models.Dto;

namespace LStorageClient.Views;

public interface IUserWindowApi
{
    Task<string> GetFilePath();
    Task<string> GetFolderPath(string? fileName);
}