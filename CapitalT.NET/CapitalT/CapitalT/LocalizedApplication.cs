using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CapitalT
{
    /// <summary>
    /// Main point for CapitalT
    /// </summary>
    public static class LocalizedApplication
    {
        internal const string UserCultureKey = "CapitalT.UserCulture";

        private static readonly ConcurrentDictionary<string, Localizer> _localizerCache = new ConcurrentDictionary<string,Localizer>();

        private static readonly object _configurationLockObject = new object();
        private static CapitalTConfiguration _configuration;
        public static CapitalTConfiguration Configuration
        {
            get { return _configuration; }
            set
            {
                lock (_configurationLockObject)
                {
                    _localizerCache.Clear();
                    _configuration = value;
                }
            }
        }

        /// <summary>
        /// Creates a new default configuration and calls the callback to further configure the localization
        /// </summary>
        /// <param name="configureCallback"></param>
        public static void Configure(Action<CapitalTConfiguration> configureCallback)
        {
            if (configureCallback == null)
            {
                throw new ArgumentNullException("configureCallback");
            }
            var config = new CapitalTConfiguration();
            configureCallback(config);
            Configuration = config;
        }

        #region GetLocalizer
        public static Localizer GetLocalizer(string scope)
        {
            return _localizerCache.GetOrAdd(scope, (s) => new Text(s, Configuration).Get);
        }

        public static Localizer GetLocalizer<TScope>()
        {
            return GetLocalizer(typeof(TScope));
        }

        public static Localizer GetLocalizer(Type scope)
        {
            return GetLocalizer(scope.FullName);
        }
        #endregion

        
        public static CultureInfo GetUserCulture(HttpContextBase httpContext)
        {
            var userCulture = httpContext.Items[UserCultureKey] as CultureInfo;
            if (userCulture == null)
            {
                var config = Configuration;

                var cultureSelectors = config.UserCultureSelectors;
                var appCultureProvider = config.AppCultureProvider;

                var userCultureResults = cultureSelectors
                    .SelectMany(selector => selector.GetCultures(httpContext))
                    .OrderByDescending(result => result.Priority)
                    .ThenByDescending(result => result.Quality)
                    .ToList();

                var appCultures = appCultureProvider.GetAppCultures();

                foreach (var result in userCultureResults)
                {
                    if (appCultures.Contains(result.Culture))
                    {
                        userCulture = result.Culture;
                        break;
                    }
                }
                if (userCulture == null)
                {
                    // no match, maybe extend this later to try parent cultures
                    userCulture = Configuration.NeutralAppCulture;
                }

                httpContext.Items[UserCultureKey] = userCulture;
            }
            return userCulture;
        }
    }
}
