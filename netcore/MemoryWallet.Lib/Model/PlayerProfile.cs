using System;

namespace MemoryWallet.Lib.Model
{
    public class PlayerProfile
    {
        public PlayerProfile(long id, string name, string email, decimal balance = 0)
        {
            Id = id;
            Name = name;
            Balance = balance;
            Email = email;
        }
        
        public long Id { get; private set;  }
        
        public string Name { get; private set;  }
        
        public decimal Balance { get; private set; }
        
        public string Email { get; private set; }


        public void Deposit(decimal amt)
        {
            Balance += amt;
        }

        public void Withdrawal(decimal amt)
        {
            if (Balance < amt)
            {
                throw new Exception($"Insufficient balance! ");
            }

            Balance -= amt;
        }

        public override string ToString()
        {
            return $"Player Id:{Id}, Name:{Name} Email:{Email} Balance:{Balance}";
        }
    }
}