using Bookify.Web.Core.Models;

namespace Bookify.Web.Controllers
{
	[Authorize(Roles = SystemRoles.Archive)]
	public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {

            var categories = await _context.Categories.ToListAsync();
            var mapedcategores = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
            return View(mapedcategores);
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = _mapper.Map<Category>(model);
                category.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
				await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                var viewModel = _mapper.Map<CategoryViewModel>(category);
                return PartialView("_CategoryRow", viewModel);
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
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();
            var mapedCategory = _mapper.Map<CategoryFormViewModel>(category);
            return PartialView("_Form", mapedCategory);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(model.Id);
                if (category is null)
                    return NotFound();
                category = _mapper.Map(model, category);
				category.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
				category.LastUpdatedOn = DateTime.Now;
				await _context.SaveChangesAsync();
                var viewModel = _mapper.Map<CategoryViewModel>(category);
                return PartialView("_CategoryRow", viewModel);
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
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();
            category.IsDeleted = !category.IsDeleted;
			category.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
			category.LastUpdatedOn = DateTime.Now;
			await _context.SaveChangesAsync();
			return Ok(category.LastUpdatedOn.ToString());
        }
        public IActionResult IsAllowed(CategoryFormViewModel model)
        {
            var category = _context.Categories.SingleOrDefault(c => c.Name == model.Name);
            var isallowed = category is null || category.Id.Equals(model.Id);
            return Json(isallowed);
        }
    }
}
