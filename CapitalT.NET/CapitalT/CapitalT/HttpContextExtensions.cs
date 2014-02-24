using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CapitalT
{
    public static class HttpContextExtensions
    {
        internal const string HttpContextBaseKey = "CapitalT.HttpContextBase";

        public static HttpContextBase GetHttpContextBase(this HttpContext context)
        {
            HttpContextBase hcb = context.Items[HttpContextBaseKey] as HttpContextBase;
            if (hcb == null)
            {
                context.Items[HttpContextBaseKey]
                    = hcb
                    = new HttpContextWrapper(context);
            }
            return hcb;
        }

        public static Localizer GetLocalizer<TScope>(this HttpContext context)
        {
            return GetLocalizer<TScope>(context.GetHttpContextBase());
        }

        public static Localizer GetLocalizer(this HttpContext context, Type scope)
        {
            return GetLocalizer(context.GetHttpContextBase(), scope);
        }

        public static Localizer GetLocalizer<TScope>(this HttpContextBase context)
        {
            return GetLocalizer(context, typeof(TScope));
        }

        public static Localizer GetLocalizer(this HttpContextBase context, Type scope)
        {
            return LocalizedApplication.GetLocalizer(scope);
        }

        public static CultureInfo UserCulture(this HttpContext context)
        {
            return UserCulture(context.GetHttpContextBase());
        }

        public static CultureInfo UserCulture(this HttpContextBase context)
        {
            return LocalizedApplication.GetUserCulture(context);
        }
    }
}
