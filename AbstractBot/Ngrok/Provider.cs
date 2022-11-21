using System.Text.Json;
using System.Threading.Tasks;
using GryphonUtilities;

namespace AbstractBot.Ngrok;

internal static class Provider
{
    public static Task<ListTunnelsResult> ListTunnels()
    {
        return RestHelper.CallGetMethodAsync<ListTunnelsResult>(ApiProvider, Method, options: Options);
    }

    private static readonly JsonSerializerOptions Options = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true
    };

    private const string ApiProvider = "http://127.0.0.1:4040/api";
    private const string Method = "/tunnels";
}