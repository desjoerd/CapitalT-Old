using Autofac;
using Autofac.Core;
using CapitalT.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Module = Autofac.Module;

namespace CapitalT
{
    public class LocalizationModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var capitalTProperty = FindCapitalTProperty(registration.Activator.LimitType);

            if (capitalTProperty != null)
            {
                var scope = registration.Activator.LimitType.FullName;

                registration.Activated += (sender, e) =>
                {
                    var localizer = LocalizedApplication.GetLocalizer(scope);
                    capitalTProperty.SetValue(e.Instance, localizer, null);
                };
            }
        }

        private static PropertyInfo FindCapitalTProperty(Type type)
        {
            return type.GetProperty("T", typeof(Localizer));
        }
    }
}
