using CapitalT.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CapitalT
{
    public class Text
    {
        private readonly string _scope;
        private readonly CapitalTConfiguration _configuration;

        public Text(string scope, CapitalTConfiguration configuration)
        {
            this._scope = scope;
            this._configuration = configuration;
        }

        public LocalizedString Get(string textHint, params object[] args)
        {
            var culture = LocalizedApplication.GetUserCulture(HttpContext.Current.GetHttpContextBase());
            var localizedFormat = _configuration.LocalizedStringService.GetLocalizedString(_scope, textHint, culture);

            return args.Length == 0
                ? new LocalizedString(localizedFormat, _scope, textHint, args)
                : new LocalizedString(string.Format(culture, localizedFormat, args), _scope, textHint, args);
        }
    }
}
