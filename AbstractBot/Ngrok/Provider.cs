using System.Threading.Tasks;

namespace AbstractBot.Ngrok;

internal static class Provider
{
    public static Task<ListTunnelsResult?> ListTunnels()
    {
        return RestHelper.CallGetMethodAsync<ListTunnelsResult>(ApiProvider, Method);
    }

    private const string ApiProvider = "http://127.0.0.1:4040/api";
    private const string Method = "/tunnels";
}