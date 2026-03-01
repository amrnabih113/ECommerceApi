namespace ECommerce.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductsRepository Products { get; }
        ICategoriesRepository Categories { get; }

        IBrandsRepository Brands { get; }

        ICartsRepository Carts { get; }

        ICartItemsRepository CartItems { get; }

        IProductVariantsRepository ProductVariants { get; }

        Task<int> CompleteAsync();
    }
}