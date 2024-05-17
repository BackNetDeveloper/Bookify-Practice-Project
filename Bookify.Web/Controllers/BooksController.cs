using Bookify.Web.Helper;
using Bookify.Web.Services;
using Bookify.Web.Settings;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;
namespace Bookify.Web.Controllers
{
	[Authorize(Roles = SystemRoles.Archive)]
	public class BooksController : Controller
	{
		
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment  _webHostEnvironment;
		private readonly IMapper              _mapper;
        private readonly Cloudinary           _cloudinary;
        private readonly IDocumentSettings    _DocumentSettings;
		private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
		private int          _maxAllowedSize    = 2097152;
		public BooksController(ApplicationDbContext         context,
			                   IWebHostEnvironment          webHostEnvironment,
			                   IMapper                      mapper,
			                   IOptions<CloudinarySettings> cloudinary
			                  )
		{
			Account account = new()
			{
				Cloud     = cloudinary.Value.Cloud,
				ApiKey    = cloudinary.Value.ApiKey,
				ApiSecret = cloudinary.Value.ApiSecret,
			};
			_context            = context;
			_webHostEnvironment = webHostEnvironment;
			_mapper             = mapper;
			_cloudinary         = new Cloudinary(account);
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		[AjaxOnly]
		[ValidateAntiForgeryToken]
		public IActionResult GetBooks()
		{
			var skip     = int.Parse(Request.Form["start"]);
			var pageSize = int.Parse(Request.Form["length"]);

			var sortColumnIndex = int.Parse(Request.Form["order[0][column]"]);
			var sortColumnName  = Request.Form[$"columns[{sortColumnIndex}][name]"];
			var sortDirection   = Request.Form["order[0][dir]"];

			var searchValue = Request.Form["search[value]"];

			IQueryable<Book> books = _context.Books.Include(b => b.Author)
				                                   .Include(b => b.Categories)
				                                   .ThenInclude(c => c.Category);
			if (!string.IsNullOrEmpty(searchValue))
			{
				books = books.Where(b => b.Title.Contains(searchValue) || b.Author!.Name.Contains(searchValue));
			}
			books = books.OrderBy($"{sortColumnName} {sortDirection}");
			var data = books.Skip(skip)
				            .Take(pageSize)
				            .ToList();
			var mappedData = _mapper.Map<IEnumerable<BookViewModel>>(data);
			var recordsTotal = books.Count();

			var jsonData = new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = mappedData };
			return Json(jsonData);
		}
		[HttpGet]
		public IActionResult Details(int id)
		{
			var book = _context.Books.Include(b => b.Author)       // Related To The Book It Self
                                     .Include(b => b.Copies)       // Related To The Book It Self
                                     .Include(b => b.Categories)   // Related To The Book It Self
									 .ThenInclude(c => c.Category) // Related To The BookCategory Of This Book 
									 .SingleOrDefault(b => b.Id == id);
			if (book is null)
				return NotFound();

			var viewModel = _mapper.Map<BookViewModel>(book);
			return View(viewModel);
		}
		[HttpGet]
		public IActionResult Create()
		{
			return View("Form", PopulateViewModel());
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BookFormViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("Form",PopulateViewModel(model));
			}
			else
			{
				var book = _mapper.Map<Book>(model);
				
				if (model.Image is not null)
				{
					var extension = Path.GetExtension(model.Image.FileName);
					if (!_allowedExtensions.Contains(extension))
					{ 
						ModelState.AddModelError(nameof(model.Image),CustomErrorMsg.NotAllowedExtension);
						return View("Form", PopulateViewModel(model));
					}
					if (model.Image.Length > _maxAllowedSize)
					{
						ModelState.AddModelError(nameof(model.Image), CustomErrorMsg.MaxSize);
						return View("Form", PopulateViewModel(model));
					}
					//=============================================================================================
					// Upload To Local Server
					var fileName = _DocumentSettings.UploadFileToLocalServer(model.Image, _webHostEnvironment, "/images/books");
					book.ImageUrl = $"/images/books/{fileName}";
					book.ImageThumbnailUrl = $"/images/books/thumb/{fileName}";
                    _DocumentSettings.UploadFileThumbToLocalServer(model.Image,fileName, _webHostEnvironment, "/images/books/thumb");
                    //===================================    Cloudinary     ========================================
                    //// Upload To Cloudinary
                    //var CloudinaryResults = await DocumentSettings.UploadFileToCloudinary(model.Image, _cloudinary);
                    //book.ImageUrl = CloudinaryResults.Url;
                    //book.ImagePublicId = CloudinaryResults.PublicId;
                    //book.ImageThumbnailUrl = DocumentSettings.GetThumbnailUrlFromCloudinary(book.ImageUrl);
                    //============================================================================================= 
                }
				book.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
				foreach (var SelectedCategoryId in model.SelectedCategories)
					book.Categories.Add(new BookCategory { BookId = book.Id , CategoryId = SelectedCategoryId });
				
				await _context.AddAsync(book);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Details), new {id = book.Id});
			}
		}
		[HttpGet]
		public IActionResult Edit(int id)
		{
			var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == id);
			if (book is null)
				return NotFound();

			var Model = _mapper.Map<BookFormViewModel>(book);
			var viewModel = PopulateViewModel(Model);
			viewModel.SelectedCategories = book.Categories.Select(c=>c.CategoryId).ToList();
			return View("Form", viewModel);
		}
		private string ImagePublicIdValue = string.Empty;
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(BookFormViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("Form", PopulateViewModel(model));
			}
			var book = _context.Books.Include(b => b.Categories)
				                     .Include(b=>b.Copies)
				                     .SingleOrDefault(b => b.Id == model.Id);
			if (book is null)
				return NotFound();
			else
			{
				if (model.Image is not null)
				{
					if(!string.IsNullOrEmpty(book.ImageUrl))
					{
                        //=============================================================================================
                        // Delete From Local Server
                        _DocumentSettings.DeleteFileFromLocalServer(book.ImageUrl, _webHostEnvironment);
                        _DocumentSettings.DeleteFileThumbFromLocalServer(book.ImageThumbnailUrl, _webHostEnvironment);
						//===================================    Cloudinary    ========================================
						//// Delete From Cloudinary
						//await _cloudinary.DeleteResourcesAsync(book.ImagePublicId);
						//=============================================================================================
					}
					var extension = Path.GetExtension(model.Image.FileName);
					if (!_allowedExtensions.Contains(extension))
					{
						ModelState.AddModelError(nameof(model.Image), CustomErrorMsg.NotAllowedExtension);
						return View("Form", PopulateViewModel(model));
					}
					if (model.Image.Length > _maxAllowedSize)
					{
						ModelState.AddModelError(nameof(model.Image), CustomErrorMsg.MaxSize);
						return View("Form", PopulateViewModel(model));
					}
                    //=============================================================================================
                    // Upload To Local Server
                    var fileName = _DocumentSettings.UploadFileToLocalServer(model.Image, _webHostEnvironment, "/images/books");
                    model.ImageUrl = $"/images/books/{fileName}";
                    model.ImageThumbnailUrl = $"/images/books/thumb/{fileName}";
                    _DocumentSettings.UploadFileThumbToLocalServer(model.Image,fileName, _webHostEnvironment, "/images/books/thumb");
                    //===================================    Cloudinary    ========================================
                    //// Upload To Cloudinary
                    //var CloudinaryResults = await DocumentSettings.UploadFileToCloudinary(model.Image, _cloudinary);
                    //model.ImageUrl = CloudinaryResults.Url;
                    //ImagePublicIdValue = CloudinaryResults.PublicId;
                    //=============================================================================================
                }
				else if(model.ImageUrl is null && !string.IsNullOrEmpty(book.ImageUrl))
				{
					// We Send No Image In Our Model Then, We Must Keep The Old One & The Old ImageThumbnai 
					model.ImageUrl = book.ImageUrl;
					model.ImageThumbnailUrl = book.ImageThumbnailUrl;
				}
				book = _mapper.Map(model, book);
				book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
				book.LastUpdatedOn = DateTime.Now;
				//===================================    Cloudinary    ========================================
				//book.ImageThumbnailUrl = DocumentSettings.GetThumbnailUrlFromCloudinary(book.ImageUrl);
				//book.ImagePublicId = ImagePublicIdValue;
				//=============================================================================================
				foreach (var SelectedCategoryId in model.SelectedCategories)
					book.Categories.Add(new BookCategory { BookId = book.Id, CategoryId = SelectedCategoryId });

				if (!model.IsAvailableForRental)
					foreach (var copy in book.Copies)
						copy.IsAvailableForRental = false;

				_context.Update(book);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Details), new { id = book.Id });
			}
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (id <= 0)
                return NotFound();
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();
            book.IsDeleted = !book.IsDeleted;
			book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
			book.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult IsAllowed(BookFormViewModel model)
		{
			var book = _context.Books.SingleOrDefault(b => b.Title == model.Title && b.AuthorId == model.AuthorId);
			var isallowed = book is null || book.Id.Equals(model.Id);
			return Json(isallowed);
		}
		private BookFormViewModel PopulateViewModel(BookFormViewModel? model = null)
		{
			var viewModel = model is null ? new BookFormViewModel() : model;
			#region Fill Authors & Categories Using AutoMapper
			var Authors    = _context.Authors
							         .Where(a => !a.IsDeleted)
							         .OrderBy(a => a.Name)
							         .ToList();
			var Categories = _context.Categories
									 .Where(c => !c.IsDeleted)
									 .OrderBy(c => c.Name)
									 .ToList();

			viewModel.Authors    = _mapper.Map<IList<SelectListItem>>(Authors);
			viewModel.Categories = _mapper.Map<IList<SelectListItem>>(Categories);
			#endregion
			#region Fill Authors & Categories Without Using AutoMapper
			//var Authors = _context.Authors
			//							 .Where(c => !c.IsDeleted)
			//							 .Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() })
			//							 .OrderBy(a => a.Text)
			//							 .ToList();
			//var Categories = _context.Categories
			//						 .Where(c => !c.IsDeleted)
			//						 .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
			//						 .OrderBy(c => c.Text)
			//						 .ToList();
			//viewModel.Authors = Authors;
			//viewModel.Categories = Categories; 
			#endregion
			return viewModel;
		}
		}
}
