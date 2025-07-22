using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data; // Projenin namespace'i

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    // Sadece bu constructor kalsın
    public HomeController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public IActionResult Index()
    {
        ViewBag.Message = "Hoşgeldiniz!";
        return View();
    }

    public async Task<IActionResult> Blog()
    {
        try
        {
            var posts = await _context.BlogPosts.ToListAsync();
            if (posts == null || posts.Count == 0)
            {
                Console.WriteLine("Veritabanından veri gelmedi veya boş.");
            }
            else
            {
                Console.WriteLine("Veri başarıyla alındı.");
            }

            return View(posts);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Blog action HATASI: " + ex.ToString());
            throw;
        }
    }


    // Belirli bir blog yazısını asenkron olarak id'ye göre buluyoruz
    public async Task<IActionResult> BlogDetail(int id)
    {
        // Belirli bir blog yazısını id'ye göre asenkron olarak bul
        var post = await _context.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
        if (post == null)
        {
            return NotFound(); // Eğer blog yazısı bulunamazsa 404 döndür
        }
        return View(post);
    }

    [Authorize]
    // GET isteği için ValidateAntiForgeryToken niteliğini kaldırdık.
    // Bu nitelik sadece POST metotları için gereklidir.
    public IActionResult CreateBlog()
    {
        return View(new BlogPost());
    }


    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken] // CSRF saldırılarına karşı koruma
    public async Task<IActionResult> CreateBlog(BlogPost model, IFormFile imageFile)
    {
        

        try
        {
            if (!ModelState.IsValid)
            {
                // Model doğrulaması başarısız olursa view'i tekrar göster
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
                // Resim yüklenmediyse varsayılan bir resim atayabilir veya hata verebilirsin
                // Örneğin: model.ImageFile = "/images/default.png";
                // Eğer ImageFile alanı veritabanında NULL kabul etmiyorsa ve resim yüklenmediyse burada hata alırsın.
                // Bu durumda ya nullable yapmalısın (string?) ya da varsayılan bir değer atamalısın.
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
            // Blog yazısını veritabanına ekle
            _context.BlogPosts.Add(model);
            await _context.SaveChangesAsync(); // Asenkron kaydetme

            // Başarılı olursa Blog sayfasına yönlendir
            return RedirectToAction("Blog");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Bir hata oluştu: " + ex.ToString()); // TAM hata mesajı
            return View(model);
        }

    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken] // CSRF saldırılarına karşı koruma
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
                // Eğer blog yazısının bir resmi varsa, o resmi de sil
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
            TempData["SilmeBasarili"] = "Blog başarıyla silindi !";
        }
        else
        {
            TempData["UyariMesaji"] = "Paylaşılan bloğu yalnızca paylaşan kullanıcı silebilir!!";
        }
        return RedirectToAction("Blog");
    }

    public IActionResult DogumGunu()
    {
        return View();
    }

}
