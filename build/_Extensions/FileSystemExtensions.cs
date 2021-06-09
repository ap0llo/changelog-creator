using System.Collections.Generic;
using System.Linq;
using Cake.Core.IO;

namespace Build
{
    internal static class FileSystemExtensions
    {
        public static ICollection<FilePath> GetFilePaths(this IFileSystem fileSystem, DirectoryPath? directory = null, string filter = "*", SearchScope scope = SearchScope.Current)
        {
            directory ??= new DirectoryPath(".");

            return fileSystem.GetDirectory(directory)
                .GetFiles(filter, scope)
                .Select(x => x.Path)
                .ToList();
        }
    }
}
