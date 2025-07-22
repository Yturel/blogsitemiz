
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WebApplication5.Areas.Identity.Pages.Account;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Formatters;

public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string? ImageFile { get; set; } // Resim yolunu tutacak özellik
    public DateTime? olusturulanZaman { get; set; } = DateTime.Now.AddHours(2);
    public string? PaylasanKullanici { get; set; }

    public string Kategori { get; set; }
   
    public string FontFamily { get; set; }


    //Bu son eklediğim fonksiyon debug modunda patlıyor ayrıca details.cshtml kısmında h5 etiketi ile de başvuru mevcud.
    //artık patlamıyor, devam.
}