using System;
using System.Threading.Tasks;

namespace RedisCacheAdapter
{
    public interface ICacheProvider
    {
        Task<byte[]> GetKey(string key);
        Task<bool> SetKey(string key, byte[] value);
    }
}
