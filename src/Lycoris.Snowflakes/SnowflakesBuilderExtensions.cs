using Lycoris.Snowflakes.Impl;
using Lycoris.Snowflakes.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Lycoris.Snowflakes
{
    public static class SnowflakesBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SnowflakeOptionBuilder AddSnowflake(this IServiceCollection services) => new SnowflakeOptionBuilder(services);

        /// <summary>
        /// 添加单机雪花Id服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static SnowflakeOptionBuilder AddSnowflake(this IServiceCollection services, Action<SnowflakeOptionBuilder> configure)
        {
            var builder = new SnowflakeOptionBuilder(services);

            configure.Invoke(builder);

            return builder;
        }

        /// <summary>
        /// 注册为单例服务，使用 <see cref="ISnowflakeMaker.GetNextId()"/> 或 <see cref="ISnowflakeMaker.GetNextIdAsync()"/> 获取雪花Id
        /// </summary>
        /// <param name="builder"></param>
        public static IServiceCollection AsService(this SnowflakeOptionBuilder builder)
        {
            if (SnowflakeHelper.HelperEnabled)
                throw new Exception("");

            builder.services.Configure<SnowflakeOption>(opt =>
            {
                opt.WorkId = builder.WorkId;
                opt.WorkIdLength = builder.WorkIdLength;
                opt.StartTimeStamp = builder.StartTimeStamp;
            });

            builder.services.TryAddSingleton<ISnowflakeMaker, SnowflakesMakerService>();

            return builder.services;
        }

        /// <summary>
        /// 注册为静态实例，使用 <see cref="SnowflakeHelper.GetNextId()"/> 或 <see cref="SnowflakeHelper.GetNextIdAsync()"/> 获取雪花Id
        /// </summary>
        /// <param name="builder"></param>
        public static IServiceCollection AsHelper(this SnowflakeOptionBuilder builder)
        {
            if (builder.services.Any(f => f.ImplementationType == typeof(SnowflakesMakerService)))
                throw new Exception("");

            SnowflakeHelper.Init(builder);

            return builder.services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static DistributedSnowflakeOptionBuilder AddDistributedSnowflake(this IServiceCollection services) => new DistributedSnowflakeOptionBuilder(services);

        /// <summary>
        /// 添加分布式雪花Id服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static DistributedSnowflakeOptionBuilder AddDistributedSnowflake(this IServiceCollection services, Action<DistributedSnowflakeOptionBuilder> configure)
        {
            var builder = new DistributedSnowflakeOptionBuilder(services);

            configure.Invoke(builder);

            return builder;
        }

        /// <summary>
        /// 注册为单例服务，使用 <see cref="ISnowflakeMaker.GetNextId()"/> 或 <see cref="ISnowflakeMaker.GetNextIdAsync()"/> 获取雪花Id
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AsService(this DistributedSnowflakeOptionBuilder builder)
        {
            if (DistributedSnowflakeHelper.HelperEnabled)
                throw new Exception("");

            builder.services.Configure<DistributedSnowflakeOption>(opt =>
            {
                opt.WorkId = builder.WorkId;
                opt.WorkIdLength = builder.WorkIdLength;
                opt.StartTimeStamp = builder.StartTimeStamp;
                opt.RedisPrefix = builder.RedisPrefix;
                opt.RefreshAliveInterval = builder.RefreshAliveInterval;
            });

            builder.services.TryAddSingleton(typeof(IDistributedSnowflakesRedis), builder.redisType);

            if (!builder.services.Any(f => f.ImplementationType == typeof(DistributedSnowflakesSupport)))
                builder.services.AddSingleton<IDistributedSnowflakesSupport, DistributedSnowflakesSupport>(sp => new DistributedSnowflakesSupport(sp.GetRequiredService<IOptions<DistributedSnowflakeOption>>().Value, sp.GetRequiredService<IDistributedSnowflakesRedis>()));

            builder.services.TryAddSingleton<ISnowflakeMaker, DistributedSnowflakeService>();

            builder.services.AddHostedService(sp =>
            {
                var option = sp.GetRequiredService<IOptions<DistributedSnowflakeOption>>();
                option.Value.Type = DistributedSnowflakeType.AsService;
                return new DistributedSnowflakesWorkBackgroundService(option.Value, sp.GetRequiredService<IDistributedSnowflakesSupport>());
            });

            return builder.services;
        }

        /// <summary>
        /// 注册为静态实例，使用 <see cref="DistributedSnowflakeHelper.GetNextId()"/> 或 <see cref="DistributedSnowflakeHelper.GetNextIdAsync()"/> 获取雪花Id
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AsHelper(this DistributedSnowflakeOptionBuilder builder)
        {
            if (builder.services.Any(f => f.ImplementationType == typeof(DistributedSnowflakeService)))
                throw new Exception("");

            var option = new DistributedSnowflakeOption()
            {
                WorkId = builder.WorkId,
                WorkIdLength = builder.WorkIdLength,
                StartTimeStamp = builder.StartTimeStamp,
                RedisPrefix = builder.RedisPrefix,
                RefreshAliveInterval = builder.RefreshAliveInterval,
                Type = DistributedSnowflakeType.AsHelper
            };

            var redisHelper = new DistributedSnowflakesSupport(option, builder.redisHelper);

            DistributedSnowflakeHelper.Init(option, redisHelper);

            builder.services.AddHostedService(sp => new DistributedSnowflakesWorkBackgroundService(option, null));

            return builder.services;
        }
    }
}
