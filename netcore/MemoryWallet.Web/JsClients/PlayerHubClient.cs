using System;
using System.Collections.Generic;
using Microsoft.JSInterop;

namespace MemoryWallet.Web.JsClients
{
    public class PlayerHubClient : IDisposable
    {
        private readonly string _username;
        private readonly IJSRuntime _jsRuntime;

        /// <summary>
        /// Signalr clients by key
        /// </summary>
        private static readonly Dictionary<string, PlayerHubClient> _clients = new Dictionary<string, PlayerHubClient>();

        public PlayerHubClient(string username, IJSRuntime jsRuntime)
        {
            _username = username;
            _jsRuntime = jsRuntime;
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing playerhubclient");
            
        }
    }
}