namespace WebScraping.Services;
public static class AddressHelper
{
    const string SCHEME = "https://";
    public static string GetBaseAddres(string address)
    {
        string clearScheme = address.Substring(SCHEME.Length, address.Length - SCHEME.Length);
        int firstSlash = clearScheme.IndexOf("/");
        var host = clearScheme.Substring(0, firstSlash);
        string Baseaddress = $"{SCHEME}{host}";
        return Baseaddress;
    }
    public static string GetApi(string address)
    {
        int baseaddressLength = GetBaseAddres(address).Length;
        return address.Substring(baseaddressLength, address.Length - baseaddressLength);
    }

    public static string Normilizeaddress(string address)
    {
        var baseaddress = GetBaseAddres(address);
        var api = GetApi(address);
        var cleadDoubleslash = api.Replace("//", "/") ?? api;
        return $"{baseaddress}{cleadDoubleslash}";
    }

    public static string GetUri(string address)
    {
        if (string.IsNullOrWhiteSpace(address)) return address;
        return address.StartsWith(SCHEME) ? address.Substring(SCHEME.Length, address.Length - SCHEME.Length) : address;
    }
    public static string MargeHostAndApi(string host, string api)
    {
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(api))
            return string.Empty;
        else
            return Normilizeaddress($"{host}{api}");
    }
}
