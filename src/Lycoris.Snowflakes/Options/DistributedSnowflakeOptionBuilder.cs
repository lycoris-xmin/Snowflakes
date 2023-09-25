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

        internal Type redisType = null;

        internal IDistributedSnowflakesRedis redisHelper = null;

        public DistributedSnowflakeOptionBuilder(IServiceCollection services)
        {
            this.services = services;
        }
    }
}
