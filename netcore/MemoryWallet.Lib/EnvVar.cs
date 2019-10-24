using System;

namespace MemoryWallet.Lib
{
    public static class EnvVar
    {
        public static string DataStorageLocation
        {
            get
            {
                var val = Environment.GetEnvironmentVariable("MEMORY_WALLET__DATA_REPOSITORY",
                    EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(val))
                    throw new Exception("MEMORY_WALLET__DATA_REPOSITORY is missing from Env Var");

                return val;
            }
        }
    }
}