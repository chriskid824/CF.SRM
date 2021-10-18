using Convience.Filestorage.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public class Utility
    {
        public class UploadFile
        {
            public FileInfo GetFileInfoAsync(string path)
            {
                var fileInfo = new FileInfo(path);

                if (fileInfo.Exists)
                {
                    return fileInfo;
                }
                return null;
            }
            public string CreateFileFromStreamAsync(string path, Stream inputStream, bool overwrite = false)
            {
                if (!overwrite && System.IO.File.Exists(path))
                {
                    throw new FileStoreException($"Cannot create file '{path}' because it already exists.");
                }

                if (Directory.Exists(path))
                {
                    throw new FileStoreException($"Cannot create file '{path}' because it already exists as a directory.");
                }

                // Create directory path if it doesn't exist.
                var physicalDirectoryPath = Path.GetDirectoryName(path);
                Directory.CreateDirectory(physicalDirectoryPath);

                var fileInfo = new FileInfo(path);
                using (var outputStream = fileInfo.Create())
                {
                    inputStream.CopyTo(outputStream);
                }
                return path;
            }
        }
    }
}
