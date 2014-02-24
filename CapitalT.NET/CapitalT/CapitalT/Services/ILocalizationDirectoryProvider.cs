using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Services
{
    public interface ILocalizationDirectoryProvider
    {
        IEnumerable<LocalizationDirectory> GetLocalizationDirectories();
    }
}
