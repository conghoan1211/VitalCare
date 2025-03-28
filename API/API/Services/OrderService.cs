using static API.Helper.Common;
using API.Models;
using API.ViewModels;
using AutoMapper;
using InstagramClone.Utilities;
using Microsoft.EntityFrameworkCore;
using API.Helper;
using static System.Net.WebRequestMethods;

namespace API.Services
{
    public interface IOrderService
    {
        public Task<(string, OrderVM?)> CreateOrder(InsertOrderVM? input);
        public Task<(string, List<OrderVM>?)> GetAll();
        public Task<string> SetStatus(string orderId, int status);
        public Task<(string, List<OrderVM>?)> GetOrderByUserId(string userId);

    }
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly Exe201Context _context;
        private readonly IProfileService _profileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(IMapper mapper, Exe201Context context, IProfileService profileService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _profileService = profileService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<(string, OrderVM?)> CreateOrder(InsertOrderVM? input)
        {
            if (input == null) return ("Dữ liệu đầu vào không hợp lệ.", null);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order
                    {
                        OrderId = GenerateOrderId(),
                        UserId = input.UserID,
                        Username = input.UserName,
                        Phone = input.Phone,
                        Email = input.Email,
                        Address = $"{input.DistrictName} - {input.ProvinceName}",
                        SpecificAddress = input.SpecificAddress,
                        Note = input.Note,
                        Status = (int)OrderStatus.Pending,
                        PaymentMethod = input.PaymentMethod,
                        Total = input.Total,
                        ShipPrice = input.ShipPrice,
                        CreatedAt = DateTime.UtcNow,
                        OrderDate = DateTime.UtcNow
                    };
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    foreach (var product in input.Products)
                    {
                        var productData = await _context.Products.FindAsync(product.ProductID);
                        if (productData == null)
                        {
                            throw new Exception($"Sản phẩm {product.ProductID} không tồn tại.");
                        }
                        // 🔴 Kiểm tra nếu stock không đủ
                        if (productData.Stock < product.Quantity)
                        {
                            throw new Exception($"Sản phẩm {product.Title} chỉ còn {productData.Stock} trong kho, không đủ để đặt hàng.");
                        }
                        var orderDetail = new OrderDetail
                        {
                            DetailId = Guid.NewGuid().ToString(),
                            OrderId = order.OrderId,
                            ProductId = product.ProductID,
                            Quantity = product.Quantity,
                            Title = product.Title,
                            CategoryName = product.CategoryName,
                            CreateAt = order.CreatedAt,
                            CurrentPrice = product.CurrentPrice,
                            NewPrice = product.NewPrice,
                            ProductUrl = productData.ProductUrl,
                            Total = product.Quantity * (int)product.NewPrice,
                        };

                        productData.Stock -= product.Quantity;
                        productData.Sold += product.Quantity;
                        productData.Status = productData.Stock > 0 ? (int)ProductStatus.Available : (int)ProductStatus.OutOfStock;

                        _context.Products.Update(productData);
                        await _context.OrderDetails.AddAsync(orderDetail);
                    }
                    await _context.SaveChangesAsync();

                    // 👉 Cập nhật profile người dùng sau khi đặt hàng thành công
                    var updatedProfile = new UpdateProfileModels
                    {
                        UserID = input.UserID,
                        UserName = input.UserName,
                        Phone = input.Phone,
                        Email = input.Email,
                        Address = $"{input.DistrictName} - {input.ProvinceName}",
                        ProvinceId = input.ProvinceId,
                        DistrictId = input.DistrictId,
                        ProvinceName = input.ProvinceName,
                        DistrictName = input.DistrictName,
                    };

                    await _profileService.UpdateProfile(updatedProfile, _httpContextAccessor.HttpContext); // 👈 gọi service update profile

                    await transaction.CommitAsync();

                    OrderVM orderVm = new OrderVM
                    {
                        OrderId = order.OrderId,
                        Address = order.Address,
                        SpecificAddress = order.SpecificAddress,
                        CreatedAt = order.CreatedAt,
                        Email = order.Email,
                        Note = order.Note,
                        OrderDate = order.OrderDate,
                        PaymentMethod = order.PaymentMethod,
                        Phone = order.Phone,
                        ShipPrice = order.ShipPrice,
                        Status = order.Status,
                        Total = order.Total,
                        UserID = order.UserId,
                        UserName = order.Username,
                        Products = input.Products
                    };

                    return (string.Empty, orderVm);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ($"Lỗi khi tạo đơn hàng: {ex.Message}", null);
                }
            }
        }

        public async Task<(string, List<OrderVM>?)> GetAll()
        {
            var orders = await _context.Orders
                .OrderByDescending(x => x.CreatedAt)
                .Select(order => new OrderVM
                {
                    OrderId = order.OrderId,
                    UserID = order.UserId,
                    UserName = order.Username,
                    Phone = order.Phone,
                    Email = order.Email,
                    Address = order.Address,
                    SpecificAddress = order.SpecificAddress,
                    Note = order.Note,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod,
                    Total = order.Total,
                    ShipPrice = order.ShipPrice,
                    CreatedAt = order.CreatedAt,
                    OrderDate = order.OrderDate,
                    Products = order.OrderDetails.Select(detail => new OrderDetailVM
                    {
                        ProductID = detail.ProductId,
                        Title = detail.Product.Title,
                        CategoryName = detail.Product.Category.Name,
                        CurrentPrice = detail.CurrentPrice,
                        NewPrice = detail.NewPrice,
                        ProductUrl = detail.Product.ProductUrl,
                        Quantity = detail.Quantity,
                        TotalPrice = detail.Total
                    }).ToList()
                }).ToListAsync();

            if (orders == null || orders.Count == 0)
                return ("Không tìm thấy đơn hàng nào.", null);

            return ("", orders);
        }
        public async Task<string> SetStatus(string orderId, int status)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (order == null) return "Không tìm thấy đơn hàng.";
            order.Status = status;

            if (status == (int)OrderStatus.Cancelled)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = orderDetail.Product;
                    if (product != null)
                    {
                        product.Stock += orderDetail.Quantity;
                        if (product.Sold >= orderDetail.Quantity) product.Sold -= orderDetail.Quantity;

                        product.Status = product.Stock > 0 ? (int)ProductStatus.Available : (int)ProductStatus.OutOfStock;
                        _context.Products.Update(product);
                    }
                }
            }
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<(string, List<OrderVM>?)> GetOrderByUserId(string userId)
        {
            var orders = await _context.Orders
                 .Where(x => x.UserId == userId)
                 .OrderByDescending(x => x.CreatedAt)
                 .Select(order => new OrderVM
                 {
                     OrderId = order.OrderId,
                     UserID = order.UserId,
                     UserName = order.Username,
                     Phone = order.Phone,
                     Email = order.Email,
                     Address = order.Address,
                     SpecificAddress = order.SpecificAddress,
                     Note = order.Note,
                     Status = order.Status,
                     PaymentMethod = order.PaymentMethod,
                     Total = order.Total,
                     ShipPrice = order.ShipPrice,
                     CreatedAt = order.CreatedAt,
                     OrderDate = order.OrderDate,
                     Products = order.OrderDetails.Select(detail => new OrderDetailVM
                     {
                         ProductID = detail.ProductId,
                         Title = detail.Product.Title,
                         CategoryName = detail.Product.Category.Name,
                         CurrentPrice = detail.CurrentPrice,
                         NewPrice = detail.NewPrice,
                         ProductUrl = detail.Product.ProductUrl,
                         Quantity = detail.Quantity,
                         TotalPrice = detail.Total
                     }).ToList()
                 }).ToListAsync();

            if (orders == null || orders.Count == 0) return ("Không tìm thấy đơn hàng.", null);

            return ("", orders);
        }

    }
}
