using API.Common;
using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;

namespace API.Services
{
    public interface ICategoryService
    {
        public Task<(string, List<CategoryVM>?)> GetList(bool? active = true, int? typeCateria = null);
        public Task<(string, CategoryVM?)> GetDetail(int categoryId, bool? active = null, int? typeCateria = null);
        public Task<string> DoToggleActive(int? categoryId, bool active);
        public Task<string> DoInsertUpdate(InsertUpdateCategory? input);
        public Task<string> DoDeleteSoft(int? categoryId);

    }

    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly Exe201Context _context;

        public CategoryService(IMapper mapper, Exe201Context context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<string> DoToggleActive(int? categoryId, bool active)
        {
            if (categoryId == null) return "Category ID is not valid!";

            var cate = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId && x.IsDeleted == true);
            if (cate == null) return "Category is not available!";

            cate.IsActive = active;
            _context.Categories.Update(cate);
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<(string, List<CategoryVM>?)> GetList(bool? active = null, int? typeCateria = null)
        {
            var list = await _context.Categories
                .Where(x => x.IsDeleted == false && (active == null || x.IsActive == active)
                && (typeCateria == null || x.TypeObject == typeCateria))
                .ToListAsync();
            if (list.IsNullOrEmpty()) return ("No Category available", null);

            var listMapper = _mapper.Map<List<CategoryVM>>(list);
            return ("", listMapper);
        }

        public async Task<(string, CategoryVM?)> GetDetail(int categoryId, bool? active = null, int? typeCateria = null)
        {
            var category = await _context.Categories
            .Where(x => x.IsDeleted == false
                && (!active.HasValue || x.IsActive == active)
                && (typeCateria == null || x.TypeObject == typeCateria))
                .FirstOrDefaultAsync(x => x.Id == categoryId);

            if (category == null) return ("Category is not available!", null);

            var listMapper = _mapper.Map<CategoryVM>(category);
            return ("", listMapper);
        }

        public async Task<string> DoInsertUpdate(InsertUpdateCategory? input)
        {
            if (input == null) return ("Please input all fields");
            if (input.Id == null || input.Id == 0)
            {
                Category category = new Category
                {
                    Name = input.Name,
                    IsActive = input.IsActive,
                    TypeObject = input.TypeObject,
                    IsDeleted = false
                };
                await _context.Categories.AddAsync(category);
            }
            else
            {
                var cate = await _context.Categories.FirstOrDefaultAsync(x => x.Id == input.Id);
                if (cate == null) return ("Category Id is not available");

                cate.IsActive = input.IsActive;
                cate.TypeObject = input.TypeObject;
                cate.Name = input.Name;
                
                _context.Categories.Update(cate);
            }
            await _context.SaveChangesAsync();
            return ("");
        }
        public async Task<string> DoDeleteSoft(int? categoryId)
        {
            if (categoryId == null) return "Category ID is not valid!";
            var cate = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId && x.IsDeleted == false);
            if (cate == null) return ("Category is not available!");

            cate.IsDeleted = true;
            _context.Categories.Update(cate);
            await _context.SaveChangesAsync();
            return string.Empty;
        }

    }
}
