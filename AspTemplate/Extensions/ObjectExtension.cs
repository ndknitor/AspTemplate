using Newtonsoft.Json;

public static class ObjectExtensions
{
    public static string ToJson(this object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}
