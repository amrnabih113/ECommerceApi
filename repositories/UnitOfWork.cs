using ECommerce.Data;
using ECommerce.Interfaces.Repositories;

namespace ECommerce.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _context;
        public IProductsRepository Products { get; private set; }
        public ICategoriesRepository Categories { get; private set; }
        public IBrandsRepository Brands { get; private set; }
        public ICartsRepository Carts { get; private set; }
        public ICartItemsRepository CartItems { get; private set; }
        public IReviewsRepository Reviews { get; private set; }
        public IProductImagesRepository ProductImages { get; private set; }
        public IProductVariantsRepository ProductVariants { get; private set; }
        public IWishListRepository WishList { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Products = new ProductsRepository(_context);
            Categories = new CategoriesRepository(_context);
            Brands = new BrandsRepository(_context);
            Carts = new CartsRepository(_context);
            CartItems = new CartItemsRepository(_context);
            Reviews = new ReviewsRepository(_context);
            ProductImages = new ProductImagesRepository(_context);
            ProductVariants = new ProductVariantsRepository(_context);
            WishList = new WishListRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
