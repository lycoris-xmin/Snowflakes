using Lycoris.Snowflakes.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lycoris.Snowflakes.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DistributedSnowflakesSupport : IDistributedSnowflakesSupport
    {
        private readonly IDistributedSnowflakesRedis _distributedRedis;

        /// <summary>
        /// 当前生成的work节点
        /// </summary>
        private readonly string _currentWorkIndex;

        /// <summary>
        /// 使用过的work节点
        /// </summary>
        private readonly string _inUse;

        /// <summary>
        /// 
        /// </summary>
        private int _workId;

        /// <summary>
        /// 
        /// </summary>
        private readonly DistributedSnowflakeOption _option;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <param name="distributedRedis"></param>
        public DistributedSnowflakesSupport(DistributedSnowflakeOption option, IDistributedSnowflakesRedis distributedRedis)
        {
            _option = option;
            _distributedRedis = distributedRedis;
            _currentWorkIndex = $"{_option.RedisPrefix}:CurrentWorkIndex";
            _inUse = $"{_option.RedisPrefix}:Use";
        }

        /// <summary>
        /// 获取下一个可用的机器id
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> GetNextWorkIdAsync()
        {
            var cache = await StringAdditionAsync(_currentWorkIndex);
            _workId = (int)cache - 1;

            if (_workId > 1 << _option.WorkIdLength)
            {
                var startSorce = DateTime.Now.AddSeconds(-1800).AddSeconds(-(int)Math.Ceiling(_option.RefreshAliveInterval.TotalSeconds));
                var endSource = DateTime.Now.AddMinutes(-5);
                var newWorkdId = await SortRangeBySourceWithScoresAsync(_inUse, GetTimestamp(startSorce), GetTimestamp(endSource), offset: 1);
                if (!newWorkdId.Any())
                    throw new Exception("没有可用的节点");

                _workId = int.Parse(newWorkdId.First().Key);
            }

            //将正在使用的workId写入到有序列表中
            await _distributedRedis.ZAddAsync(_inUse, (GetTimestamp(), _workId.ToString()));
            return _workId;
        }

        /// <summary>
        /// 刷新机器id的存活状态
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAliveAsync() => await _distributedRedis.ZAddAsync(_inUse, (GetTimestamp(), _workId.ToString()));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RemoveNotAliveWorkNodeAsync()
        {
            var startSorce = DateTime.Now.AddSeconds(-1801).AddSeconds(-(int)Math.Ceiling(_option.RefreshAliveInterval.TotalSeconds));
            var notAliveWorkdId = await SortRangeBySourceWithScoresAsync(_inUse, 0, GetTimestamp(startSorce), count: 20);
            if (notAliveWorkdId != null && notAliveWorkdId.Count > 0)
            {
                foreach (var item in notAliveWorkdId)
                    await _distributedRedis.ZRemAsync(_inUse, notAliveWorkdId.Select(x => x.Key).ToArray());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static long GetTimestamp(DateTime? time = null)
        {
            if (time == null)
                time = DateTime.Now;
            var dt1970 = new DateTime(1970, 1, 1);
            return (time.Value.Ticks - dt1970.Ticks) / 10000;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        private async Task<long> StringAdditionAsync(string key, long value = 1, TimeSpan? timeSpan = null)
        {
            var cache = await _distributedRedis.IncrByAsync(key, value);
            if (timeSpan != null)
                await _distributedRedis.ExpireAsync(key, timeSpan.Value);

            return cache;
        }

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, decimal>> SortRangeBySourceWithScoresAsync(string key, decimal min, decimal max, long? count = null, long offset = 0)
        {
            var cache = await _distributedRedis.ZRangeByScoreWithScoresAsync(key, min, max, count, offset);
            if (cache == null || cache.Length == 0)
                return new Dictionary<string, decimal>();

            var dic = new Dictionary<string, decimal>();
            foreach (var (member, score) in cache)
                dic.Add(member, score);

            return dic.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
