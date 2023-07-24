using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesTutorial.Data;
using RazorPagesTutorial.Services;

namespace RazorPagesTutorial.Pages.Person
{
    public class EditModel : PageModel
    {

		private readonly DatabaseContext _ctx;
		private readonly IFileService _fileService;
		public EditModel(DatabaseContext ctx,IFileService fileService)
		{
			this._ctx = ctx;
			_fileService = fileService;
		}
		[BindProperty]
		public Data.Person Person { get; set; }

		public IActionResult OnGet(int id)
		{
			var person = _ctx.Person.Find(id);
			if(person==null)
				return NotFound();
			Person = person;
			return Page();
		}
		public async Task<IActionResult> OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			try
			{
				if (Person == null)
					return NotFound();
				var oldImage = Person.ProfilePicture;
				if (Person.ImageFile != null)
				{
					//we have to save the image. remove the older image from folder
					
					var fResult = _fileService.SaveImage(Person.ImageFile);
					if (fResult.Item1 == 1)
					{
						Person.ProfilePicture = fResult.Item2; //name of image
					}
				}
				_ctx.Person.Update(Person);
				await _ctx.SaveChangesAsync();
				if(!string.IsNullOrEmpty(oldImage)&& oldImage!=Person.ProfilePicture)
				{
					_fileService.DeleteImage(oldImage);
				}
				TempData["success"] = "Saved sucessfully";
				return RedirectToPage("DisplayAll");
			}
			catch (Exception ex)
			{
				TempData["error"] = "Error has occur";
				return Page();
			}
			
		}
	}
}
