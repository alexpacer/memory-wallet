using System.ComponentModel.DataAnnotations;

namespace MemoryWallet.Web.Data
{
    public class RegisterModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Email { get; set; }
    }
}