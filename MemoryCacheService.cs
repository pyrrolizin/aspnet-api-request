using Microsoft.Extensions.Caching.Memory;

namespace Utils
{
    public class MemoryCacheService : IMemoryCacheService
    {
        public MemoryCache Cache { get; set; }
        public MemoryCacheService()
        {
            Cache = new MemoryCache(new MemoryCacheOptions());
        }
    }
}