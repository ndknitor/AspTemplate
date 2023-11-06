public interface IFileService
{
    Stream Get(string url);
    Task<string> Insert(IFormFile file, string url = null);
    Task<string> Update(IFormFile file, string url = null);
    Task<string> Upsert(IFormFile file, string url = null);
    Task<bool> Delete(string url);
}
public class ApplicationFileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    public ApplicationFileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }
    public async Task<bool> Delete(string fileName)
    {
        string savePath = Path.Combine(_environment.WebRootPath, fileName);
        if (File.Exists(savePath))
        {
            await Task.Run(() =>
            {
                File.Delete(savePath);
            });
            return true;
        }
        return false;
    }

    public Stream Get(string url)
    {
        string savePath = Path.Combine(_environment.WebRootPath, url);
        if (File.Exists(savePath))
        {
            return File.OpenRead(savePath);
        }
        return null;
    }

    public async Task<string> Insert(IFormFile file, string url = null)
    {
        url = url ?? file.FileName;
        string savePath = Path.Combine(_environment.WebRootPath, url);
        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        if (!File.Exists(savePath))
        {
            using (Stream sourcestream = file.OpenReadStream())
            {
                using (FileStream desStream = File.Create(savePath))
                {
                    await sourcestream.CopyToAsync(desStream);
                }
            }
            return url;
        }
        return null;
    }

    public async Task<string> Update(IFormFile file, string url)
    {
        url = url ?? file.FileName;
        string savePath = Path.Combine(_environment.WebRootPath, url);
        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        if (File.Exists(savePath))
        {
            using (Stream sourcestream = file.OpenReadStream())
            {
                using (FileStream desStream = File.Create(savePath))
                {
                    await sourcestream.CopyToAsync(desStream);
                }
            }
            return url;
        }
        return null;
    }

    public async Task<string> Upsert(IFormFile file, string url)
    {
        url = url ?? file.FileName;
        string savePath = Path.Combine(_environment.WebRootPath, url);
        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        using (Stream sourcestream = file.OpenReadStream())
        {
            using (FileStream desStream = File.Create(savePath))
            {
                await sourcestream.CopyToAsync(desStream);
            }
        }
        return url;
    }
}