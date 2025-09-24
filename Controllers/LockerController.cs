using Microsoft.AspNetCore.Mvc;
using LockerManagementSystem.Models;
using LockerManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LockerManagementSystem.Controllers
{
    public class LockerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public LockerController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RequestForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestForm(LockerRequest model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload
                    if (model.StudyLoadDocument != null && model.StudyLoadDocument.Length > 0)
                    {
                        // Create uploads directory if it doesn't exist
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Generate unique file name to prevent conflicts
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.StudyLoadDocument.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.StudyLoadDocument.CopyToAsync(stream);
                        }

                        // Store file information in database
                        model.FileName = model.StudyLoadDocument.FileName;
                        model.FilePath = uniqueFileName;
                        model.FileSize = model.StudyLoadDocument.Length;
                    }

                    // Set default status and date
                    model.Status = "Pending";
                    model.SubmissionDate = DateTime.Now;

                    // Save to database
                    _context.LockerRequests.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Locker request submitted successfully!";
                    return RedirectToAction("Confirmation", new { id = model.Id });
                }
                catch (Exception ex)
                {
                    // Log the error and show user-friendly message
                    ModelState.AddModelError("", "An error occurred while saving your request. Please try again.");
                    // You can log the actual error: ex.Message
                }
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }

        public IActionResult Confirmation(int id)
        {
            var request = _context.LockerRequests.FirstOrDefault(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpGet]
        public IActionResult Admin()
        {
            var requests = _context.LockerRequests.OrderByDescending(r => r.SubmissionDate).ToList();
            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.LockerRequests.FindAsync(id);
            if (request != null)
            {
                request.Status = "Approved";
                request.Approver = "Engr. Jessie Flores";
                request.ApproverTitle = "Dept. Head Dean";
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Request approved successfully!";
            }
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _context.LockerRequests.FindAsync(id);
            if (request != null)
            {
                request.Status = "Rejected";
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Request rejected successfully!";
            }
            return RedirectToAction("Admin");
        }

        // New action to view request details
        public IActionResult Details(int id)
        {
            var request = _context.LockerRequests.FirstOrDefault(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }
    }
}