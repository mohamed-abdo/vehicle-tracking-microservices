using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisCacheAdapter
{
    public interface ICacheProvider
    {
        Task<byte[]> GetKey(string key);
        Task<bool> SetKey(string key, byte[] value);
        Dictionary<string, string> GetHashKey(string key);
        void SetHashKey(string key, Dictionary<string, string> value);
    }
}
