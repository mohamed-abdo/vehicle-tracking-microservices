using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using DomainModels.System;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Linq;
namespace RedisCacheAdapter
{
    public class CacheManager : ICacheProvider
    {
        private const string _redisConnMSG = "redis cache connection is required.";
        private readonly string _redisConnectionStr;
        private readonly ILogger _logger;
        private ConnectionMultiplexer _redisConnection;
        private readonly int _dbIndex;
        public CacheManager(ILogger logger, string redisConnectionStr, int dbIndex = 0)
        {
            _logger = logger;
            _redisConnectionStr = redisConnectionStr ?? throw new ArgumentNullException(_redisConnMSG);
            _redisConnection = Redis;
            _dbIndex = dbIndex;
        }
        private ConnectionMultiplexer Redis =>
             _redisConnection == null || !_redisConnection.IsConnected ?
                new Function(_logger, Identifiers.RetryCount)
                    .Decorate(() =>
                     {
                         return ConnectionMultiplexer.Connect(_redisConnectionStr);
                     }).Result : _redisConnection;

        public IDatabase CacheDB =>

             //The object returned from GetDatabase is a cheap pass-thru object
             //Ref:https://stackexchange.github.io/StackExchange.Redis/Basics
             Redis?.GetDatabase(_dbIndex);

        public async Task<byte[]> Get(byte[] key)
        {
            return await CacheDB.StringGetAsync(key, CommandFlags.HighPriority);
        }

        public async Task Set(byte[] key, byte[] value, TimeSpan? timeout)
        {
            await CacheDB.StringSetAsync(key, value, timeout, When.Always, CommandFlags.FireAndForget);
        }

        public async Task<string> Get(string key)
        {
            return await CacheDB.StringGetAsync(key, CommandFlags.HighPriority);
        }

        public async Task Set(string key, string value, TimeSpan? timeout)
        {
            await CacheDB.StringSetAsync(key, value, timeout, When.Always, CommandFlags.FireAndForget);
        }

        public async Task<IEnumerable<string>> GetMembers(string key)
        {
            return (Array.ConvertAll(await CacheDB.SetMembersAsync(key, CommandFlags.HighPriority), m => (string)m));
        }

        public async Task SetMembers(string key, IEnumerable<string> values)
        {
            await CacheDB.SetAddAsync(key, Array.ConvertAll(values?.ToArray(), m => (RedisValue)m), CommandFlags.FireAndForget);
        }
        public async Task<Dictionary<string, string>> GetHash(string key)
        {
            return (await CacheDB.HashGetAllAsync(key, CommandFlags.HighPriority))?.ToStringDictionary();
        }

        public async Task SetHash(string key, Dictionary<string, string> value)
        {
            var data = value.Select(d => new HashEntry(d.Key, d.Value));
            await CacheDB.HashSetAsync(key, data?.ToArray(), CommandFlags.FireAndForget);
        }

    }
}
