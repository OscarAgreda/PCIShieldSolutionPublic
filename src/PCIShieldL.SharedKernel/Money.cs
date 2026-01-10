using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
namespace PCIShieldLib.SharedKernel
{
    public class MoneyValue : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }
        public MoneyValue(decimal amount, string currency)
        {
             Guard.Against.Negative(amount, nameof(amount));
            Guard.Against.NullOrWhiteSpace(currency, nameof(currency));
            Amount = amount;
            Currency = currency;
        }
        public MoneyValue Add(MoneyValue summand)
        {
            if (Currency != summand.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");
            return new MoneyValue(Amount + summand.Amount, Currency);
        }
        public MoneyValue Subtract(MoneyValue subtrahend)
        {
            if (Currency != subtrahend.Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");
            return new MoneyValue(Amount - subtrahend.Amount, Currency);
        }
        public static MoneyValue operator +(MoneyValue a, MoneyValue b) => a.Add(b);
        public static MoneyValue operator -(MoneyValue a, MoneyValue b) => a.Subtract(b);
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
        public override string ToString() => $"{Amount} {Currency}";
    }
    public class AddressValue : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string ZipCode { get; }
        public AddressValue(string street, string city, string state, string country, string zipCode)
        {
            Guard.Against.NullOrWhiteSpace(street, nameof(street));
            Guard.Against.NullOrWhiteSpace(city, nameof(city));
            Guard.Against.NullOrWhiteSpace(state, nameof(state));
            Guard.Against.NullOrWhiteSpace(country, nameof(country));
            Guard.Against.NullOrWhiteSpace(zipCode, nameof(zipCode));
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }
        public override string ToString() => $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }
}