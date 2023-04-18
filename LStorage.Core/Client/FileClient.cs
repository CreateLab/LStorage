using Flurl.Http;
using LStorage.Models.Dto;
using Serilog;

namespace LStorage.Core.Client;

public class FileClient
{
    private string? _serverUrl;
    private const string getFilesUrl = "/file/getFiles";
    private const string uploadFileUrl = "/file/uploadFile";
    private const string downloadFileUrl = "/file/downloadFile";

    public FileClient(string? serverUrl)
    {
        _serverUrl = serverUrl;
    }

    public async Task<IEnumerable<FileDto>> GetFiles(string selectedProjectId)
    {
        try
        {
            var url = _serverUrl + getFilesUrl;
            var response = await url.WithOAuthBearerToken(Setting.Setting.Token)
                .SetQueryParam("projectId", selectedProjectId)
                .GetJsonAsync<IEnumerable<FileDto>>();
            return response;
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(GetFiles), selectedProjectId, e.Message);
        }

        return null;
    }

    public async Task UploadFile(string fileName, Stream openReadStream, string projectId)
    {
        try
        {
            var url = _serverUrl + uploadFileUrl;
            await url.WithOAuthBearerToken(Setting.Setting.Token).PostMultipartAsync(mp =>
            {
                mp.AddFile("file", openReadStream, fileName);
                mp.AddString("projectId", projectId);
            });
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(UploadFile), fileName, e.Message);
        }
    }

    public async Task DownloadFile(string fileId, string filePath, string selectedProjectId)
    {
        try
        {
            var url = _serverUrl + downloadFileUrl;

            var folder = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            await url.WithOAuthBearerToken(Setting.Setting.Token).SetQueryParam("fileId", fileId)
                .SetQueryParam("projectId", selectedProjectId)
                .DownloadFileAsync(folder, fileName);
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(DownloadFile), fileId, e.Message);
        }
    }
}