using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MemoryWallet.Web
{
    public static class Clients
    {
        /// <summary>
        /// Inbound message handler 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="method"></param>
        /// <param name="username"></param>
        /// <param name="message"></param>
        /// <remarks>
        /// This method is called from Javascript when amessage is received
        /// </remarks>
        [JSInvokable]
        public static Task ReceiveMessage(string username, string message)
        {
            Console.WriteLine($"ReceiveMessage: {username}:{message}");

            return Task.FromResult("received message return");
        }
    }
}