using System.Composition;
using Microsoft.Extensions.Logging;

namespace Kaiheila.Events.Combiners
{
    /// <summary>
    /// 开黑啦事件合并器主机。
    /// </summary>
    [Export]
    public class KhEventCombinerHost
    {
        /// <summary>
        /// 初始化开黑啦事件合并器主机。
        /// </summary>
        /// <param name="logger">开黑啦事件合并器主机日志记录器。</param>
        public KhEventCombinerHost(
            ILogger<KhEventCombinerHost> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 开黑啦事件合并器主机日志记录器。
        /// </summary>
        private readonly ILogger<KhEventCombinerHost> _logger;
    }
}
