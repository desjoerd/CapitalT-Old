using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CapitalT.Culture
{
    public class ClaimsCultureSelector : IUserCultureSelector
    {
        public IEnumerable<CultureSelectorResult> GetCultures(HttpContextBase httpContext)
        {
            if (System.Security.Claims.ClaimsPrincipal.Current == null)
            {
                yield break;
            }

            var cultureClaim = System.Security.Claims.ClaimsPrincipal.Current.Claims.Where(c => c.Type == "urn:CapitalT:Culture").FirstOrDefault();
            if (cultureClaim != null)
            {
                CultureInfo culture = null;
                try
                {
                    culture = new CultureInfo(cultureClaim.Value);
                }
                catch (CultureNotFoundException) { }

                if (culture != null)
                {
                    yield return new CultureSelectorResult
                    {
                        Culture = culture,
                        Priority = 0
                    };
                }
            }
        }
    }
}
