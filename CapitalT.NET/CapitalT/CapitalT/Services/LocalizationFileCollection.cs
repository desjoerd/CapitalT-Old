using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Services
{
    public class LocalizationFileCollection : ILocalizationDirectoryProvider
    {
        private readonly List<LocalizationDirectory> _directories;

        public LocalizationFileCollection()
        {
            _directories = new List<LocalizationDirectory>();
        }

        public IEnumerable<LocalizationDirectory> Directories { get { return _directories.AsReadOnly(); } }

        public LocalizationDirectory AddDirectory(string directoryVirtualPath)
        {
            var dir = _directories.Where(d => d.VirtualPath == directoryVirtualPath).FirstOrDefault();
            if (dir == null)
            {
                dir = new LocalizationDirectory(directoryVirtualPath);
                _directories.Add(dir);
            }
            return dir;
        }

        IEnumerable<LocalizationDirectory> ILocalizationDirectoryProvider.GetLocalizationDirectories()
        {
            return Directories;
        }
    }

    public class LocalizationDirectory : IEquatable<LocalizationDirectory>
    {
        private readonly string _virtualPath;

        private readonly List<LocalizationFile> _files;

        public LocalizationDirectory(string directoryVirtualPath)
        {
            _virtualPath = directoryVirtualPath;
            _files = new List<LocalizationFile>();
        }

        public string VirtualPath { get { return _virtualPath; } }

        public IEnumerable<LocalizationFile> Files { get { return _files.AsReadOnly(); } }

        public LocalizationDirectory AddFile(string fileName, int priority = 0)
        {
            _files.RemoveAll(file => file.FileName == fileName);

            _files.Add(new LocalizationFile(VirtualPath, fileName, priority));
            return this;
        }

        public bool Equals(LocalizationDirectory other)
        {
            return other != null && other.VirtualPath == VirtualPath;
        }
    }

    public class LocalizationFile
    {
        private readonly string _directoryVirtualPath;
        private readonly string _fileName;
        private readonly int _priority;

        public LocalizationFile(string directoryVirtualPath, string fileName, int priority)
        {
            _directoryVirtualPath = directoryVirtualPath;
            _fileName = fileName;
            _priority = priority;
        }

        public string DirectoryVirtualPath { get { return _directoryVirtualPath; } }

        public string FileName { get { return _fileName; } }

        public int Priority { get { return _priority; } }
    }
}
