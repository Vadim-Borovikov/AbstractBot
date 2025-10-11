using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface IBatcher
{
    Task DoForChunksAsync<T>(IEnumerable<T> sequence, Func<IList<T>, Task> work);
    Task<IEnumerable<TOutput>> DoForChunksAsync<TInput, TOutput>(IEnumerable<TInput> sequence,
        Func<IList<TInput>, Task<IEnumerable<TOutput>>> work);
}