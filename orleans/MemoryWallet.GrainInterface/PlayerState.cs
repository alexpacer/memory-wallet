using System;

namespace MemoryWallet.GrainInterface
{
    [Serializable]
    public class PlayerState
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
    }
}
