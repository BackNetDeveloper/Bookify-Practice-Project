namespace Bookify.Web.Core.Models
{
	[Index(nameof(Title),nameof(AuthorId),IsUnique = true)]
	public class Book:BaseEntity
	{
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public DateTime PublishingDate { get; set; } 
		public string? ImageUrl { get; set; }
		public string? ImageThumbnailUrl { get; set; }
		public string? ImagePublicId { get; set; }
        public bool IsAvailableForRental { get; set; }
		public string Description { get; set; } = null!;

		[MaxLength(500, ErrorMessage = CustomErrorMsg.MaxLength)]
		public string Title { get; set; } = null!;

		[MaxLength(200, ErrorMessage = CustomErrorMsg.MaxLength)]
		public string Publisher { get; set; } = null!;

		[MaxLength(50, ErrorMessage = CustomErrorMsg.MaxLength)]
		public string Hall { get; set; } = null!;
        public ICollection<BookCategory> Categories { get; set; } = new HashSet<BookCategory>();
        public ICollection<BookCopy> Copies { get; set; } = new HashSet<BookCopy>();
    }
}
