namespace MemoryWallet.Lib.Model
{
    public class PlayerAlreadyRegisteredEvt
    {
        public PlayerAlreadyRegisteredEvt(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }
}