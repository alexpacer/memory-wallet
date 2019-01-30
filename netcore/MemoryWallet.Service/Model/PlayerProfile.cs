namespace MemoryWallet.Service.Model
{
    public class PlayerProfile
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public decimal Balance { get; set; }
        
        public string Email { get; set; }
    }
}