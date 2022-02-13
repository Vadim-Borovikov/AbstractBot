using System;
using System.Threading.Tasks;
using RestSharp;

namespace AbstractBot;

internal static class RestHelper
{
    public static async Task<T> CallGetMethodAsync<T>(string apiProvider, string method)
    {
        using (RestClient client = new(apiProvider))
        {
            RestRequest request = new(method);
            return await client.GetAsync<T>(request) ?? throw new NullReferenceException("REST method returned null");
        }
    }
}