using BuildingAspects.Behaviors;
using DomainModels.System;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RedisCacheAdapter
{
	public class CacheManager
	{
		private readonly string _redisConnectionStr;
		private readonly ILogger _logger;
		private ConnectionMultiplexer _redisConnection;
		public CacheManager(ILogger logger, string redisConnectionStr)
		{
			_logger = logger;
			_redisConnectionStr = redisConnectionStr ?? throw new ArgumentNullException("redis cache connection is required.");
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

		public byte[] GetKey(byte[] key)
		{
			return CacheDB.StringGetAsync(key)?.Result;
		}

		public bool SetKey(byte[] key, byte[] value)
		{
			return CacheDB.StringSetAsync(key, value).Result;
		}
	}
}
