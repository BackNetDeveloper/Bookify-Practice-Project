using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
		public DbSet<Author> Authors { get; set; } = null!;
		public DbSet<Book> Books { get; set; } = null!;
		public DbSet<BookCategory> BooksCategories { get; set; } = null!;
		public DbSet<BookCopy> BooksCopies { get; set; } = null!;
		public DbSet<Category> Categories { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.HasSequence<int>("SerialNumber", schema: "shared")
				   .StartsAt(1000001);
			builder.Entity<BookCopy>()
				   .Property(e => e.SerialNumber)
				   .HasDefaultValueSql("next value for[Shared].[SerialNumber]");
			// To Make Composite Pk [We Use Only Fluent APIs]
			builder.Entity<BookCategory>()
				   .HasKey(Item => new { Item.BookId, Item.CategoryId });
			base.OnModelCreating(builder);
		}
	}
}