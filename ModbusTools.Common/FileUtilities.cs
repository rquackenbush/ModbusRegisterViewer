using System.IO;

namespace ModbusTools.Common
{
    public static class FileUtilities
    {
        /// <summary>
        /// Will attempt to create the path for a file if it doesn't exist
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectoryForFile(string path)
        {
            var directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
