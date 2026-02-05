# 🛒 Listify - Alışveriş Listesi Yönetim Uygulaması

## 📝 Proje Hakkında
Listify, günlük alışveriş ihtiyaçlarınızı düzenli ve organize bir şekilde yönetmenizi sağlayan modern bir web uygulamasıdır. ASP.NET Core 10.0 teknolojisi ile geliştirilmiş olup, SQLite veritabanı kullanmaktadır.

## 📝 Hazırlayanlar
- Hüseyin Cevher Aslan (132130049)
- Muhammet Emin Kocaman (132130041)
- Meylis Charyev (132130121)

## 🚀 Özellikler
- ✅ Çoklu liste yönetimi
- 📦 Ürün ekleme, düzenleme ve silme
- 📁 Kategori yönetimi
- ⭐ Favori ürünleri kaydetme
- 📊 Detaylı alışveriş istatistikleri ve grafikler
- 📱 Mobil uyumlu, responsive tasarım
- 🎨 Modern ve kullanıcı dostu arayüz
- 🖨️ Liste yazdırma
- 📄 CSV dışa aktarma
- 🔍 Ürün arama ve filtreleme

## 🛠️ Teknolojiler
- **Backend:** ASP.NET Core 10.0 MVC
- **Veritabanı:** Entity Framework Core 10.0 + SQLite
- **Frontend:** Bootstrap 5.3, Font Awesome 6
- **Grafik:** Chart.js 4.4.0
- **Logging:** Serilog

## 📋 Kurulum

### Gereksinimler
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Adımlar

1. **Projeyi klonlayın veya indirin**
```bash
git clone <repository-url>
cd Listify
```

2. **Projeyi restore edin**
```bash
dotnet restore
```

3. **Projeyi build edin**
```bash
dotnet build
```

4. **Uygulamayı çalıştırın**
```bash
dotnet run
```

5. **Tarayıcıda açın**
Uygulama genellikle `http://localhost:5000` veya `https://localhost:5001` adresinde çalışır.

## 📁 Proje Yapısı

```
Listify/
├── Controllers/          # MVC Controllers
├── Models/              # Veri modelleri
├── Views/               # Razor Views
├── Services/            # İş mantığı servisleri
├── Data/                # Veritabanı context
├── wwwroot/
│   ├── css/            # Özel CSS dosyaları
│   └── js/             # JavaScript dosyaları
├── Program.cs           # Uygulama başlangıç noktası
└── appsettings.json    # Konfigürasyon dosyası
```

## 🎯 Kullanım

1. **Liste Oluştur:** Ana sayfadan "Yeni Liste" butonuna tıklayarak yeni bir alışveriş listesi oluşturun.
2. **Ürün Ekle:** Listeye ürün eklemek için "Ürün Ekle" butonunu kullanın.
3. **Kategori Yönetimi:** Ürünleri kategorilere ayırarak düzenli tutun.
4. **Favoriler:** Sık kullandığınız ürünleri favorilere ekleyin.
5. **İstatistikler:** Alışveriş alışkanlıklarınızı istatistikler sayfasından takip edin.

## 📸 Ekran Görüntüleri

### Ana Sayfa
- Tüm listelerinizi görüntüleyin
- Hızlı ürün ekleme
- İstatistik özeti

### Ürünler Sayfası
- Ürün ekleme/düzenleme
- Checkbox ile ürün işaretleme
- Filtreleme ve arama

### İstatistikler Sayfası
- Kategori dağılımı grafiği
- Liste bazlı istatistikler
- Fiyat özeti

## 📝 Lisans
Bu proje eğitim amaçlı geliştirilmiştir.

## 🔗 Bağlantılar
- Youtube Videosu: https://www.youtube.com/watch?v=BY0fYei3vGc
