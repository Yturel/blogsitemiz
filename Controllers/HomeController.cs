using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication5.Data; // Projenin namespace'i
using System.IO; // Path ve FileStream için eklendi
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Mvc.Formatters; // Task için eklendi

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    // Sadece bu constructor kalsýn
    public HomeController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public IActionResult Index()
    {
        ViewBag.Message = "Hoþgeldiniz!";
        return View();
    }

    public async Task<IActionResult> Blog()
    {
        try
        {
            var posts = await _context.BlogPosts.ToListAsync();
            if (posts == null || posts.Count == 0)
            {
                Console.WriteLine("Veritabanýndan veri gelmedi veya boþ.");
            }
            else
            {
                Console.WriteLine("Veri baþarýyla alýndý.");
            }

            return View(posts);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Blog action HATASI: " + ex.ToString());
            throw;
        }
    }


    // Belirli bir blog yazýsýný asenkron olarak id'ye göre buluyoruz
    public async Task<IActionResult> BlogDetail(int id)
    {
        // Belirli bir blog yazýsýný id'ye göre asenkron olarak bul
        var post = await _context.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
        if (post == null)
        {
            return NotFound(); // Eðer blog yazýsý bulunamazsa 404 döndür
        }
        return View(post);
    }

    [Authorize]
    // GET isteði için ValidateAntiForgeryToken niteliðini kaldýrdýk.
    // Bu nitelik sadece POST metotlarý için gereklidir.
    public IActionResult CreateBlog()
    {
        return View(new BlogPost());
    }


    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken] // CSRF saldýrýlarýna karþý koruma
    public async Task<IActionResult> CreateBlog(BlogPost model, IFormFile imageFile)
    {
        

        try
        {
            if (!ModelState.IsValid)
            {
                // Model doðrulamasý baþarýsýz olursa view'i tekrar göster
                return View(model);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var uploadPath = Path.Combine(_env.WebRootPath, "images");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.ImageFile = "/images/" + fileName;


            }


            else
            {
                // Resim yüklenmediyse varsayýlan bir resim atayabilir veya hata verebilirsin
                // Örneðin: model.ImageFile = "/images/default.png";
                // Eðer ImageFile alaný veritabanýnda NULL kabul etmiyorsa ve resim yüklenmediyse burada hata alýrsýn.
                // Bu durumda ya nullable yapmalýsýn (string?) ya da varsayýlan bir deðer atamalýsýn.
            }
            string paylasanKullanici = User.Identity.Name;
            if (paylasanKullanici == "yturel573@gmail.com" || paylasanKullanici == "turely573@gmail.com")
            {
                paylasanKullanici = "Yunus";
            }
            else
            {
                paylasanKullanici = "Agnieszka"; 
            }
            model.PaylasanKullanici = paylasanKullanici;
            // Blog yazýsýný veritabanýna ekle
            _context.BlogPosts.Add(model);
            await _context.SaveChangesAsync(); // Asenkron kaydetme

            // Baþarýlý olursa Blog sayfasýna yönlendir
            return RedirectToAction("Blog");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Bir hata oluþtu: " + ex.ToString()); // TAM hata mesajý
            return View(model);
        }

    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken] // CSRF saldýrýlarýna karþý koruma
    public async Task<IActionResult> Delete(int id)
    {
        
        var post = await _context.BlogPosts.FirstOrDefaultAsync(p => p.Id == id); // Asenkron sorgu

        string paylasanKullanici = User.Identity.Name;
        if (paylasanKullanici == "yturel573@gmail.com" || paylasanKullanici == "turely573@gmail.com")
        {
            paylasanKullanici = "Yunus";
        }
        else
        {
            paylasanKullanici = "Agnieszka";
        }

        if (post.PaylasanKullanici == paylasanKullanici || post.PaylasanKullanici == null)
        {
            if (post != null)
            {
                // Eðer blog yazýsýnýn bir resmi varsa, o resmi de sil
                if (!string.IsNullOrEmpty(post.ImageFile))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", post.ImageFile.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync(); // Asenkron kaydetme
            }
            TempData["SilmeBasarili"] = "Blog baþarýyla silindi !";
        }
        else
        {
            TempData["UyariMesaji"] = "Paylaþýlan bloðu yalnýzca paylaþan kullanýcý silebilir!!";
        }
        return RedirectToAction("Blog");
    }

    public IActionResult DogumGunu()
    {
        return View();
    }

}
