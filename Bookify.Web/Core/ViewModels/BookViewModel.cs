using Humanizer;

namespace Bookify.Web.Core.ViewModels
{
	public class BookViewModel
	{
		public int Id { get; set; } // Because We Need It For Edit Button In Details View
		public string Title { get; set; } = null!;
		public string AuthorName { get; set; } = null!; // string Because We Need To Show The Author Name In Details View
		public string Publisher { get; set; } = null!;
		public DateTime PublishingDate { get; set; }
		public string? ImageUrl { get; set; }
		public string? ImageThumbnailUrl { get; set; }
		public string Hall { get; set; } = null!;
		public bool IsAvailableForRental { get; set; }
		public string Description { get; set; } = null!;

		// IEnumerable<string> Because We Need To Show Names Of Categories In Details View
		public IEnumerable<string> Categories { get; set; } = null!;
		public IEnumerable<BookCopyViewModel> Copies { get; set; } = null!;
		public bool IsDeleted { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}
