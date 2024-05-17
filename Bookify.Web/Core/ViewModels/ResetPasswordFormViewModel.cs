using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModels
{
	public class ResetPasswordFormViewModel
	{
		public string Id { get; set; } = null!;
		[
		 DataType(DataType.Password),
		 StringLength(100, ErrorMessage = CustomErrorMsg.MaxMinLength, MinimumLength = 8),
		 RegularExpression(RegexPatterns.Password, ErrorMessage = CustomErrorMsg.WeakPassword)
		]
		[RequiredIf("Id == null", ErrorMessage = CustomErrorMsg.RequiredField)]
		public string? Password { get; set; } = null!;

		[DataType(DataType.Password), Display(Name = "Confirm password"),
			Compare("Password", ErrorMessage = CustomErrorMsg.ConfirmPasswordNotMatch)]
		[RequiredIf("Id == null", ErrorMessage = CustomErrorMsg.RequiredField)]
		public string? ConfirmPassword { get; set; } = null!;
	}
}
