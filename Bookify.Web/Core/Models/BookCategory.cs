namespace Bookify.Web.Core.Models
{
	public class BookCategory
	{
		// We Made A Composite Pk For This Table In [OnModelCreating]
		public int BookId { get; set; }
        public Book? Book { get; set; }          // Navigational Property
        public int CategoryId { get; set; }
        public Category? Category { get; set; }  // Navigational Property
	}
}
