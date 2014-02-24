using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Services
{
    /// <summary>
    /// No current requeststate
    /// </summary>
    public interface ILocalizedStringService
    {
        string GetLocalizedString(string scope, string textHint, CultureInfo culture);
    }
}
