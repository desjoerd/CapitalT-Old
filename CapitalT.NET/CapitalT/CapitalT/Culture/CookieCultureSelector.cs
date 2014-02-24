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
    public class CookieCultureSelector : IUserCultureSelector
    {
        public IEnumerable<CultureSelectorResult> GetCultures(HttpContextBase httpContext)
        {
            var cultureCookie = httpContext.Request.Cookies["CapitalT.CultureName"];
            if (cultureCookie != null)
            {
                CultureSelectorResult result = null;
                try
                {
                    var culture = CultureInfo.GetCultureInfo(cultureCookie.Value);
                    result = new CultureSelectorResult
                    {
                        Culture = culture,
                        Priority = -1
                    };
                }
                catch (CultureNotFoundException) { }
                
                if (result != null)
                {
                    yield return result;
                }
            }
        }
    }
}
