namespace ECommerce.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductsRepository Products { get; }
        ICategoriesRepository Categories { get; }

        IBrandsRepository Brands { get; }

        ICartsRepository Carts { get; }

        ICartItemsRepository CartItems { get; }

        IReviewsRepository Reviews { get; }

        IProductImagesRepository ProductImages { get; }

        IProductVariantsRepository ProductVariants { get; }

        IWishListRepository WishList { get; }

        IAddressesRepository Addresses { get; }

        IBannersRepository Banners { get; }

        ICouponsRepository Coupons { get; }

        IUserCouponsRepository UserCoupons { get; }

        IOrdersRepository Orders { get; }

        Task<int> CompleteAsync();
    }
}