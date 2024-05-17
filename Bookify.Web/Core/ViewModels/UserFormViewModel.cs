using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModels
{
	public class UserFormViewModel
	{
		public string? Id { get; set; }

		[
		 MaxLength(100, ErrorMessage = CustomErrorMsg.MaxLength), Display(Name = "Full Name"),
		 RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = CustomErrorMsg.OnlyEnglishLetters)
		]
		public string FullName { get; set; } = null!;

		[
	     MaxLength(20, ErrorMessage = CustomErrorMsg.MaxLength), Display(Name = "User Name"),
		 RegularExpression(RegexPatterns.Username, ErrorMessage = CustomErrorMsg.InvalidUsername)
		]
		[Remote("AllowUserName", null!, AdditionalFields = "Id", ErrorMessage = CustomErrorMsg.Duplicated)]
		public string UserName { get; set; } = null!;

		[MaxLength(200, ErrorMessage = CustomErrorMsg.MaxLength), EmailAddress]
		[Remote("AllowEmail", null!, AdditionalFields = "Id", ErrorMessage = CustomErrorMsg.Duplicated)]
		public string Email { get; set; } = null!;
		[
	     DataType(DataType.Password),
		 StringLength(100, ErrorMessage = CustomErrorMsg.MaxMinLength, MinimumLength = 8),
		 RegularExpression(RegexPatterns.Password, ErrorMessage = CustomErrorMsg.WeakPassword)
	    ]
		[RequiredIf("Id == null", ErrorMessage = CustomErrorMsg.RequiredField)] // Very Important Only When We Create New User & When We Update We Do Not Need This Value
		public string? Password { get; set; } = null!; // When We Update We Do Not Need This Value That Why We Made It Nullable Property 

		[DataType(DataType.Password), Display(Name = "Confirm password"),
			Compare("Password", ErrorMessage = CustomErrorMsg.ConfirmPasswordNotMatch)]
		[RequiredIf("Id == null", ErrorMessage = CustomErrorMsg.RequiredField)] // Very Important Only When We Create New User & When We Update We Do Not Need This Value
		public string? ConfirmPassword { get; set; } = null!; // When We Update We Do Not Need This Value That Why We Made It Nullable Property

		[Display(Name = "Roles")]
		public IList<string> SelectedRoles { get; set; } = new List<string>();

		public IEnumerable<SelectListItem>? Roles { get; set; }
	}
}
