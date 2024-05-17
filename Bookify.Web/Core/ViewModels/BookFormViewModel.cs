using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModels
{
	public class BookFormViewModel
	{
		public int Id { get; set; }
		[ MaxLength(500, ErrorMessage = CustomErrorMsg.MaxLength)]
		[Remote("IsAllowed", "Books", AdditionalFields = "Id,AuthorId", ErrorMessage = CustomErrorMsg.Duplicated)]
		public string Title { get; set; } = null!;

		[Display(Name ="Author")]
		[Remote("IsAllowed", "Books", AdditionalFields = "Id,Title", ErrorMessage = CustomErrorMsg.Duplicated)]
		public int AuthorId { get; set; }

		[Display(Name = "Publishing Date")]
		//[AssertThat("PublishingDate <= Today()", ErrorMessage = CustomErrorMsg.NotAllowFutureDates)]
		public DateTime PublishingDate { get; set; } = DateTime.Now;
		public IFormFile? Image { get; set; }
		public string? ImageUrl { get; set; }
		public string? ImageThumbnailUrl { get; set; }
		public string? ImagePublicId { get; set; }

		[Display(Name = "Is Available For Rental ?")]
		public bool IsAvailableForRental { get; set; }
		public string Description { get; set; } = null!;

		[MaxLength(200, ErrorMessage = CustomErrorMsg.MaxLength)]
		public string Publisher { get; set; } = null!;

		[MaxLength(50, ErrorMessage = CustomErrorMsg.MaxLength)]
		public string Hall { get; set; } = null!;

		[Display(Name = "Categories")]
		public IList<int> SelectedCategories { get; set; } = new List<int>();
		public IList<SelectListItem>? Categories { get; set; } // For Filling The Categories Dropdown List
		public IList<SelectListItem>? Authors { get; set; }    // For Filling The Authors Dropdown List
	}
}
