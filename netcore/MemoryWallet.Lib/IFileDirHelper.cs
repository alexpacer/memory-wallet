using System;
using System.IO;
using System.Threading;

namespace MemoryWallet.Lib
{
    public interface IFileDirHelper
    {
        void CopyFile(string source, string target, bool overwrite);
        void CopyFile(string source, string target, bool overwrite, int numberOfRetryTimes, int retryWaitInterval = 10);
        string ReadAllText(string filePath);
        void WriteStream(MemoryStream dataStream, string targetFile);
        MemoryStream ReadToStream(string file);
        void DeleteFile(string filePath);
        void DeleteDirectory(string dirPath, bool recursive);
        bool FileExists(string dirPath);
        bool DirectoryExists(string dirPath);
        string GetDirectory(string filePath);
        long GetFileSize(string filePath);
        string GetUserTempDirectory();
        string BuildPath(params string[] paths);
        void CreateIfNotExists(string directoryPath);
        string GetFileName(string softpaqFile, bool withoutExtension = false);
        void CreateTextFile(string filePath, string content);
        void CreateTextFileAsync(string filePath, string content);
        
    }

    public class FileDirDirHelper : IFileDirHelper
    {
        /// <summary>
        /// Copy file from `source` path to `target` path, overwrites target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="overWrite"></param>
        void IFileDirHelper.CopyFile(string source, string target, bool overWrite)
        {
            var targetDir = Path.GetDirectoryName(target);
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            File.Copy(source, target, overWrite);
        }

        void IFileDirHelper.CopyFile(string source, string target, bool overWrite, int numberOfRetryTimes,
            int retryWaitInterval)
        {
            var self = (IFileDirHelper) this;
            bool retry = true;
            var retryCounter = 0;
            while (retry)
            {
                try
                {
                    self.CopyFile(source, target, overWrite);
                    retry = false;
                }
                catch (IOException)
                {
                    retryCounter++;
                    // handle writing file problem with retry 
                    Thread.Sleep(TimeSpan.FromSeconds(retryWaitInterval));

                    if (retryCounter >= numberOfRetryTimes)
                    {
                        retry = false;
                    }
                }
            }
        }

        string IFileDirHelper.ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        void IFileDirHelper.WriteStream(MemoryStream dataStream, string targetFile)
        {
            using (var f = File.Open(targetFile, FileMode.Create, FileAccess.Write))
            {
                dataStream.Seek(0, SeekOrigin.Begin);
                dataStream.CopyTo(f);
            }
        }

        MemoryStream IFileDirHelper.ReadToStream(string file)
        {
            var ms = new MemoryStream();
            using var f = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] bytes = new byte[file.Length];
            f.Read(bytes, 0, (int)file.Length);
            ms.Write(bytes, 0, (int)file.Length);

            return ms;
        }

        void IFileDirHelper.DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        void IFileDirHelper.DeleteDirectory(string dirPath, bool recursive)
        {
            if (Directory.Exists(dirPath))
            {
                try
                {
                    Directory.Delete(dirPath, recursive);
                }
                catch (UnauthorizedAccessException)
                {
                    // assuming this is a read-only file, remove the attr and try again
                    DiveDirectory(dirPath, s =>
                    {
                        File.SetAttributes(s, FileAttributes.Normal);
                        File.Delete(s);
                    });

                    Directory.Delete(dirPath, recursive);
                }
            }
        }

        private void DiveDirectory(string startingDir, Action<string> fileOperation)
        {
            foreach (string d in Directory.GetDirectories(startingDir))
            {
                foreach (string f in Directory.GetFiles(d))
                {
                    fileOperation(f);
                }

                DiveDirectory(d, fileOperation);
            }
        }


        bool IFileDirHelper.FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        bool IFileDirHelper.DirectoryExists(string dirPath)
        {
            return Directory.Exists(dirPath);
        }

        string IFileDirHelper.GetDirectory(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        long IFileDirHelper.GetFileSize(string filePath)
        {
            var info = new FileInfo(filePath);
            if (!info.Exists)
                throw new FileNotFoundException($"{filePath} cannot be located.");


            return info.Length;
        }

        string IFileDirHelper.GetUserTempDirectory()
        {
            return Path.GetTempPath();
        }

        string IFileDirHelper.BuildPath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        void IFileDirHelper.CreateIfNotExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        }

        string IFileDirHelper.GetFileName(string softpaqFile, bool withoutExtension)
        {
            if (withoutExtension)
            {
                return Path.GetFileNameWithoutExtension(softpaqFile);
            }

            return Path.GetFileName(softpaqFile);
        }

        void IFileDirHelper.CreateTextFile(string filePath, string content)
        {
            using (var f = File.CreateText(filePath))
            {
                f.Write(content);
            }
        }

        async void IFileDirHelper.CreateTextFileAsync(string filePath, string content)
        {
            using (var f = File.CreateText(filePath))
            {
                await f.WriteAsync(content);
            }
        }
    }
}