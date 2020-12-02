using System;
using System.Linq;
using Kaiheila.Events.Combiners;
using Xunit;

namespace Kaiheila.Test.Events.Combiners
{
    public class KhEventCombiners
    {
        [Fact]
        public static void KhEventCombinersBasedOnKhEventCombiner()
        {
            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                x.GetTypes()
                    .Where(type => Attribute.GetCustomAttribute(type, typeof(KhEventCombinerAttribute)) is not null)))
                Assert.Equal(typeof(KhEventCombinerBase), type.BaseType);
        }
    }
}
