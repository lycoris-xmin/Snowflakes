using System.Threading.Tasks;

namespace Lycoris.Snowflakes
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISnowflakeMaker
    {
        /// <summary>
        /// 获取id
        /// </summary>
        /// <returns></returns>
        long GetNextId();

        /// <summary>
        /// 获取id
        /// </summary>
        /// <returns></returns>
        Task<long> GetNextIdAsync();

        /// <summary>
        /// 获取id
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        long GetNextId(int? workId);

        /// <summary>
        /// 获取id
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        Task<long> GetNextIdAsync(int? workId);
    }
}
