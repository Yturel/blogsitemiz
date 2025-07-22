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
