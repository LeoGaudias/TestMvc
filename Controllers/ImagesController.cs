using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestMvc.Data;
using TestMvc.Models;
using TestMvc.ViewModels;

namespace TestMvc.Controllers
{
    // Les bons tutos
    // https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud?view=aspnetcore-2.2
    // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.2

    public class ImagesController : Controller
    {
        private readonly TestContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ImagesController(TestContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Images.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var image = await _context.Images.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (image == null)
                return NotFound();

            return View(image);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,File")] ImageViewModel vm)
        {
            // /!\ Warning /!\
            // Don't rely on or trust the FileName property without validation. 
            // The FileName property should only be used for display purposes.

            var image = new Image
            {
                Title = vm.Title,
                Description = vm.Description,
                Path = "/img/" + vm.File.FileName
            };

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(image);
                    await _context.SaveChangesAsync();

                    var path = _hostingEnvironment.WebRootPath + "\\img\\" + vm.File.FileName;

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await vm.File.CopyToAsync(stream);
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(vm);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var image = await _context.Images.FindAsync(id);

            if (image == null)
                return NotFound();

            return View(image);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
                return NotFound();

            var studentToUpdate = await _context.Images.FirstOrDefaultAsync(x => x.Id == id);

            if (await TryUpdateModelAsync<Image>(studentToUpdate, "", x => x.Title, x => x.Description))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
                return NotFound();

            var image = await _context.Images.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (image == null)
                return NotFound();

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(image);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Cette option 

            var image = await _context.Images.FindAsync(id);

            if (image == null)
                return RedirectToAction(nameof(Index));

            var path = _hostingEnvironment.WebRootPath + image.Path.Replace("/","\\");
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            try
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

            // Ou cette option --> plus performantes
            // try
            // {
            //     Image imageToDelete = new Image() { Id = id };
            //     _context.Entry(imageToDelete).State = EntityState.Deleted;
            //     await _context.SaveChangesAsync();
            //     return RedirectToAction(nameof(Index));
            // }
            // catch (DbUpdateException /* ex */)
            // {
            //     //Log the error (uncomment ex variable name and write a log.)
            //     return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            // }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}