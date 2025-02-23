using System.Text.Json;
using System.Threading.Tasks;
using GryphonUtilities;

namespace AbstractBot.Utilities.Ngrok;

internal static class Provider
{
    public static Task<ListTunnelsResult> ListTunnels(JsonSerializerOptions options)
    {
        return RestManager.GetAsync<ListTunnelsResult>(ApiProvider, Method, options: options);
    }

    private const string ApiProvider = "http://127.0.0.1:4040/api";
    private const string Method = "/tunnels";
}