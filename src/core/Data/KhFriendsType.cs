using System;

namespace Kaiheila.Data
{
    [Obsolete]
    public enum KhFriendsType
    {
        Friend = 0,
        Request = 1,
        Blocked = 2
    }

    [Obsolete]
    public static class KhFriends
    {
        public static string GetTypeString(this KhFriendsType type) =>
            type switch
            {
                KhFriendsType.Friend => "friend",
                KhFriendsType.Request => "request",
                KhFriendsType.Blocked => "blocked",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
    }
}
