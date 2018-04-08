using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisCacheAdapter
{
    public interface ICacheProvider
    {
        Task<byte[]> Get(byte[] key);
        Task Set(byte[] key, byte[] value, TimeSpan? timeout); 
        Task<string> Get(string key);
        Task Set(string key, string value, TimeSpan? timeout);
        Task<IEnumerable<string>> GetMembers(string key);
        Task SetMembers(string key, IEnumerable<string> values);
        Task<Dictionary<string, string>> GetHash(string key);
        Task SetHash(string key, Dictionary<string, string> value);
    }
}
