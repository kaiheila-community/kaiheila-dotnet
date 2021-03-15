using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Kaiheila.Client.Rest;
using Kaiheila.Client.WebHook;
using Kaiheila.Events;

namespace Kaiheila.Client
{
    /// <summary>
    /// Kaiheila 机器人。
    /// </summary>
    /// <remarks>
    /// <see cref="Bot"/> 类是所有 Kaiheila 机器人的基类。
    /// <para><see cref="Bot"/> 类中提供了诸如 <see cref="Start"/> 和 <see cref="Dispose"/> 等公用方法。</para>
    /// <para><see cref="Bot"/> 类是一个抽象类。你需要使用其派生类来初始化 Kaiheila 机器人。</para>
    /// </remarks>
    /// <example>
    /// 下面的示例演示了如何创建一个 <see cref="WebHookClient"/> 实例并连接。
    /// <code>
    /// namespace BotExample
    /// {
    ///     public static class Program
    ///     {
    ///         public static void Main(string[] args)
    ///         {
    ///             Bot bot =
    ///                 WebHookClient.CreateWebHookClient() // 使用「WebHookClient」来创建
    ///                     .Configure(options =>
    ///                     {
    ///                         options
    ///                             .Listen(8000) // 要监听的端口
    ///                             .UseEncryptKey("VYDVSU") // 「开发者中心」中提供的 Encrypt Key
    ///                             .UseVerifyToken("JEe-o85KiCd78fi") // 「开发者中心」中提供的 Verify Token
    ///                             .UseBotAuthorization("1E/3p7EClDQ=/Fro4cT47ipnqcERGFDvfQw=="); // 「开发者中心」中提供的 Token
    ///                     }).Build(); // 创建「WebHookClient」
    /// 
    ///             bot.Start(); // 启动机器人
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="RestClient"/>
    /// <seealso cref="WebHookClient"/>
    public abstract class Bot : IDisposable
    {
        #region Lifecycle

        /// <summary>
        /// 连接到 Kaiheila 服务器。
        /// </summary>
        /// <remarks>
        /// <para>在连接到 Kaiheila 服务器之后，<see cref="Event"/> 就会推送所有的服务器消息。</para>
        /// <para>如果你不需要接收服务器消息，请使用 <see cref="RestClient"/>。<see cref="RestClient"/> 实例无需调用 <see cref="Start"/> 即可发送 <see cref="KhEventBase"/>。</para>
        /// </remarks>
        public abstract void Start();

        #endregion

        #region Message

        protected internal abstract Task<(string, long)> SendTextMessage(
            int type,
            long channel,
            string content,
            string quote = null);

        #endregion

        #region Dispose

        /// <summary>
        /// 断开 <see cref="Bot"/> 与 Kaiheila 服务器的连接，并释放所有资源。
        /// </summary>
        /// <remarks>
        /// <para>在执行一次 <see cref="Dispose"/> 方法之后，该对象的所有资源将会立即被销毁。</para>
        /// <para>若要再次使用机器人，请创建一个新的 <see cref="Bot"/> 实例。</para>
        /// </remarks>
        public abstract void Dispose();

        #endregion
    }
}
