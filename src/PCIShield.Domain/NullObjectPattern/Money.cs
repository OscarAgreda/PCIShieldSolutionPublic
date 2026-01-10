using System;
namespace PCIShield.Domain.NullObject
{
    public record Money : IComparable<Money>
    {
        public decimal Amount { get; }
        public Currency Currency { get; }
        public static Money Zero { get; } = new(0, Currency.Empty);
        private bool IsZero => Amount == 0 && Currency == Currency.Empty;
        public Money(decimal amount, Currency currency)
        {
            if (amount < 0)
            {
                throw new InvalidOperationException("Money amount cannot be negative");
            }
            Amount = Math.Round(amount, 2);
            Currency = currency;
        }
        public Money Add(Money other)
        {
            return other.IsZero ? this
            : IsZero ? other
            : Currency == other.Currency ? new Money(Amount + other.Amount, Currency)
            : throw new InvalidOperationException("Cannot add money of different currencies");
        }
        public Money Subtract(Money other)
        {
            return other.IsZero ? this
            : IsZero ? throw new InvalidOperationException("Cannot sutract from zero")
            : other.Currency != Currency ? throw new ArgumentException("Cannot subtract different currencies")
            : other.Amount > Amount ? throw new ArgumentException("Not enough funds")
            : other.Amount == Amount ? Money.Zero
            : new(Amount - other.Amount, Currency);
        }
        public Money Scale(decimal factor)
        {
            return IsZero ? this
            : factor < 0 ? throw new InvalidOperationException("Cannot multiply by a negative factor")
            : new Money(Amount * factor, Currency);
        }
        public int CompareTo(Money? other)
        {
            return other is null ? 1
            : other.IsZero || IsZero ? Amount.CompareTo(other.Amount)
            : Currency == other.Currency ? Amount.CompareTo(other.Amount)
            : throw new InvalidOperationException("Cannot compare money of different currencies");
        }
        public override string ToString()
        {
            return IsZero ? "0.00" : $"{Amount:0.00} {Currency.Symbol}";
        }
    }
    public record struct Currency(string Symbol)
    {
        public Currency()
            : this(string.Empty) { }
        internal static Currency Empty => new(string.Empty);
        public static readonly Currency EUR = new("EUR");
        public static readonly Currency USD = new("USD");
        public Money Amount(decimal amount)
        {
            return new(amount, this);
        }
        public override string ToString()
        {
            return Symbol;
        }
    }
}