using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using PCIShieldLib.SharedKernel;
using Microsoft.EntityFrameworkCore;
namespace PCIShield.Domain.ValueObjects.CustomerValueObjects
{
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimaryEmailVobj : ValueObject
    {
        public string Email { get; }
        private CustomerPrimaryEmailVobj() { }
        public CustomerPrimaryEmailVobj(string email)
        {
            Guard.Against.NullOrEmpty(email, nameof(email));
            Email = email;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Email.ToLowerInvariant();
        }
        public override string ToString() => Email;
    }
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimaryPhoneVobj : ValueObject
    {
        public string PhoneNumber { get; }
        private CustomerPrimaryPhoneVobj() { }
        public CustomerPrimaryPhoneVobj(string phoneNumber)
        {
            Guard.Against.NullOrEmpty(phoneNumber, nameof(phoneNumber));
            PhoneNumber = phoneNumber;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PhoneNumber;
        }
        public override string ToString() => PhoneNumber;
    }
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimaryDocumentIdVobj : ValueObject
    {
        public string DocumentNumber { get; }
        private CustomerPrimaryDocumentIdVobj() { }
        public CustomerPrimaryDocumentIdVobj(string docNumber)
        {
            Guard.Against.NullOrEmpty(docNumber, nameof(docNumber));
            DocumentNumber = docNumber;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DocumentNumber;
        }
        public override string ToString() => DocumentNumber;
    }
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimaryAddressLineVobj : ValueObject
    {
        public string AddressLine { get; }
        private CustomerPrimaryAddressLineVobj() { }
        public CustomerPrimaryAddressLineVobj(string addressLine)
        {
            Guard.Against.NullOrEmpty(addressLine, nameof(addressLine));
            AddressLine = addressLine;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return AddressLine;
        }
        public override string ToString() => AddressLine;
    }
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimaryPostalCodeVobj : ValueObject
    {
        public string PostalCode { get; }
        private CustomerPrimaryPostalCodeVobj() { }
        public CustomerPrimaryPostalCodeVobj(string postalCode)
        {
            Guard.Against.NullOrEmpty(postalCode, nameof(postalCode));
            PostalCode = postalCode;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PostalCode;
        }
        public override string ToString() => PostalCode;
    }
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimaryContactFullNameVobj : ValueObject
    {
        public string FullName { get; }
        private CustomerPrimaryContactFullNameVobj() { }
        public CustomerPrimaryContactFullNameVobj(string fullName)
        {
            Guard.Against.NullOrEmpty(fullName, nameof(fullName));
            FullName = fullName;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FullName;
        }
        public override string ToString() => FullName;
    }
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimaryContactEmailVobj : ValueObject
    {
        public string Email { get; }
        private CustomerPrimaryContactEmailVobj() { }
        public CustomerPrimaryContactEmailVobj(string email)
        {
            Guard.Against.NullOrEmpty(email, nameof(email));
            Email = email;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Email.ToLowerInvariant();
        }
        public override string ToString() => Email;
    }
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimarySalespersonNameVobj : ValueObject
    {
        public string SalespersonName { get; }
        private CustomerPrimarySalespersonNameVobj() { }
        public CustomerPrimarySalespersonNameVobj(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            SalespersonName = name;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return SalespersonName;
        }
        public override string ToString() => SalespersonName;
    }
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimaryEconomicActivityCodeVobj : ValueObject
    {
        public string ActivityCode { get; }
        private CustomerPrimaryEconomicActivityCodeVobj() { }
        public CustomerPrimaryEconomicActivityCodeVobj(string code)
        {
            Guard.Against.NullOrEmpty(code, nameof(code));
            ActivityCode = code;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ActivityCode;
        }
        public override string ToString() => ActivityCode;
    }
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimaryEconomicActivityNameVobj : ValueObject
    {
        public string ActivityName { get; }
        private CustomerPrimaryEconomicActivityNameVobj() { }
        public CustomerPrimaryEconomicActivityNameVobj(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            ActivityName = name;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ActivityName;
        }
        public override string ToString() => ActivityName;
    }
    [Serializable]
    [Keyless]
    [NotMapped]
    public class CustomerPrimaryTaxSystemTypeNameVobj : ValueObject
    {
        public string TaxSystemTypeName { get; }
        private CustomerPrimaryTaxSystemTypeNameVobj() { }
        public CustomerPrimaryTaxSystemTypeNameVobj(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            TaxSystemTypeName = name;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TaxSystemTypeName;
        }
        public override string ToString() => TaxSystemTypeName;
    }
}
