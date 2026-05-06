# Altın Kasap Izgara — QR Dijital Menü Sistemi

Altın Kasap Izgara Restoran için geliştirilmiş, mobil-first QR kod tabanlı dijital menü uygulaması. Müşteriler masadaki QR kodu tarayarak menüyü görüntüleyebilir, sipariş verebilir ve ödeme yapabilir.

## Teknoloji Yığını

- **Framework**: ASP.NET Core 8 MVC
- **Database**: MySQL 8 (Pomelo EF Core provider)
- **ORM**: Entity Framework Core 8
- **Authentication**: ASP.NET Core Identity
- **UI Framework**: Bootstrap 5
- **QR Üretimi**: QRCoder
- **Görsel İşleme**: SixLabors.ImageSharp
- **Logging**: Serilog (File, Console sinks)

## Kurulum

### Ön Koşullar
- .NET 8 SDK
- MySQL 8 Server
- Git

### Adımlar

1. **Repoyu klonla**
   ```bash
   git clone https://github.com/toolshubteam-creator/altin-kasap-qr-menu.git
   cd altin-kasap-qr-menu
   ```

2. **NuGet paketlerini yükle**
   ```bash
   dotnet restore
   ```

3. **appsettings.Development.json ayarla**
   `AltinKasap.Web/appsettings.Development.json` dosyasını düzenle:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=altinkasap;User=root;Password=SENIN_SIFREN;"
     }
   }
   ```

4. **Veritabanı migrationlarını uygula** (Adım 2'de)
   ```bash
   dotnet ef database update
   ```

5. **Projeyi çalıştır**
   ```bash
   cd AltinKasap.Web
   dotnet run
   ```
   Tarayıcıda `https://localhost:5001` aç

## Klasör Yapısı

```
AltinKasap.Web/
├── Models/           # Entity sınıfları (Ürün, Kategori, Sipariş, vb.)
├── ViewModels/       # View-specific veri sınıfları
├── Services/         # İş mantığı servisleri
├── Repositories/     # Data access layer
├── Controllers/      # MVC denetleyicileri
├── Views/            # Razor şablonları
│   ├── Admin/        # Yönetim paneli view'leri
│   └── Menu/         # Müşteri menü view'leri
├── Data/             # EF Core DbContext
│   └── Migrations/   # Migration dosyaları
├── wwwroot/uploads/  # Yüklenen resimler (Ürün, QR, Restoran)
└── Properties/       # launchSettings.json, vb.
```

## Lisans

MIT

## İletişim

Sorular ve öneriler için: [iletişim bilgileri]
