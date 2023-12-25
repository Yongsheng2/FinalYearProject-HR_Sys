using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Models.ViewModels;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DocumentController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index(string id)
        {
            var employeeSelected = await _db.EmployeeDetails.FindAsync(id);
            

            if (employeeSelected == null)
            {
                return NotFound();
            }
            else
            {
                var documentList = await _db.Document.Where(d => d.owner_id == employeeSelected.employee_id).ToListAsync();

                EmployeeDocumentListVM list = new EmployeeDocumentListVM()
                {
                    owner = employeeSelected,
                    documentsOwned = documentList
                };

                return View(list);
            }
           
        }

        public async Task<IActionResult> Create(string id)
        {
            var employeeSelected = await _db.EmployeeDetails.FindAsync(id);

            if (employeeSelected == null)
            {
                return NotFound();
            }
            else
            {
                EmployeeDocumentVM empDoc = new EmployeeDocumentVM()
                {
                    employee_id = employeeSelected.employee_id,
                    document = new Document() { owner_id = employeeSelected.employee_id }
                };

                return View(empDoc);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeDocumentVM empDoc)
        {
            var employeeSelected = await _db.EmployeeDetails.FindAsync(empDoc.employee_id);

            if (employeeSelected == null)
            {
                return NotFound();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;

                    if (files.Count > 0)
                    {

                        _db.Document.Add(empDoc.document);
                        await _db.SaveChangesAsync();

                        var docFromDb = await _db.Document.FindAsync(empDoc.document.document_id);

                        var uploads = Path.Combine(webRootPath, "documents");
                        var extension = Path.GetExtension(files[0].FileName);
                        var filepath = docFromDb.owner_id + "-" + docFromDb.document_id + extension;

                        using (var filestream = new FileStream(Path.Combine(uploads, filepath), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }

                        docFromDb.document_path = @"\documents\" + filepath;

                        await _db.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), docFromDb.owner_id);
                    }
                }
            }

            return View(empDoc);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var document = await _db.Document.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }
            else
            {
                return View(document);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Document doc)
        {
            if (ModelState.IsValid)
            {
                _db.Update(doc);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index), doc.owner_id);
            }

            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            var document = await _db.Document.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }
            else
            {
                return View(document);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            var document = await _db.Document.FindAsync(id);
            var empId = document.owner_id;

            if (document == null)
            {
                return View();
            }

            _db.Document.Remove(document);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), empId);
        }

        public async Task<FileResult> Download(string? id)
        {
            var document = await _db.Document.FindAsync(id);

            string path = _hostingEnvironment.WebRootPath + document.document_path;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", document.document_name + Path.GetExtension(path));
        }
    }
}
