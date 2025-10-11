using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;

namespace AbstractBot.Modules;

[PublicAPI]
public class Batcher : IBatcher
{
    public Batcher(byte size) => _size = size;

    public async Task DoForChunksAsync<T>(IEnumerable<T> sequence, Func<IList<T>, Task> work)
    {
        foreach (T[] chunk in sequence.Chunk(_size))
        {
            await work(chunk);
        }
    }

    public async Task<IEnumerable<TOutput>> DoForChunksAsync<TInput, TOutput>(IEnumerable<TInput> sequence,
        Func<IList<TInput>, Task<IEnumerable<TOutput>>> work)
    {
        List<TOutput> result = new();
        foreach (TInput[] chunk in sequence.Chunk(_size))
        {
            IEnumerable<TOutput> resultChunk = await work(chunk);
            result.AddRange(resultChunk);
        }
        return result;
    }

    private readonly byte _size;
}