namespace Bookify.Web.Core.ViewModels
{
    public class AuthorFormViewModel
    {
        public int Id { get; set; }

        [
         MaxLength(100, ErrorMessage = CustomErrorMsg.MaxLength), Display(Name = "Author"),
	     RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = CustomErrorMsg.OnlyEnglishLetters)
        ]
        [Remote("IsAllowed", "Authors", AdditionalFields = "Id", ErrorMessage = CustomErrorMsg.Duplicated)]
        public string Name { get; set; } = null!;
    }
}
