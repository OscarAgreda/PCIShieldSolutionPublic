using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PCIShieldLib.SharedKernel.Interfaces;
namespace PCIShield.Infrastructure.Helpers
{
    public interface IRepositoryFactory
    {
        IRepository<T> GetRepository<T>() where T : class, IAggregateRoot;
    }
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly ILifetimeScope _scope;
        public RepositoryFactory(ILifetimeScope scope)
        {
            _scope = scope;
        }
        public IRepository<T> GetRepository<T>() where T : class, IAggregateRoot
        {
            return _scope.Resolve<IRepository<T>>();
        }
    }
    public interface IReadRedisRepositoryFactory
    {
        IReadRedisRepository<T> GetReadRedisRepository<T>() where T : class, IAggregateRoot;
    }
    public class ReadRedisRepositoryFactory : IReadRedisRepositoryFactory
    {
        private readonly ILifetimeScope _scope;
        public ReadRedisRepositoryFactory(ILifetimeScope scope)
        {
            _scope = scope;
        }
        public IReadRedisRepository<T> GetReadRedisRepository<T>() where T : class, IAggregateRoot
        {
            return _scope.Resolve<IReadRedisRepository<T>>();
        }
    }
}
