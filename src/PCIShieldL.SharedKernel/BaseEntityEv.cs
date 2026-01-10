using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace PCIShieldLib.SharedKernel
{
    public abstract class BaseEntityEv<TId>
    {
        [NotMapped]
        public List<BaseDomainEvent> Events = new();
       [NotMapped]
        public TId Id { get; set; }
    }
}