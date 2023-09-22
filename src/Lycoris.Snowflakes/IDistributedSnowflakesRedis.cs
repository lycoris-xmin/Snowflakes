using System;
using System.Threading.Tasks;

namespace Lycoris.Snowflakes
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDistributedSnowflakesRedis
    {
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment）
        /// </summary>
        /// <param name="key">redis键</param>
        /// <param name="value">增量值</param>
        /// <returns></returns>
        Task<long> IncrByAsync(string key, long value);

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key">redis键</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        Task<bool> ExpireAsync(string key, TimeSpan expire);

        /// <summary>
        /// [redis-server 5.0.0] 删除并返回有序集合key中的最多count个具有最低得分的成员。如未指定，count的默认值为1。指定一个大于有序集合的基数的count不会产生错误。
        /// 当返回多个元素时候，得分最低的元素将是第一个元素，然后是分数较高的元素。
        /// </summary>
        /// <param name="key">redis键</param>
        /// <param name="scoreMembers">过期时间</param>
        /// <returns></returns>
        Task<long> ZAddAsync(string key, params (decimal, object)[] scoreMembers);

        /// <summary>
        /// 移除有序集合中的一个或多个成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">redis键</param>
        /// <param name="member">一个或多个成员</param>
        /// <returns></returns>
        Task<long> ZRemAsync<T>(string key, params T[] member);

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员和分数
        /// </summary>
        /// <param name="key">redis键</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="count">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        Task<(string member, decimal score)[]> ZRangeByScoreWithScoresAsync(string key, decimal min, decimal max, long? count = null, long offset = 0L);
    }
}
