using CapitalT.Culture;
using CapitalT.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT
{
    /// <summary>
    /// Customization:
    /// Multiple Culture selection strategies
    /// IUserCultureSelector[] UserCultureSelectors
    /// 
    /// Neutral Culture
    /// CultureInfo NeutralCulture
    /// 
    /// AppCultures, (for example by settings in a DB or detecting)
    /// IAppCultureService AppCultureService
    /// 
    /// Translation directories and files
    /// LocalizationFileCollection LocalizationFiles
    /// 
    /// Getting translations
    /// ILocalizedStringService LocalizedStringService
    /// 
    /// Settings:
    /// 
    /// 1. Default Culture, Single setting (CultureInfo)
    /// 2. Culture selectors, UserCultureProvider with Culture Selectors
    /// 3. Localization Directories and files, AppCultureProvider with Access to translations and the app cultures, Default Culture Included
    /// 
    /// Statics:
    /// - GetAppLanguages(), retrieved by scanning the LocalizationDirectories
    /// - GetUserCulture(HttpContextBase httpContext), retrieved by getting the best match between the culture selectors and the AppLanguages
    /// 
    /// - LocalizerFor(Type scope)
    /// - LocalizerFor(string scope)
    /// 
    /// - InjectLocalizer(object instance)
    /// 
    /// - GetText(string context, string textHint, params object[] args)
    /// 
    /// Logic Flow:
    ///     InjectT()
    /// 
    ///     GetText()
    ///     
    ///     GetUserCultures
    ///     Match with AppCultures
    ///     If match, set user culture
    ///     else set to default culture
    ///     
    ///     load user culture
    ///     return localizedString
    /// </summary>
    public class CapitalTConfiguration
    {
        public CapitalTConfiguration()
        {
            UserCultureSelectors = new List<IUserCultureSelector>
            {
                new HttpHeaderCultureSelector(),
                new CookieCultureSelector(),
                new ClaimsCultureSelector()
            };
            
            NeutralAppCulture = CultureInfo.GetCultureInfo("en");
            
            LocalizationFiles = new LocalizationFileCollection();
            AppCultureProvider = new ScanningAppCultureProvider(LocalizationFiles);
            LocalizedStringService = new LocalizedStringService(LocalizationFiles);
        }

        public ICollection<IUserCultureSelector> UserCultureSelectors { get; private set; }

        public CultureInfo NeutralAppCulture { get; set; }

        public IAppCultureProvider AppCultureProvider { get; private set; }

        public LocalizationFileCollection LocalizationFiles { get; private set; }

        public ILocalizedStringService LocalizedStringService { get; private set; }
    }
}
