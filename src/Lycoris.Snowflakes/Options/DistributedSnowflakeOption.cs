using System;

namespace Lycoris.Snowflakes.Options
{
    public class DistributedSnowflakeOption : SnowflakeOption
    {
        /// <summary>
        /// 分布式路由前缀(根据对应集群或者服务类别配置不同的前缀，如果未设置，则会随机生成guid)
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
        AsHelper = 1
    }
}
