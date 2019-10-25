using Microsoft.AspNetCore.Mvc;

namespace MemoryWallet.Web.Models
{
    public class RegisterPlayerModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class PlayerLoginModel
    {
        public string Email { get; set; }
    }
}