namespace Bookify.Web.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "Lenght Must Be Less Than 100 Chars !")]
        public string Name { get; set; } = null!;
		public ICollection<BookCategory> Books { get; set; } = new HashSet<BookCategory>();
	}
}
