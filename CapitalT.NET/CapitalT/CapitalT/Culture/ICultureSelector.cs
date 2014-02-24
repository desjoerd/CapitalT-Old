using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CapitalT.Culture
{
    public class CultureSelectorResult
    {
        public int Priority { get; set; }
        public float Quality { get; set; }
        public CultureInfo Culture { get; set; }
    }
    
    public interface IUserCultureSelector
    {
        IEnumerable<CultureSelectorResult> GetCultures(HttpContextBase httpContext);
    }
}
