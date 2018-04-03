using System;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using DomainModels.System;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace RedisCacheAdapter
{
    public class CacheManager : ICacheProvider
    {
        private const string _redisConnMSG = "redis cache connection is required.";
        private readonly string _redisConnectionStr;
        private readonly ILogger _logger;
        private ConnectionMultiplexer _redisConnection;
        public CacheManager(ILogger logger, string redisConnectionStr)
        {
            _logger = logger;
            _redisConnectionStr = redisConnectionStr ?? throw new ArgumentNullException(_redisConnMSG);
            _redisConnection = Redis;
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
             Redis?.GetDatabase();

        public async Task<byte[]> GetKey(byte[] key)
        {
            return await CacheDB.StringGetAsync(key);
        }

        public async Task<bool> SetKey(byte[] key, byte[] value)
        {
            return await CacheDB.StringSetAsync(key, value);
        }

        public async Task<byte[]> GetKey(string key)
        {
            return await CacheDB.StringGetAsync(key);
        }

        public async Task<bool> SetKey(string key, byte[] value)
        {
            return await CacheDB.StringSetAsync(key, value);
        }
    }
}
