using System;
using System.Collections.Concurrent;
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
    public class LocalizedStringService : ILocalizedStringService
    {
        private readonly ILocalizationDirectoryProvider _localizationDirectoryProvider;

        public LocalizedStringService(ILocalizationDirectoryProvider localizationDirectoryProvider)
        {
            this._localizationDirectoryProvider = localizationDirectoryProvider;
        }

        public string GetLocalizedString(string scope, string text, CultureInfo culture)
        {
            if (culture == LocalizedApplication.Configuration.NeutralAppCulture)
            {
                return text;
            }

            var localizedStrings = LoadCulture(culture);

            string scopedKey = (scope + "|" + text).ToLowerInvariant();
            if (localizedStrings.ContainsKey(scopedKey))
            {
                return localizedStrings[scopedKey];
            }

            string genericKey = ("|" + text).ToLowerInvariant();
            if (localizedStrings.ContainsKey(genericKey))
            {
                return localizedStrings[genericKey];
            }

            return GetParentTranslation(scope, text, culture);
        }

        private string GetParentTranslation(string scope, string text, CultureInfo culture)
        {
            string scopedKey = (scope + "|" + text).ToLowerInvariant();
            string genericKey = ("|" + text).ToLowerInvariant();
            try
            {
                CultureInfo parentCultureInfo = culture.Parent;
                if (parentCultureInfo.IsNeutralCulture)
                {
                    var localizedStrings = LoadCulture(parentCultureInfo);
                    if (localizedStrings.ContainsKey(scopedKey))
                    {
                        return localizedStrings[scopedKey];
                    }
                    if (localizedStrings.ContainsKey(genericKey))
                    {
                        return localizedStrings[genericKey];
                    }
                    return text;
                }
            }
            catch (CultureNotFoundException) { }

            return text;
        }

        private Dictionary<string, string> LoadCulture(CultureInfo culture)
        {
            var localizedStrings = HostingEnvironment.Cache[GetLocalizedStringsCacheKey(culture)] as Dictionary<string, string>;
            if (localizedStrings == null)
            {
                var cacheDateTime = DateTime.Now;
                IEnumerable<string> cacheDependencyPaths;
                localizedStrings = LoadLocalizedStringsForCulture(culture, out cacheDependencyPaths);

                HostingEnvironment.Cache.Add(
                    GetLocalizedStringsCacheKey(culture),
                    localizedStrings,
                    new CacheDependency(cacheDependencyPaths.ToArray(), new string[] { "CapitalT.AppCultures" }, cacheDateTime),
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.High,
                    null);
            }
            return localizedStrings;
        }

        private Dictionary<string, string> LoadLocalizedStringsForCulture(CultureInfo culture, out IEnumerable<string> cacheDependencyPaths)
        {
            var paths = _localizationDirectoryProvider.GetLocalizationDirectories()
                    .Distinct()
                .SelectMany(d => d.Files)
                .OrderBy(f => f.Priority)
                .Select(f => HostingEnvironment.MapPath(string.Format("{0}/{1}/{2}", f.DirectoryVirtualPath, culture.Name, f.FileName)))
                .ToList();

            var existingPaths = new List<string>();

            var localizedStrings = new Dictionary<string, string>();
            foreach (var path in paths)
            {
                var file = new FileInfo(path);
                if (file.Exists)
                {
                    existingPaths.Add(path);
                    try
                    {
                        using (var stream = file.OpenRead())
                        using (var reader = new StreamReader(stream))
                        {
                            ParseLocalizationStream(reader, localizedStrings, true);
                        }
                    }
                    catch (IOException) { }
                }
            }
            cacheDependencyPaths = existingPaths;
            return localizedStrings;
        }

        private static string GetLocalizedStringsCacheKey(CultureInfo culture)
        {
            return "po:" + culture.Name;
        }

        private static readonly Dictionary<char, char> _escapeTranslations = new Dictionary<char, char> {
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' }
        };

        private static string Unescape(string str)
        {
            StringBuilder sb = null;
            bool escaped = false;
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (escaped)
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder(str.Length);
                        if (i > 1)
                        {
                            sb.Append(str.Substring(0, i - 1));
                        }
                    }
                    char unescaped;
                    if (_escapeTranslations.TryGetValue(c, out unescaped))
                    {
                        sb.Append(unescaped);
                    }
                    else
                    {
                        // General rule: \x ==> x
                        sb.Append(c);
                    }
                    escaped = false;
                }
                else
                {
                    if (c == '\\')
                    {
                        escaped = true;
                    }
                    else if (sb != null)
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb == null ? str : sb.ToString();
        }

        private static void ParseLocalizationStream(TextReader reader, IDictionary<string, string> translations, bool merge)
        {
            string poLine, id, scope;
            id = scope = String.Empty;
            while ((poLine = reader.ReadLine()) != null)
            {
                if (poLine.StartsWith("#:"))
                {
                    scope = ParseScope(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgctxt"))
                {
                    scope = ParseContext(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgid"))
                {
                    id = ParseId(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgstr"))
                {
                    string translation = ParseTranslation(poLine);
                    // ignore incomplete localizations (empty msgid or msgstr)
                    if (!String.IsNullOrWhiteSpace(id) && !String.IsNullOrWhiteSpace(translation))
                    {
                        string scopedKey = (scope + "|" + id).ToLowerInvariant();
                        if (!translations.ContainsKey(scopedKey))
                        {
                            translations.Add(scopedKey, translation);
                        }
                        else
                        {
                            if (merge)
                            {
                                translations[scopedKey] = translation;
                            }
                        }
                    }
                    id = scope = String.Empty;
                }

            }
        }

        private static string ParseTranslation(string poLine)
        {
            return Unescape(poLine.Substring(6).Trim().Trim('"'));
        }

        private static string ParseId(string poLine)
        {
            return Unescape(poLine.Substring(5).Trim().Trim('"'));
        }

        private static string ParseScope(string poLine)
        {
            return Unescape(poLine.Substring(2).Trim().Trim('"'));
        }

        private static string ParseContext(string poLine)
        {
            return Unescape(poLine.Substring(7).Trim().Trim('"'));
        }
    }
}
