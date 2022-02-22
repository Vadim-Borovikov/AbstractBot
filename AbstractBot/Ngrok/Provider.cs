using System.Threading.Tasks;
using GryphonUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AbstractBot.Ngrok;

internal static class Provider
{
    public static Task<ListTunnelsResult> ListTunnels()
    {
        return RestHelper.CallGetMethodAsync<ListTunnelsResult>(ApiProvider, Method, settings: Settings);
    }

    private static readonly JsonSerializerSettings Settings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };

    private const string ApiProvider = "http://127.0.0.1:4040/api";
    private const string Method = "/tunnels";
}