using System;
using System.Globalization;
using System.Threading.Tasks;

using LanguageExt;

using Microsoft.Extensions.Localization;
using MudBlazor;

using static LanguageExt.Prelude;

namespace PCIShield.BlazorAdmin.Client.Auth
{
    public class MudLocalizerService : MudLocalizer
    {
        private readonly IClientLocalizationService _locService;

        public MudLocalizerService(IClientLocalizationService locService)
        {
            _locService = locService;
        }

        public override LocalizedString this[string key]
        {
            get
            {
                if (CultureInfo.CurrentUICulture.Name.StartsWith("en"))
                {
                    return new LocalizedString(key, key, resourceNotFound: false);
                }

                string text = _locService.GetStringAsync(key).GetAwaiter().GetResult();
                bool notFound = text == $"[{key}]";

                if (notFound)
                {
                    return new LocalizedString(key, key, resourceNotFound: true);
                }

                return new LocalizedString(key, text, resourceNotFound: false);
            }
        }
    }
}
