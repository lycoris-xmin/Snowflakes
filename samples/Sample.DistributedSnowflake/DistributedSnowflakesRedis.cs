using CSRedis;
using Lycoris.Snowflakes;

namespace Sample.DistributedSnowflake
{
    public class DistributedSnowflakesRedis : IDistributedSnowflakesRedis
    {
        private readonly CSRedisClient client;

        public DistributedSnowflakesRedis()
        {
            client = new CSRedisClient("host:port,password=password,defaultDatabase=0");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public Task<bool> ExpireAsync(string key, TimeSpan expire) => client.ExpireAsync(key, expire);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<long> IncrByAsync(string key, long value) => client.IncrByAsync(key, value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="scoreMembers"></param>
        /// <returns></returns>
        public Task<long> ZAddAsync(string key, params (decimal, object)[] scoreMembers) => client.ZAddAsync(key, scoreMembers);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Task<(string member, decimal score)[]> ZRangeByScoreWithScoresAsync(string key, decimal min, decimal max, long? count = null, long offset = 0) => client.ZRangeByScoreWithScoresAsync(key, min, max, count, offset);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public Task<long> ZRemAsync<T>(string key, params T[] member) => client.ZRemAsync(key, member);
    }
}
