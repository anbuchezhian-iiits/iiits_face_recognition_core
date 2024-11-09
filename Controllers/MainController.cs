using iiits_face_recognition_core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

public class PersonController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PersonController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    // Save action
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(string PersonName, IFormFile Photo1, IFormFile Photo2)
    {
        if (ModelState.IsValid)
        {
            var person = new Person { PersonName = PersonName };

            if (Photo1 != null)
            {
                person.Photo1Path = await SaveImageAsync(Photo1);
            }
            if (Photo2 != null)
            {
                person.Photo2Path = await SaveImageAsync(Photo2);
            }

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Details are saved successfully!";
            return RedirectToAction("List", "Registration");
        }

        return View("New"); // Ensure the "New" view exists in Views/Registration/New.cshtml
    }

    // Edit person GET action
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var person = await _context.Persons.FindAsync(id);
        if (person == null)
        {
            return NotFound();
        }

        // Explicitly specifying the path for the Edit view
        return View("~/Views/Registration/Edit.cshtml", person); // Corrected path
    }

    // Edit person POST action
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string PersonName, IFormFile Photo1, IFormFile Photo2)
    {
        var person = await _context.Persons.FindAsync(id);
        if (person == null)
        {
            return NotFound();
        }

        person.PersonName = PersonName;

        if (Photo1 != null)
        {
            person.Photo1Path = await SaveImageAsync(Photo1);
        }
        if (Photo2 != null)
        {
            person.Photo2Path = await SaveImageAsync(Photo2);
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Person details updated successfully!";  // You can use TempData for feedback messages

        return RedirectToAction("List", "Registration");  // Make sure your List action and view are correctly linked
    }

    // Delete person action
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var person = await _context.Persons.FindAsync(id);
        if (person == null)
        {
            return NotFound(); // Return 404 if person not found
        }

        // Delete associated images from the upload folder
        DeleteImageIfExists(person.Photo1Path);
        DeleteImageIfExists(person.Photo2Path);

        // Remove the person from the database
        _context.Persons.Remove(person);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Record deleted successfully!";
        return RedirectToAction("List", "Registration"); // Ensure List action is present in the correct controller
    }

    // Helper method to delete the image if it exists
    private void DeleteImageIfExists(string imagePath)
    {
        if (!string.IsNullOrEmpty(imagePath))
        {
            // Get the absolute path of the image file
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));

            if (System.IO.File.Exists(filePath))
            {
                // Delete the file from the server
                System.IO.File.Delete(filePath);
            }
        }
    }


    private async Task<string> SaveImageAsync(IFormFile image)
    {
        // Ensure that the "uploads" folder exists within wwwroot
        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

        // Generate a unique file name using the original file name and a random string
        var uniqueFileName = Path.GetFileNameWithoutExtension(image.FileName) + "_" + Path.GetRandomFileName() + Path.GetExtension(image.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Save the file asynchronously
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return "/uploads/" + uniqueFileName; // Return the relative file path
    }
}
