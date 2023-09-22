using System;

namespace Lycoris.Snowflakes.Options
{
    public class SnowflakeOption
    {
        /// <summary>
        /// 工作机器ID，默认从1开始
        /// 用于防止时钟回拨导致的Id重复
        /// </summary>
        public int? WorkId { get; set; } = 1;

        /// <summary>
        /// 工作机器id所占用的长度，最大10，默认10
        /// </summary>
        public int WorkIdLength { get; set; } = 10;

        /// <summary>
        /// 用于计算时间戳的开始时间，默认起始时间 UTC 2000-01-01
        /// </summary>
        public DateTime StartTimeStamp { get; set; } = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
