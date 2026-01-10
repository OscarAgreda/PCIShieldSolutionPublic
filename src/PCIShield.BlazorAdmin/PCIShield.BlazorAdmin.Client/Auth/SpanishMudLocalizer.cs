using PCIShield.Client.Services.Auth;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Globalization;

namespace PCIShield.BlazorAdmin.Client.Auth
{
    public class SpanishMudLocalizer : MudLocalizer
    {
        private readonly IHttpAppLocalizationClientService _localizationClient;

        public SpanishMudLocalizer(IHttpAppLocalizationClientService localizationClient)
        {
            _localizationClient = localizationClient ?? throw new ArgumentNullException(nameof(localizationClient));
        }

        public override LocalizedString this[string key]
        {
            get
            {
                if (CultureInfo.CurrentUICulture.Name.StartsWith("en"))
                {
                    return new LocalizedString(key, key, resourceNotFound: false);
                }

                var result = _localizationClient.GetStringAsync(key, CultureInfo.CurrentUICulture.Name)
                    .GetAwaiter().GetResult();

                return result.Match(
                    Right: value => new LocalizedString(key, value, resourceNotFound: false),
                    Left: _ => new LocalizedString(key, key, resourceNotFound: true)
                );
            }
        }
    }
}