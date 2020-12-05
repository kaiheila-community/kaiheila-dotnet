using Kaiheila.Assets;

namespace Kaiheila.Events
{
    public abstract class KhEventAsset : KhEventBase
    {
        public const string AssetsPrefix = @"https://img.kaiheila.cn";

        public KhEventAsset(
            string path,
            string name)
        {
            Path = path;
            Name = name;
            Type = AssetsHelper.GetAssetType(path);
        }

        public string Type;

        public string Path;

        public string Name;

        public string GetUrl() => AssetsPrefix + Path;
    }
}
