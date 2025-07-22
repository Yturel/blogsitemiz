using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication5.Data; // Projenin namespace'i
using System.IO; // Path ve FileStream i�in eklendi
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Mvc.Formatters; // Task i�in eklendi

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    // Sadece bu constructor kals�n
    public HomeController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public IActionResult Index()
    {
        ViewBag.Message = "Ho�geldiniz!";
        return View();
    }

    public async Task<IActionResult> Blog()
    {
        try
        {
            var posts = await _context.BlogPosts.ToListAsync();
            if (posts == null || posts.Count == 0)
            {
                Console.WriteLine("Veritaban�ndan veri gelmedi veya bo�.");
            }
            else
            {
                Console.WriteLine("Veri ba�ar�yla al�nd�.");
            }

            return View(posts);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Blog action HATASI: " + ex.ToString());
            throw;
        }
    }


    // Belirli bir blog yaz�s�n� asenkron olarak id'ye g�re buluyoruz
    public async Task<IActionResult> BlogDetail(int id)
    {
        // Belirli bir blog yaz�s�n� id'ye g�re asenkron olarak bul
        var post = await _context.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
        if (post == null)
        {
            return NotFound(); // E�er blog yaz�s� bulunamazsa 404 d�nd�r
        }
        return View(post);
    }

    [Authorize]
    // GET iste�i i�in ValidateAntiForgeryToken niteli�ini kald�rd�k.
    // Bu nitelik sadece POST metotlar� i�in gereklidir.
    public IActionResult CreateBlog()
    {
        return View(new BlogPost());
    }


    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken] // CSRF sald�r�lar�na kar�� koruma
    public async Task<IActionResult> CreateBlog(BlogPost model, IFormFile imageFile)
    {
        

        try
        {
            if (!ModelState.IsValid)
            {
                // Model do�rulamas� ba�ar�s�z olursa view'i tekrar g�ster
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
                // Resim y�klenmediyse varsay�lan bir resim atayabilir veya hata verebilirsin
                // �rne�in: model.ImageFile = "/images/default.png";
                // E�er ImageFile alan� veritaban�nda NULL kabul etmiyorsa ve resim y�klenmediyse burada hata al�rs�n.
                // Bu durumda ya nullable yapmal�s�n (string?) ya da varsay�lan bir de�er atamal�s�n.
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
            // Blog yaz�s�n� veritaban�na ekle
            _context.BlogPosts.Add(model);
            await _context.SaveChangesAsync(); // Asenkron kaydetme

            // Ba�ar�l� olursa Blog sayfas�na y�nlendir
            return RedirectToAction("Blog");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Bir hata olu�tu: " + ex.ToString()); // TAM hata mesaj�
            return View(model);
        }

    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken] // CSRF sald�r�lar�na kar�� koruma
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
                // E�er blog yaz�s�n�n bir resmi varsa, o resmi de sil
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
            TempData["SilmeBasarili"] = "Blog ba�ar�yla silindi !";
        }
        else
        {
            TempData["UyariMesaji"] = "Payla��lan blo�u yaln�zca payla�an kullan�c� silebilir!!";
        }
        return RedirectToAction("Blog");
    }

    public IActionResult DogumGunu()
    {
        return View();
    }

}
