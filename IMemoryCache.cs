using Microsoft.Extensions.Caching.Memory;

namespace Utils
{
    public interface IMemoryCacheService
    {
        MemoryCache Cache { get; }
    }
}