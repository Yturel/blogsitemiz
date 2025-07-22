using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data; // Projenin namespace'i
using Microsoft.AspNetCore.Server.Kestrel.Core; // KestrelServerOptions için eklendi


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)  // Geliştirme için
    .AddEnvironmentVariables();               // Railway için

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Kestrel için maksimum istek gövdesi boyutunu artır (örneğin 50 MB)
// Bu, büyük dosyaların yüklenmesine izin verir.
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB (50 * 1024 * 1024)
});

// Eğer IIS kullanıyorsanız (in-process hosting), aşağıdaki satırı da eklemeniz gerekebilir:
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 52428800; // 50 MB
});


// Veritabanı bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Identity servislerini ekle
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Buraya EmailSender servisini ekle
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error"); // İstersen production için hata sayfası
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// EmailSender sınıfı Program.cs'nin sonunda ya da ayrı bir dosyada olabilir
public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // SMTP veya başka mail gönderme kodunu buraya ekle
        return Task.CompletedTask;
    }
}
