using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesTutorial.Data;
using RazorPagesTutorial.Services;

namespace RazorPagesTutorial.Pages.Person
{
    public class CreateModel : PageModel
    {
        private readonly   DatabaseContext _ctx;
        private readonly   IFileService _fileService;
        public CreateModel(DatabaseContext ctx,IFileService fileService)
        {
            this._ctx = ctx;
            _fileService = fileService;
        }
        [BindProperty]
        public Data.Person Person {get;set;}
        public IActionResult OnGet()
        {
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
                if(Person == null)
                    return NotFound();
                if(Person.ImageFile != null)
                {
                    var fResult=_fileService.SaveImage(Person.ImageFile);
                    if (fResult.Item1 == 1)
                    {
                        Person.ProfilePicture=fResult.Item2; //name of image
                    }
                }
                    _ctx.Person.Add(Person);
                   await _ctx.SaveChangesAsync();
                TempData["success"] = "Saved sucessfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error has occur";
            }
            return RedirectToPage();
		}
	}
}
