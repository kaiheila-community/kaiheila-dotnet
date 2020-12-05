using System;

namespace Kaiheila.Assets
{
    public static class AssetsHelper
    {
        public static string GetAssetType(string path)
        {
            string[] split = path.Split('/');

            if (split.Length < 3 || string.IsNullOrEmpty(split[1]))
                throw new ArgumentOutOfRangeException(nameof(path), "Path不正确。");

            return split[1];
        }
    }
}
