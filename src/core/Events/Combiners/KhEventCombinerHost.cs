using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Kaiheila.Events.Combiners
{
    /// <summary>
    /// Kaiheila事件合并器主机。
    /// </summary>
    [Export]
    public class KhEventCombinerHost
    {
        /// <summary>
        /// 初始化Kaiheila事件合并器主机。
        /// </summary>
        /// <param name="logger">Kaiheila事件合并器主机日志记录器。</param>
        public KhEventCombinerHost(
            ILogger<KhEventCombinerHost> logger)
        {
            _logger = logger;

            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                x.GetTypes()
                    .Where(type => Attribute.GetCustomAttribute(type, typeof(KhEventCombinerAttribute)) is not null)))
            {
                Type targetType = (Attribute.GetCustomAttribute(type, typeof(KhEventCombinerAttribute)) as KhEventCombinerAttribute)?.Type;
                if (targetType == null || _combiners.ContainsKey(targetType) || type.FullName == null) continue;

                _combiners.Add(targetType, Activator.CreateInstance(type) as KhEventCombinerBase);
            }

            _logger.LogInformation($"加载了{_combiners.Count}个Kaiheila事件合并器。");
        }

        public KhEventBase Combine(KhEventBase a, KhEventBase b)
        {
            if (a.GetType() != b.GetType())
                throw new InvalidOperationException($"{a.GetType().Name}和{b.GetType().Name}不是同一类型。");

            Type targetType = a.GetType();

            if (!_combiners.TryGetValue(targetType, out KhEventCombinerBase combiner))
                throw new NotImplementedException($"不支持{targetType.Name}的合并。");

            return combiner.Combine(a, b);
        }

        private readonly Dictionary<Type, KhEventCombinerBase> _combiners = new Dictionary<Type, KhEventCombinerBase>();

        /// <summary>
        /// Kaiheila事件合并器主机日志记录器。
        /// </summary>
        private readonly ILogger<KhEventCombinerHost> _logger;
    }
}
