using System.Threading.Tasks;

namespace Lycoris.Snowflakes
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDistributedSnowflakesSupport
    {
        /// <summary>
        /// 获取下一个可用的机器id
        /// </summary>
        /// <returns></returns>
        Task<int> GetNextWorkIdAsync();

        /// <summary>
        /// 刷新机器id的存活状态
        /// </summary>
        /// <returns></returns>
        Task RefreshAliveAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task RemoveNotAliveWorkNodeAsync();
    }
}
