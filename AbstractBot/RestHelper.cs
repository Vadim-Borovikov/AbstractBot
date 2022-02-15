using System.Threading.Tasks;
using GoogleSheetsManager;
using RestSharp;

namespace AbstractBot;

internal static class RestHelper
{
    public static async Task<T> CallGetMethodAsync<T>(string apiProvider, string method)
    {
        using (RestClient client = new(apiProvider))
        {
            RestRequest request = new(method);
            T? result = await client.GetAsync<T>(request);
            return result.GetValue("REST method returned null");
        }
    }
}
