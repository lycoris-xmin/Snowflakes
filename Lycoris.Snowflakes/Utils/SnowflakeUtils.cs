using System;

namespace Lycoris.Snowflakes.Utils
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class SnowflakeUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        public static long SnowflakeTimeStamp(long ticks, long lastTimestamp = 0L)
        {
            var current = (DateTime.Now.Ticks - ticks) / 10000;
            return lastTimestamp == current ? SnowflakeTimeStamp(lastTimestamp) : current;
        }
    }
}
