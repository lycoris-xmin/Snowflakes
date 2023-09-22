using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lycoris.Snowflakes.Options
{
    /// <summary>
    /// 
    /// </summary>
    public class DistributedSnowflakeOptionBuilder : DistributedSnowflakeOption
    {
        internal readonly IServiceCollection services;

        internal Type redisType;

        internal IDistributedSnowflakesRedis redisHelper;

        public DistributedSnowflakeOptionBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DistributedSnowflakeOptionBuilder AddRedisService<T>() where T : IDistributedSnowflakesRedis
        {
            redisType = typeof(T);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DistributedSnowflakeOptionBuilder AddRedisHelper<T>() where T : IDistributedSnowflakesRedis, new()
        {
            redisHelper = new T();
            return this;
        }
    }
}
