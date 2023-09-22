using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Snowflakes.Options
{
    public class SnowflakeOptionBuilder : SnowflakeOption
    {
        internal readonly IServiceCollection services;

        public SnowflakeOptionBuilder(IServiceCollection services)
        {
            this.services = services;
        }
    }
}
