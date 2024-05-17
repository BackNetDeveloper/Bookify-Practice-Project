namespace Bookify.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 1.Category
            CreateMap<Category, CategoryViewModel>(); // Work With Index View

            CreateMap<CategoryFormViewModel, Category>().ReverseMap(); // Work With Forms [Create & Update]

            CreateMap<Category, SelectListItem>()
                .ForMember(dist => dist.Value, options => options.MapFrom(src => src.Id))
                .ForMember(dist => dist.Text, options => options.MapFrom(src => src.Name));

            // 2.Author												
            CreateMap<Author, AuthorViewModel>(); // Work With Index View

            CreateMap<AuthorFormViewModel, Author>().ReverseMap(); // Work With Forms [Create & Update]

            CreateMap<Author, SelectListItem>()
                .ForMember(dist => dist.Value, options => options.MapFrom(src => src.Id))
                .ForMember(dist => dist.Text, options => options.MapFrom(src => src.Name));

            // 3.Book
            CreateMap<BookFormViewModel, Book>().ReverseMap() // Work With Forms [Create & Update]
                                                 .ForMember(dest => dest.Categories, options => options.Ignore());
            CreateMap<Book, BookViewModel>()
                .ForMember(dist => dist.AuthorName, options => options.MapFrom(src => src.Author!.Name))
                .ForMember(dist => dist.Categories, options => options.MapFrom(src => src.Categories
                                                                                         .Select(c => c.Category!.Name)
                                                                                         .ToList()));
            CreateMap<BookCopy, BookCopyViewModel>()
               .ForMember(dist => dist.BookTitle, options => options.MapFrom(src => src.Book!.Title));
            // 3.BookCopy
            CreateMap<BookCopyFormViewModel, BookCopy>().ReverseMap();
            // 4.Users
            CreateMap<ApplicationUser, UserViewModel>();
            CreateMap<UserFormViewModel, ApplicationUser>() // Work With Forms [Create & Update]
                .ForMember(dist => dist.NormalizedUserName, options => options.MapFrom(src => src.UserName!.ToUpper()))
                .ForMember(dist => dist.NormalizedEmail   , options => options.MapFrom(src => src.Email!.ToUpper()))
                .ReverseMap();
        }
    } }

