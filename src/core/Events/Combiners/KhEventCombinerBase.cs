using System;

namespace Kaiheila.Events.Combiners
{
    public abstract class KhEventCombinerBase
    {
        public abstract KhEventBase Combine(KhEventBase a, KhEventBase b);
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class KhEventCombinerAttribute : Attribute
    {
        public KhEventCombinerAttribute(Type type)
        {
            Type = type;
        }

        public readonly Type Type;
    }
}
