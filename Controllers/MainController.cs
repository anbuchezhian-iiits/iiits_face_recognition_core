using iiits_face_recognition_core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class PersonController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly HttpClient _httpClient;

    public PersonController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, HttpClient httpClient)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _httpClient = httpClient;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(string PersonName, IFormFile Photo1, IFormFile Photo2)
    {
        PersonName = PersonName.ToLower();

        if (_context.Persons.Any(p => p.PersonName == PersonName))
        {
            ModelState.AddModelError("PersonName", "A person with this name already exists.");
            TempData["WarningMessage"] = "A person with this name already exists. Please use a different name.";
            return View("~/Views/Registration/New.cshtml");
        }

        if (ModelState.IsValid)
        {
            var person = new Person { PersonName = PersonName };
            person.Photo1Path = await SaveImageAsync(Photo1);
            person.Photo2Path = await SaveImageAsync(Photo2);

            person.Embedding = await GetEmbeddingsFromAPI(Photo1, Photo2) ?? Array.Empty<byte>();
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Details are saved successfully!";
            return RedirectToAction("List", "Registration");
        }

        return View("~/Views/Registration/New.cshtml");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var person = await _context.Persons.FindAsync(id);
        if (person == null)
        {
            return NotFound();
        }

        return View("~/Views/Registration/Edit.cshtml", person);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string PersonName, IFormFile Photo1, IFormFile Photo2)
    {
        PersonName = PersonName.ToLower();

        var person = await _context.Persons.FindAsync(id);
        if (person == null)
        {
            return NotFound();
        }

        if (_context.Persons.Any(p => p.PersonName == PersonName && p.Id != id))
        {
            ModelState.AddModelError("PersonName", "A person with this name already exists.");
            TempData["WarningMessage"] = "A person with this name already exists. Please use a different name.";
            return View("~/Views/Registration/Edit.cshtml", person);
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

        var updatedEmbeddings = await GetEmbeddingsFromAPI(Photo1, Photo2);

        if (updatedEmbeddings != null)
        {
            person.Embedding = updatedEmbeddings;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Person details updated successfully!";

        return RedirectToAction("List", "Registration");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var person = await _context.Persons.FindAsync(id);
        if (person == null)
        {
            return NotFound();
        }

        DeleteImageIfExists(person.Photo1Path);
        DeleteImageIfExists(person.Photo2Path);

        _context.Persons.Remove(person);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Record deleted successfully!";
        return RedirectToAction("List", "Registration");
    }

    private async Task<byte[]> GetEmbeddingsFromAPI(IFormFile Photo1, IFormFile Photo2)
    {
        var formData = new MultipartFormDataContent();
        if (Photo1 != null) formData.Add(new ByteArrayContent(await FileToByteArrayAsync(Photo1)), "Photo1", Photo1.FileName);
        if (Photo2 != null) formData.Add(new ByteArrayContent(await FileToByteArrayAsync(Photo2)), "Photo2", Photo2.FileName);

        try
        {
            var response = await _httpClient.PostAsync("http://127.0.0.1:5000/get_average_embedding", formData);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting embeddings from API: {ex.Message}");
            return null;
        }
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Path.GetFileNameWithoutExtension(image.FileName) + "_" + Path.GetRandomFileName() + Path.GetExtension(image.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return "/uploads/" + uniqueFileName;
    }

    private async Task<byte[]> FileToByteArrayAsync(IFormFile file)
    {
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }

    private void DeleteImageIfExists(string imagePath)
    {
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }
}

public class RecognizePersonController : Controller
{
    private readonly ApplicationDbContext _context;

    public RecognizePersonController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CaptureAndRecognize(IFormFile capturedPhoto)
    {
        if (capturedPhoto == null || capturedPhoto.Length == 0) return BadRequest("No image provided.");

        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(capturedPhoto.FileName);
        string uploads = Path.Combine("wwwroot", "captured");
        Directory.CreateDirectory(uploads);

        string filePath = Path.Combine(uploads, uniqueFileName);
        using (var stream = new FileStream(filePath, FileMode.Create)) await capturedPhoto.CopyToAsync(stream);

        byte[] unknownEmbedding;
        using (var client = new HttpClient())
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StreamContent(new FileStream(filePath, FileMode.Open)), "image", uniqueFileName);

            var response = await client.PostAsync("http://127.0.0.1:5000/recognize_face", formData);
            if (!response.IsSuccessStatusCode) return BadRequest("Face recognition API failed.");

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
            unknownEmbedding = ConvertHexToByteArray(json["unknown_embedding"].ToString());
        }

        var knownFaces = await _context.Persons.Select(p => new { p.PersonName, p.Embedding }).ToListAsync();
        string bestMatchName = null;
        double bestSimilarity = -1;

        foreach (var face in knownFaces)
        {
            byte[] knownEmbedding = face.Embedding;
            double similarity = CosineSimilarity(unknownEmbedding, knownEmbedding);

            if (similarity > bestSimilarity)
            {
                bestSimilarity = similarity;
                bestMatchName = face.PersonName;
            }
        }

        double similarityThreshold = 0.6;
        if (bestSimilarity >= similarityThreshold)
        {
            var recognizedPerson = new RecognizedPerson
            {
                RecognizedPersonName = bestMatchName,
                CapturedPhotoPath = Path.Combine("captured", uniqueFileName),
                UnknownEmbedding = unknownEmbedding,
                SimilarityScore = bestSimilarity,
                EntryDateTime = DateTime.UtcNow
            };

            _context.RecognizedPersons.Add(recognizedPerson);
            await _context.SaveChangesAsync();
            return Json(new { recognizedPerson.RecognizedPersonName, Similarity = bestSimilarity });
        }
        else
        {
            return Json(new { RecognizedPersonName = "No match found", Similarity = bestSimilarity });
        }
    }

    private byte[] ConvertHexToByteArray(string hex)
    {
        int length = hex.Length;
        byte[] bytes = new byte[length / 2];
        for (int i = 0; i < length; i += 2) bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    private double CosineSimilarity(byte[] embedding1, byte[] embedding2)
    {
        double dotProduct = 0, normA = 0, normB = 0;
        for (int i = 0; i < embedding1.Length; i++)
        {
            dotProduct += embedding1[i] * embedding2[i];
            normA += embedding1[i] * embedding1[i];
            normB += embedding2[i] * embedding2[i];
        }
        return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}
