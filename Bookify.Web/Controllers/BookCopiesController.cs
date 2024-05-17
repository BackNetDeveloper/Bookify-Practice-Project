namespace Bookify.Web.Controllers
{
	[Authorize(Roles = SystemRoles.Archive)]
	public class BookCopiesController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
        public BookCopiesController(ApplicationDbContext context , IMapper mapper)
        {
			_context = context;
			_mapper  = mapper;
        }
		[AjaxOnly]
		public IActionResult Create(int bookId)
		{
			var book = _context.Books.Find(bookId);
			if (book is null)
				return NotFound();
			var viewModel = new BookCopyFormViewModel 
			{
				BookId = bookId,
				showRentalInput = book.IsAvailableForRental 
			};
			return PartialView("Form",viewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(BookCopyFormViewModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			var book = _context.Books.Find(model.BookId);
			if (book is null)
				return NotFound();
			var copy = new BookCopy
			{
				EditionNumber = model.EditionNumber,
				IsAvailableForRental = book.IsAvailableForRental && model.IsAvailableForRental,
				CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
			};
			book.Copies.Add(copy);
			_context.SaveChanges();
			var viewModel = _mapper.Map<BookCopyViewModel>(copy);
			return PartialView("_BookCopyRow",viewModel);
		}
		[AjaxOnly]
		public IActionResult Edit(int id)
		{
			var copy = _context.BooksCopies.Include(c => c.Book).SingleOrDefault(c => c.Id == id);
			if (copy is null)
				return NotFound();
			var viewModel = _mapper.Map<BookCopyFormViewModel>(copy);
			viewModel.showRentalInput = copy.Book!.IsAvailableForRental;
			return PartialView("Form", viewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(BookCopyFormViewModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			var copy = _context.BooksCopies.Include(c => c.Book).SingleOrDefault(c => c.Id == model.Id);
			if (copy is null)
				return NotFound();
			copy.EditionNumber = model.EditionNumber;
			copy.IsAvailableForRental = copy.Book!.IsAvailableForRental && model.IsAvailableForRental;
			copy.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
			copy.LastUpdatedOn = DateTime.Now;
			_context.SaveChanges();
			var viewModel = _mapper.Map<BookCopyViewModel>(copy);
			return PartialView("_BookCopyRow", viewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			if (id <= 0)
				return NotFound();
			var bookCopy = await _context.BooksCopies.FindAsync(id);
			if (bookCopy == null)
				return NotFound();
			bookCopy.IsDeleted = !bookCopy.IsDeleted;
			bookCopy.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
			bookCopy.LastUpdatedOn = DateTime.Now;
			_context.SaveChanges();
			return Ok();
		}
	}
}
