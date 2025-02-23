using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Extensions;

[PublicAPI]
public static class PhotoSizesExtensions
{
    public static PhotoSize? Largest(this IEnumerable<PhotoSize>? photoSizes) => photoSizes?.MaxBy(p => p.Height);
}