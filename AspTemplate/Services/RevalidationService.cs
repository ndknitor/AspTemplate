public interface IRevalidationService
{
    Task<bool> WithTag(string tag);
    Task<bool> WithTags(IEnumerable<string> tags);
    Task<bool> WithPath(string path);
    Task<bool> WithPaths(IEnumerable<string> paths);
}
public class RevalidationService : IRevalidationService
{
    public readonly HttpClient client;
    public RevalidationService(string url, string token)
    {
        client = new HttpClient
        {
            BaseAddress = new Uri(url),
        };
        client.DefaultRequestHeaders.Add("Revalidate-Token", token);
    }
    ~RevalidationService()
    {
        client.Dispose();
    }
    public async Task<bool> WithPath(string path)
    {
        HttpResponseMessage message = await client.PostAsJsonAsync("/api/revalidate/paths", new string[] { path });
        ResponseMessage response = await message.Content.ReadAsAsync<ResponseMessage>();
        return response.Revalidated;
    }

    public async Task<bool> WithPaths(IEnumerable<string> paths)
    {
        HttpResponseMessage message = await client.PostAsJsonAsync("/api/revalidate/paths", paths);
        ResponseMessage response = await message.Content.ReadAsAsync<ResponseMessage>();
        return response.Revalidated;
    }

    public async Task<bool> WithTag(string tag)
    {
        HttpResponseMessage message = await client.PostAsJsonAsync("/api/revalidate/tags", new string[] { tag });
        ResponseMessage response = await message.Content.ReadAsAsync<ResponseMessage>();
        return response.Revalidated;
    }

    public async Task<bool> WithTags(IEnumerable<string> tags)
    {
        HttpResponseMessage message = await client.PostAsJsonAsync("/api/revalidate/tags", tags);
        ResponseMessage response = await message.Content.ReadAsAsync<ResponseMessage>();
        return response.Revalidated;
    }

    internal class ResponseMessage
    {
        public string Message { get; set; }
        public bool Revalidated { get; set; }
    }
}