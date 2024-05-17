using System.Security.Claims;

namespace Bookify.Web.Controllers
{
	[Authorize(Roles = SystemRoles.Archive)]
	public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthorsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {

            var authors = await _context.Authors.ToListAsync();
            var mapedauthors = _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
            return View(mapedauthors);
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var author = _mapper.Map<Author>(model);
                author.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
				await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();
                var viewModel = _mapper.Map<AuthorViewModel>(author);
                return PartialView("_AuthorRow", viewModel);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound();
            var mapedAuthor = _mapper.Map<AuthorFormViewModel>(author);
            return PartialView("_Form", mapedAuthor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AuthorFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var author = await _context.Authors.FindAsync(model.Id);
                if (author is null)
                    return NotFound();
                author = _mapper.Map(model, author);
                author.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
				author.LastUpdatedOn = DateTime.Now;
                await _context.SaveChangesAsync();
                var viewModel = _mapper.Map<AuthorViewModel>(author);
                return PartialView("_AuthorRow", viewModel);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (id <= 0)
                return NotFound();
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound();
            author.IsDeleted = !author.IsDeleted;
			author.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
			author.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Ok(author.LastUpdatedOn.ToString());
        }
        public IActionResult IsAllowed(AuthorFormViewModel model)
        {
            var author = _context.Authors.SingleOrDefault(c => c.Name == model.Name);
            var isallowed = author is null || author.Id.Equals(model.Id);
            return Json(isallowed);
        }
    }
}
