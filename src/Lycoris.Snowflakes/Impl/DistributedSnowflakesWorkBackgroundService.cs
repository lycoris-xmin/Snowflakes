using Lycoris.Snowflakes.Options;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lycoris.Snowflakes.Impl
{
    public class DistributedSnowflakesWorkBackgroundService : BackgroundService
    {
        private readonly DistributedSnowflakeOption _option;
        private readonly IDistributedSnowflakesSupport _distributedSupport;
        private readonly int RefreshAliveInterval = 0;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="option"></param>
        /// <param name="distributedSupport"></param>
        public DistributedSnowflakesWorkBackgroundService(DistributedSnowflakeOption option, IDistributedSnowflakesSupport distributedSupport)
        {
            _option = option;
            _distributedSupport = distributedSupport;
            RefreshAliveInterval = (int)Math.Ceiling(_option.RefreshAliveInterval.Add(TimeSpan.FromMinutes(1)).TotalMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                if (_option.Type == DistributedSnowflakeType.AsService || _option.Type == DistributedSnowflakeType.AsAll)
                    await ServiceSupportAsync();

                if (_option.Type == DistributedSnowflakeType.AsHelper || _option.Type == DistributedSnowflakeType.AsAll)
                    await HelperSupportAsync();

                // 延时
                await Task.Delay(RefreshAliveInterval, stoppingToken);

            } while (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task ServiceSupportAsync()
        {
            try
            {
                // 刷新机器存活时间
                await _distributedSupport.RefreshAliveAsync();

                // 移除未按照心跳时间刷新的机器Id
                await _distributedSupport.RemoveNotAliveWorkNodeAsync();
            }
            catch (Exception ex)
            {
                //_logger.Error("refresh machine alive time failed", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task HelperSupportAsync()
        {
            try
            {
                // 刷新机器存活时间
                await DistributedSnowflakeHelper._distributedSupport.RefreshAliveAsync();

                // 移除未按照心跳时间刷新的机器Id
                await DistributedSnowflakeHelper._distributedSupport.RemoveNotAliveWorkNodeAsync();
            }
            catch (Exception ex)
            {
                //_logger.Error("refresh machine alive time failed", ex);
            }
        }
    }
}
