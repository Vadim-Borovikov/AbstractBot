using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbstractBot.Utilities.Extensions;

[PublicAPI]
public static class TaskExtensions
{
    public static async Task<IEnumerable<T>> ToIEnumerable<T>(this Task<T[]> array) => await array;
}