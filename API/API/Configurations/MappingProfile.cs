using API.Models;
using API.ViewModels;
using AutoMapper;

namespace API.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mapping
            CreateMap<User, UserVM>();
            CreateMap<User, UserListVM>();
            CreateMap<User, ProfileVM>();
            CreateMap<User, UpdateProfileModels>();
            CreateMap<User, UserLoginVM>();
            CreateMap<User, EditAccountVM>();

            // Post mapping
            CreateMap<Post, PostListVM>();
            CreateMap<Post, PostVM>();

            // Product mapping
            CreateMap<Product, ProductDetailVM>();
            CreateMap<Product, ProductListVM>();
            CreateMap<Product, ProductsVM>();

            // Category mapping
            CreateMap<Category, CategoryVM>();

            // Order mapping
            CreateMap<Order, OrderVM>()
              .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.OrderDetails));

            // Ánh xạ từ OrderDetail sang OrderDetailVM
            CreateMap<OrderDetail, OrderDetailVM>()
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.Product.ProductId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Product.Title))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name))
                .ForMember(dest => dest.ProductUrl, opt => opt.MapFrom(src => src.Product.ProductUrl))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Total));


            // Chatbot mapping
            CreateMap<Conversation, ConversationVm>();
            CreateMap<Message, MessageVm>();

            // Video mapping
            CreateMap<Video, VideoListVM>();
            CreateMap<Video, VideoDetailVM>();

            // CategoryVideo mapping
            CreateMap<Category, CategoryVideoVM>();

        }
    }
}
