﻿using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GryphonUtilities.Extensions;

namespace AbstractBot.Ngrok;

internal static class Manager
{
    internal static async Task<string> GetHostAsync(JsonSerializerOptions options)
    {
        try
        {
            ListTunnelsResult listTunnels = await Provider.ListTunnels(options);
            string? url = listTunnels.Tunnels?.Where(t => t?.Proto is DesiredNgrokProto).SingleOrDefault()?.PublicUrl;
            return url.GetValue(ErrorMessage);
        }
        catch (Exception e)
        {
            throw new Exception(ErrorMessage, e);
        }
    }

    private const string DesiredNgrokProto = "https";
    private const string ErrorMessage = "Can't retrieve NGrok host.";
}