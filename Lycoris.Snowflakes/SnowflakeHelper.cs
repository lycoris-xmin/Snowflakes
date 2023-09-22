using Lycoris.Snowflakes.Options;
using Lycoris.Snowflakes.Utils;
using System;
using System.Threading.Tasks;

namespace Lycoris.Snowflakes
{
    public static class SnowflakeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly static object locker = new object();

        /// <summary>
        /// 最后的时间戳
        /// </summary>
        private static long lastTimestamp = -1L;

        /// <summary>
        /// 最后的序号
        /// </summary>
        private static uint lastIndex = 0;

        /// <summary>
        /// 工作机器长度，最大支持1024个节点，可根据实际情况调整，比如调整为9，则最大支持512个节点，可把多出来的一位分配至序号，提高单位毫秒内支持的最大序号
        /// </summary>
        private static int _workIdLength;

        /// <summary>
        /// 支持的最大工作节点
        /// </summary>
        private static int _maxWorkId;

        /// <summary>
        /// 序号长度，最大支持4096个序号
        /// </summary>
        private static int _indexLength;

        /// <summary>
        /// 支持的最大序号
        /// </summary>
        private static int _maxIndex;

        /// <summary>
        /// 当前工作节点
        /// </summary>
        private static int? _workId;

        /// <summary>
        /// 
        /// </summary>
        private static SnowflakeOption _option;

        /// <summary>
        /// 
        /// </summary>
        internal static bool HelperEnabled = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        internal static void Init(SnowflakeOption option)
        {
            _option = option;

            _workIdLength = _option.WorkIdLength;
            _maxWorkId = 1 << _workIdLength;

            // 工作机器id和序列号的总长度是22位，为了使组件更灵活，根据机器id的长度计算序列号的长度。
            _indexLength = 22 - _workIdLength;
            _maxIndex = 1 << _indexLength;

            HelperEnabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static long GetNextId() => GetNextId(null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task<long> GetNextIdAsync() => GetNextIdAsync(null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static long GetNextId(int? workId)
        {
            if (workId != null)
                _workId = workId.Value;

            if (_workId > _maxWorkId)
                throw new ArgumentException($"workId value range is 0 - {_maxWorkId}");

            lock (locker)
            {
                if (_workId == null)
                    ChangeWorkId();

                var currentTimeStamp = SnowflakeUtils.SnowflakeTimeStamp(_option.StartTimeStamp.Ticks);
                // 如果当前序列号大于允许的最大序号，则表示，当前单位毫秒内，序号已用完，则获取时间戳。
                if (lastIndex >= _maxIndex)
                    currentTimeStamp = SnowflakeUtils.SnowflakeTimeStamp(_option.StartTimeStamp.Ticks, lastTimestamp);

                if (currentTimeStamp > lastTimestamp)
                {
                    lastIndex = 0;
                    lastTimestamp = currentTimeStamp;
                }
                else if (currentTimeStamp < lastTimestamp)
                {
                    // 发生时钟回拨，切换workId，可解决。
                    ChangeWorkId();
                    return GetNextId(workId);
                }

                if (_workId == null)
                    throw new ArgumentException(nameof(_workId));

                var work = _workId.Value << _indexLength;

                var time = currentTimeStamp << _indexLength + _workIdLength;

                var id = time | (long)work | lastIndex;

                lastIndex++;

                return id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Task<long> GetNextIdAsync(int? workId) => Task.FromResult(GetNextId(workId));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static void ChangeWorkId()
        {
            lock (locker)
            {
                if (_workId == null)
                    _workId = 0;

                if (_workId > 1 << _option.WorkIdLength)
                    _workId++;
            }
        }
    }
}
