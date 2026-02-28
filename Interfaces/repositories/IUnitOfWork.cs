using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Data;
using ECommerce.Repositories;

namespace ECommerce.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductsRepository Products { get; }

        Task<int> CompleteAsync();
    }
}