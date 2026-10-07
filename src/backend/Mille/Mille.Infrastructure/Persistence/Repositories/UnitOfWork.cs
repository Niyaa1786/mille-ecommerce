using Mille.Application.Common.Interfaces;
using Mille.Domain.Interfaces;
using Mille.Infrastructure.Persistence.Data;

namespace Mille.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IUserRepository _userRepository;
        private ICategoryRepository _categoryRepository;
        private IProductRepository _productRepository;
        private IProductVariantRepository _productVariantRepository;
        private IProductImageRepository _productImageRepository;
        private ICartRepository _cartRepository;
        private IOrderRepository _orderRepository;
        private IPaymentRepository _paymentRepository;
        private ICouponRepository _couponRepository;
        private IReviewRepository _reviewRepository;

        public UnitOfWork(AppDbContext context) => _context = context;

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);
        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
        public IProductVariantRepository ProductVariants => _productVariantRepository ??= new ProductVariantRepository(_context);
        public IProductImageRepository ProductImages => _productImageRepository ??= new ProductImageRepository(_context);
        public ICartRepository Carts => _cartRepository ??= new CartRepository(_context);
        public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);
        public IPaymentRepository Payments => _paymentRepository ??= new PaymentRepository(_context);
        public ICouponRepository Coupons => _couponRepository ??= new CouponRepository(_context);
        public IReviewRepository Reviews => _reviewRepository ??= new ReviewRepository(_context);

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
    }
}
