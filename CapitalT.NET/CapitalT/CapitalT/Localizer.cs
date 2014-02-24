﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT
{
    public delegate LocalizedString Localizer(string text, params object[] args);

    public static class LocalizerExtensions
    {
        public static LocalizedString Plural(this Localizer T, string textSingular, string textPlural, int count, params object[] args)
        {
            return T(count == 1 ? textSingular : textPlural, new object[] { count }.Concat(args).ToArray());
        }

        public static LocalizedString Plural(this Localizer T, string textNone, string textSingular, string textPlural, int count, params object[] args)
        {
            switch (count)
            {
                case 0:
                    return T(textNone, new object[] { count }.Concat(args).ToArray());
                case 1:
                    return T(textSingular, new object[] { count }.Concat(args).ToArray());
                default:
                    return T(textPlural, new object[] { count }.Concat(args).ToArray());
            }
        }
    }
}
