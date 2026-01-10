using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using PCIShieldLib.SharedKernel.Interfaces;
namespace PCIShield.Infrastructure.Helpers
{
    public class EntityChangedEvent<T> : INotification where T : class, IAggregateRoot
    {
        public EntityChangedEvent(T entity)
        {
            this.Entity = entity;
        }
        public T Entity { get; }
    }
}