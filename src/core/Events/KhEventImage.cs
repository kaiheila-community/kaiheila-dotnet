using System;

namespace Kaiheila.Events
{
    public class KhEventImage : KhEventAsset
    {
        public KhEventImage(string path, string name) : base(path, name)
        {
            if (Type != "assets")
                throw new InvalidOperationException($"类型{Type}不是\"assets\"。");
        }
    }
}
