using System.IO;

namespace MemoryWallet.Lib
{
    public interface IBaseFileFactory
    {
        /// <summary>
        /// Read file from assembly relative path
        /// </summary>
        /// <param name="file"></param>
        /// <param name="optional"></param>
        /// <returns></returns>
        string ReadRelative(string file, bool optional = false);

        string GetFullPath(string filePath);
    }
    
    public class BaseFileFactory : IBaseFileFactory
    {
        private readonly string _baseDirectory;
        
        public BaseFileFactory(string assemblyDirectory)
        {
            _baseDirectory = assemblyDirectory;
        }

        string IBaseFileFactory.ReadRelative(string file, bool optional)
        {
            var content = string.Empty;
            try
            {
                content = File.ReadAllText(Path.Combine(_baseDirectory, file));
            }
            catch (FileNotFoundException)
            {
                if (!optional)
                {
                    throw;
                }
            }

            return content;
        }

        string IBaseFileFactory.GetFullPath(string filePath)
        {
            return Path.Combine(_baseDirectory, filePath);
        }
    }
}