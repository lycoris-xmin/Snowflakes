using Lycoris.Snowflakes.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lycoris.Snowflakes.Impl
{
    public class DistributedSnowflakesWorkBackgroundService : BackgroundService
    {
        private readonly DistributedSnowflakeOption _option;
        private readonly IDistributedSnowflakesSupport _distributedSupport;
        private readonly int _refreshAliveInterval = 0;
        private readonly ILogger _logger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="option"></param>
        /// <param name="distributedSupport"></param>
        public DistributedSnowflakesWorkBackgroundService(DistributedSnowflakeOption option, IDistributedSnowflakesSupport distributedSupport, ILoggerFactory factory)
        {
            _option = option;
            _distributedSupport = distributedSupport;
            _refreshAliveInterval = (int)Math.Ceiling(_option.RefreshAliveInterval.Add(TimeSpan.FromMinutes(1)).TotalMilliseconds);
            _logger = factory?.CreateLogger<DistributedSnowflakesWorkBackgroundService>();
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
                if (_option.Type == DistributedSnowflakeType.AsService)
                    await ServiceSupportAsync();
                else
                    await HelperSupportAsync();

                // 延时
                await Task.Delay(_refreshAliveInterval, stoppingToken);

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
                _logger?.LogError(ex, "refresh machine alive time failed");
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
                _logger?.LogError(ex, "refresh machine alive time failed");
            }
        }
    }
}
