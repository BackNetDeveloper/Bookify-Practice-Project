namespace Bookify.Web.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Author : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "Lenght Must Be Less Than 100 Chars !")]
        public string Name { get; set; } = null!;
    }
}
