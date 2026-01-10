using FluentValidation;
using System;
using System.Linq;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models
{
    public static class Vali
    {
        #region Helper Methods
        public static bool BeValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
        public static bool IsBase64String(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return false;
            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool ValidatePhoneNumberByCountry(string phone, string countryCode)
        {
            if (string.IsNullOrEmpty(phone)) return true;
            return true;
        }
        public static bool ValidatePostalCodeByCountry(string postalCode, string countryCode)
        {
            if (string.IsNullOrEmpty(postalCode)) return true;
            return true;
        }
        public static bool HasDuplicateJoinEntries<T>(IEnumerable<T> items) where T : class
        {
            if (items == null) return false;
            return false;
        }
        public static bool ValidateLookupReference(Guid id)
        {
            return true;
        }
        public static bool ValidateLookupReference(Guid? id)
        {
            return true;
        }
        public static bool ValidateParentReference(Guid id)
        {
            return true;
        }
        public static bool ValidateParentReference(Guid? id)
        {
            return true;
        }
        public static bool ValidateParentStatus(object parent)
        {
            var type = typeof(object);
            return true;
        }
        #endregion
    }
}