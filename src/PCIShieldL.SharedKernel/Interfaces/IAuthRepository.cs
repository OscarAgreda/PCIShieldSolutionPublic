using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Ardalis.GuardClauses;
using Ardalis.Specification;

namespace PCIShieldLib.SharedKernel.Interfaces
{

    public interface IAuthRepository<T> : IRepositoryBase<T> where T : class, IAuthEntity
    {

    }
}