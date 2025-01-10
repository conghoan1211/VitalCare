﻿using API.Common;
using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using InstagramClone.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public interface IProductService
    {
        public Task<(string, List<ProductListVM>?)> GetList();
        public Task<(string, ProductDetailVM?)> GetDetail(string productId);
        public Task<string> DoInsertUpdate(InsertUpdateProductVM? input, string userId);
        public Task<string> DoToggleActive(string productId, bool active);
    }


    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly Exe201Context _context;
        private readonly IAmazonS3Service _s3Service;

        public ProductService(IMapper mapper, Exe201Context context, IAmazonS3Service s3Service)
        {
            _context = context;
            _mapper = mapper;
            _s3Service = s3Service;
        }

        public async Task<string> DoToggleActive(string productId, bool active)
        {
            if (productId.IsEmpty()) return "Product ID is not valid!";

            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product == null) return "Product is not available!";

            product.IsActive = active;
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return "";
        }

        public async Task<(string, ProductDetailVM?)> GetDetail(string productId)
        {
            var list = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductId == productId);
            if (list == null) return ("No product available", null);

            var listMapper = _mapper.Map<ProductDetailVM>(list);
            return ("", listMapper);
        }

        public async Task<(string, List<ProductListVM>?)> GetList()
        {
            var list = await _context.Products.Include(x => x.Category).ToListAsync();
            if (list.IsNullOrEmpty()) return ("No product available", null);

            var listMapper = _mapper.Map<List<ProductListVM>>(list);
            return ("", listMapper);
        }

        public async Task<string> DoInsertUpdate(InsertUpdateProductVM? input, string userId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            var uploadedUrls = new List<string>();
            try
            {
                if (input == null) return "Please input all fields!";
                if (input.ProductId.IsEmpty())
                {
                    input.ProductId = Guid.NewGuid().ToString();

                    string key = $"{UrlS3.Product}{input.ProductId}";
                    uploadedUrls = await _s3Service.UploadFilesAsync(key, input.ImageUrl);

                    Product product = new Product
                    {
                        ProductId = input.ProductId,
                        Title = input.Title,
                        CurrentPrice = input.CurrentPrice,
                        NewPrice = input.NewPrice,
                        CategoryId = input.CategoryId,
                        CreatedAt = DateTime.Now,
                        CreateUser = userId,
                        Description = input.Description,
                        ImageUrl = string.Join(";", uploadedUrls),
                        IsActive = input.IsActive,
                        IsDeleted = false,
                        Status = input.Status,
                        Rating = input.Rating,
                        Stock = input.Stock,
                        Sold = 0,
                    };
                    await _context.Products.AddAsync(product);
                }
                else
                {
                    var oldProduct = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == input.ProductId);
                    if (oldProduct == null) return "Product is not available";

                    if (!input.ImageUrl.IsNullOrEmpty())
                    {
                        if (!oldProduct.ImageUrl.IsEmpty() && input.ImageUrl != null)
                        {
                            string folderKey = $"{UrlS3.Product}{input.ProductId}/";
                            await _s3Service.DeleteFolderAsync(folderKey);
                        }
                        string key = $"{UrlS3.Product}{input.ProductId}";
                        uploadedUrls = await _s3Service.UploadFilesAsync(key, input.ImageUrl);
                        oldProduct.ImageUrl = string.Join(";", uploadedUrls);
                    }

                    oldProduct.Title = input.Title;
                    oldProduct.NewPrice = input.NewPrice;
                    oldProduct.CurrentPrice = input.CurrentPrice;
                    oldProduct.CategoryId = input.CategoryId;
                    oldProduct.Status = input.Status;
                    oldProduct.Rating = input.Rating;
                    oldProduct.UpdateUser = userId;
                    oldProduct.Description = input.Description;
                    oldProduct.Stock = input.Stock;
                    oldProduct.UpdatedAt = DateTime.Now;

                    _context.Products.Update(oldProduct);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                if (uploadedUrls.Any())                 // Xóa ảnh đã upload lên S3 nếu có lỗi
                {
                    foreach (var url in uploadedUrls)
                    {
                        string key = $"{UrlS3.Product}{input.ProductId}";
                        uploadedUrls = await _s3Service.UploadFilesAsync(key, input.ImageUrl);
                    }
                }
                throw new Exception("An error occurred while updating the product: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message);
            }
        }
    }
}

