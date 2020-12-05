using System;

namespace Kaiheila.Events
{
    public class KhEventFile : KhEventAsset
    {
        public KhEventFile(string path, string name) : base(path, name)
        {
            if (Type != "attachments")
                throw new InvalidOperationException($"类型{Type}不是\"attachments\"。");
        }
    }
}
