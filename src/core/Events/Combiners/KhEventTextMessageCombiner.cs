namespace Kaiheila.Events.Combiners
{
    [KhEventCombiner(typeof(KhEventTextMessage))]
    public class KhEventTextMessageCombiner : KhEventCombinerBase
    {
        public override KhEventBase Combine(KhEventBase a, KhEventBase b) =>
            new KhEventTextMessage
            {
                Content = (a as KhEventTextMessage)?.Content + (b as KhEventTextMessage)?.Content
            };
    }
}
