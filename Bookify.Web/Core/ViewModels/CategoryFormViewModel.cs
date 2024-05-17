namespace Bookify.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [
         MaxLength(100, ErrorMessage = CustomErrorMsg.MaxLength), Display(Name = "Category"),
	     RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = CustomErrorMsg.OnlyEnglishLetters)
        ]
        [Remote("IsAllowed", "Categories", AdditionalFields = "Id", ErrorMessage = CustomErrorMsg.Duplicated)]
        public string Name { get; set; } = null!;
    }
}
