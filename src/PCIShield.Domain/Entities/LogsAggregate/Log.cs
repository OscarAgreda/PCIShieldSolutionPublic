using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Ardalis.GuardClauses;
using System.Collections.Immutable;
using System.Text.Json;
using LanguageExt;
using static LanguageExt.Prelude;
using PCIShield.Domain.Exceptions;
using PCIShield.Domain.Events;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;

namespace PCIShield.Domain.Entities
{
    
    public class Logs : BaseEntityEv<int>, IAggregateRoot
    {
        [Key]
        public int Id { get; private set; }
        
        public string? Message { get; private set; }
        
        public string? MessageTemplate { get; private set; }
        
        public string? Level { get; private set; }
        
        public string? Exception { get; private set; }
        
        public string? Properties { get; private set; }
        
        public void SetId(int id)
        {
            Id = Guard.Against.Negative(id, nameof(id));
        }
        public void SetMessage(string message)
        {
            Message = message;
        }
        public void SetMessageTemplate(string messageTemplate)
        {
            MessageTemplate = messageTemplate;
        }
        public void SetLevel(string level)
        {
            Level = level;
        }
        public void SetException(string exception)
        {
            Exception = exception;
        }
        public void SetProperties(string properties)
        {
            Properties = properties;
        }
        private Logs() { }
        public override bool Equals(object? obj) =>
        obj is Logs logs && Equals(logs);
        
        public bool Equals(Logs other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return Id.Equals(other.Id);
        }
        
        public override int GetHashCode() => Id.GetHashCode();
        
        public static bool operator !=(Logs left, Logs right) => !(left == right);
        
        public static bool operator ==(Logs left, Logs right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (Message?.Length > -1)
            throw new InvalidOperationException("Message cannot exceed -1 characters.");
            if (MessageTemplate?.Length > -1)
            throw new InvalidOperationException("MessageTemplate cannot exceed -1 characters.");
            if (Level?.Length > -1)
            throw new InvalidOperationException("Level cannot exceed -1 characters.");
            if (Exception?.Length > -1)
            throw new InvalidOperationException("Exception cannot exceed -1 characters.");
            if (Properties?.Length > -1)
            throw new InvalidOperationException("Properties cannot exceed -1 characters.");
        }
        
    }
}

