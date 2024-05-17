namespace Bookify.Web.Core.ViewModels
{
	public class BookCopyFormViewModel
	{
		public int Id { get; set; }
		public int BookId { get; set; }
		[Display(Name = "Is Available For Rental")]
		public bool IsAvailableForRental { get; set; }
		[Display(Name = "Edition Number"),Range(1,1000,ErrorMessage =CustomErrorMsg.ValidateRang)]
		public int EditionNumber { get; set; }
		public bool showRentalInput { get; set; }
	}
}
