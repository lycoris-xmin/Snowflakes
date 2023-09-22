using System;

namespace Lycoris.Snowflakes.Options
{
    public class DistributedSnowflakeOption : SnowflakeOption
    {
        /// <summary>
        /// 分布式Id redis缓存前缀
        /// 默认没有前缀
        /// </summary>
        public string RedisPrefix { get; set; }

        /// <summary>
        /// 分布式Id 刷新存活状态的间隔时间，默认1小时
        /// </summary>
        public TimeSpan RefreshAliveInterval { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// 
        /// </summary>
        internal DistributedSnowflakeType Type { get; set; }
    }

    public enum DistributedSnowflakeType
    {
        AsService = 0,
        AsHelper = 1,
        AsAll = 2
    }
}
