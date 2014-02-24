using CapitalT.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CapitalT.Culture
{
    public class HttpHeaderCultureSelector : IUserCultureSelector
    {
        public IEnumerable<CultureSelectorResult> GetCultures(HttpContextBase httpContext)
        {
            var userLanguages = httpContext.Request.UserLanguages;
            foreach (var userLanguage in userLanguages)
            {
                var languageQuality = userLanguage.Split(';');
                var language = languageQuality.Length >= 1 ? languageQuality[0] : null;
                if (language != null)
                {
                    CultureInfo culture = null;
                    var quality = 0.0f;
                    try
                    {
                        culture = CultureInfo.GetCultureInfo(language);

                        if (languageQuality.Length >= 2)
                        {
                            var qualityString = languageQuality[1].Split('=').LastOrDefault();
                            float parsedQuality;
                            if (float.TryParse(qualityString, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedQuality))
                            {
                                quality = parsedQuality;
                            }
                        }
                        else
                        {
                            quality = 1.0f;
                        }
                    }
                    catch (CultureNotFoundException) { }
                    if (culture != null)
                    {
                        yield return new CultureSelectorResult
                        {
                            Culture = culture,
                            Priority = Priority,
                            Quality = quality
                        };
                    }
                }
            }
        }

        public int Priority
        {
            get { return -2; }
        }
    }
}
