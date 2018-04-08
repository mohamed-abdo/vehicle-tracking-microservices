using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisCacheAdapter
{
    public interface ICacheProvider
    {
        Task<byte[]> Get(byte[] key);
        void Set(byte[] key, byte[] value, TimeSpan? timeout);
        Task<string> Get(string key);
        void Set(string key, byte[] value, TimeSpan? timeout);
        Task<IEnumerable<string>> GetMembers(string key);
        void SetMembers(string key, IEnumerable<string> values);
        Task<Dictionary<string, string>> GetHash(string key);
        void SetHash(string key, Dictionary<string, string> value);
    }
}
