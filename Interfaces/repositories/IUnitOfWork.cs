namespace ECommerce.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductsRepository Products { get; }
        ICategoriesRepository Categories { get; }

        IBrandsRepository Brands { get; }

        Task<int> CompleteAsync();
    }
}