using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PCIShieldLib.SharedKernel.Interfaces;
using Z.EntityFramework.Plus;
namespace PCIShield.Infrastructure.Data
{
    public class EfAuthRepository<T> : RepositoryBase<T>, IAuthRepository<T> where T : class, IAuthEntity
    {
        private readonly DbContext _dbContext;
        private IDbContextTransaction _transaction;
        public EfAuthRepository(AuthorizationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    
    }
}