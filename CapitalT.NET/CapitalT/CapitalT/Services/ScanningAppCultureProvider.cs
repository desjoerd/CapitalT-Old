using CapitalT.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Hosting;

namespace CapitalT.Services
{
    public class ScanningAppCultureProvider : IAppCultureProvider
    {
        private const string AppCulturesCacheKey = "CapitalT.AppCultures";

        private readonly ILocalizationDirectoryProvider _localizationDirectoryProvider;

        public ScanningAppCultureProvider(ILocalizationDirectoryProvider localizationDirectoryProvider)
        {
            this._localizationDirectoryProvider = localizationDirectoryProvider;
        }

        public IEnumerable<CultureInfo> GetAppCultures()
        {
            var appCultures = HostingEnvironment.Cache[AppCulturesCacheKey] as IEnumerable<CultureInfo>;
            if (appCultures == null)
            {
                var directories = _localizationDirectoryProvider.GetLocalizationDirectories()
                    .Distinct();

                var directoryPaths = directories.Select(dir => HostingEnvironment.MapPath(dir.VirtualPath)).ToArray();

                var cultureDirectoryNames = new List<string>();

                foreach (var directoryPath in directoryPaths)
                {
                    var directory = new DirectoryInfo(directoryPath);
                    if (directory.Exists)
                    {
                        cultureDirectoryNames.AddRange(directory.EnumerateDirectories().Select(d => d.Name));
                    }
                }
                var cultureNames = cultureDirectoryNames.Distinct();

                var appCultureCollection = new List<CultureInfo>();
                if (LocalizedApplication.Configuration.NeutralAppCulture != null)
                {
                    appCultureCollection.Add(LocalizedApplication.Configuration.NeutralAppCulture);
                }
                foreach (var cultureName in cultureNames)
                {
                    try
                    {
                        var culture = CultureInfo.GetCultureInfo(cultureName);
                        appCultureCollection.Add(culture);
                    }
                    catch (CultureNotFoundException) { }
                }
                appCultures = appCultureCollection;

                HostingEnvironment.Cache.Add(
                    AppCulturesCacheKey,
                    appCultures,
                    new CacheDependency(directoryPaths),
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.NotRemovable,
                    null);
            }

            return appCultures;
        }
    }
}
