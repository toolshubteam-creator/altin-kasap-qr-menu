# Altın Kasap Izgara — QR Dijital Menü Sistemi

ASP.NET Core MVC 8 + MySQL ile geliştirilmiş, üçüncü taraf bağımlılığı olmayan, tamamen kendi altyapınızda çalışan modern QR dijital menü sistemi.

## Özellikler

### Müşteri Tarafı
- Mobil-öncelikli (mobile-first), Bootstrap 5 ile responsive tasarım
- Dark / Light mode (kullanıcı tercihi localStorage'da saklanır)
- Kategori bazlı smooth scroll navigasyon
- Aktif duyuru bantları
- Günün Önerisi vitrini
- Allerjen ve diyet etiketleri (Glutensiz, Vejetaryen, Vegan, Acılı, Süt İçeren, Şefin Önerisi, Yeni)
- "Tükendi" işareti
- Menüyü yazdır / PDF olarak kaydet
- 5 dakika cache ile hızlı yükleme

### Admin Paneli
- ASP.NET Core Identity ile güvenli kimlik doğrulama
- Dashboard: KPI kartları + son 7 gün scan grafiği + en çok taranan QR'lar
- Kategori yönetimi (drag-drop sıralama)
- Ürün yönetimi (drag-drop, görsel yükleme + WebP optimizasyonu, etiket atama, fiyat geçmişi, "Tükendi" toggle)
- QR Kod yönetimi (özel renk + logo, PNG/SVG indirme, A4 baskı şablonu, scan tracking)
- Detaylı raporlama (günlük, haftalık, aylık, yıllık + QR bazlı)
- Restoran ayarları (logo, kapak görseli, sosyal medya, ana renk)
- Duyurular (tarih bazlı otomatik aktif/deaktif)
- Günün Önerisi yönetimi

### Güvenlik
- ASP.NET Core Identity + bcrypt
- CSRF koruması (tüm POST formlarında ve drag-drop API'sinde)
- Rate limiting: login 5/dk, public menu 60/dk (IP bazlı sabit pencere)
- HTTPS yönlendirme + HSTS
- Güvenlik başlıkları: X-Frame-Options, X-Content-Type-Options, Referrer-Policy, Permissions-Policy
- Hesap kilitleme (5 başarısız denemeden sonra)
- Serilog ile yapısal loglama (dosya + console, 14 gün rotasyon)
- WCAG 2.1 uyumlu zoom (kullanıcı zoom'u engellenmemiştir)

## Teknoloji Yığını

| Katman | Teknoloji |
|--------|-----------|
| Framework | ASP.NET Core MVC 8 |
| ORM | Entity Framework Core 8 (Pomelo MySQL Provider) |
| Veritabanı | MySQL 8.0+ (utf8mb4) |
| Auth | ASP.NET Core Identity |
| Görsel İşleme | SixLabors.ImageSharp (WebP @ Q80) |
| QR Kod | QRCoder (ECC-H, opsiyonel logo embedding) |
| Loglama | Serilog (Console + File) |
| Frontend | Bootstrap 5.3, Chart.js 4, Font Awesome 6, SortableJS |
| Cache | IMemoryCache (5 dk public menu) |

## Kurulum

### Önkoşullar
- .NET 8 SDK
- MySQL 8.0+
- Git

### Adımlar

1. **Repoyu klonla:**
   ```
   git clone https://github.com/toolshubteam-creator/altin-kasap-qr-menu.git
   cd altin-kasap-qr-menu
   ```

2. **Bağımlılıkları yükle:**
   ```
   dotnet restore
   ```

3. **MySQL bağlantısını yapılandır.** `AltinKasap.Web/appsettings.Development.json` oluştur veya düzenle:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=altinkasap;User=root;Password=YOUR_PASSWORD;"
     },
     "DefaultAdmin": {
       "Email": "admin@altinkasap.local",
       "Password": "Admin123!"
     }
   }
   ```
   Bu dosya `.gitignore` içinde, commit edilmez.

4. **Migration'ları uygula** (Pomelo MySQL provider veritabanını otomatik oluşturur):
   ```
   dotnet ef database update --project AltinKasap.Web
   ```

5. **Uygulamayı başlat:**
   ```
   dotnet run --project AltinKasap.Web
   ```

6. **Tarayıcıda aç:**
   - Müşteri menüsü: `http://localhost:5000/`
   - Kısa URL: `http://localhost:5000/altin-kasap`
   - Admin paneli: `http://localhost:5000/admin/login`
   - Default admin: `admin@altinkasap.local` / `Admin123!` (production'da mutlaka değiştirin)

`dotnet ef` aracı yoksa: `dotnet tool install --global dotnet-ef --version 8.*`

## Klasör Yapısı

```
AltinKasap.Web/
├── Controllers/
│   ├── Admin/              # SettingsController, CategoriesController, ProductsController,
│   │                       # QrCodesController, AnnouncementsController, DailySpecialsController,
│   │                       # ReportsController
│   ├── Api/                # SortController (drag-drop)
│   ├── AccountController   # Login / Logout / AccessDenied
│   ├── AdminController     # Dashboard
│   ├── ErrorController     # 404 / 500 / 429
│   └── MenuController      # Public menu (with ?qr= scan tracking)
├── Data/
│   ├── AppDbContext.cs
│   ├── DbSeeder.cs         # Admin user + restaurant + 6 kategori + 29 ürün + 7 etiket
│   └── Migrations/
├── Helpers/
│   └── SlugHelper.cs       # Türkçe karakter folding
├── Models/                 # 11 entity
├── Repositories/           # 10 repository (generic + entity-specific)
├── Services/               # MenuService, QrService, ReportService, ImageService
├── ViewModels/
├── Views/
│   ├── Shared/             # _Layout (public), _AdminLayout, _ReportsNav
│   ├── Menu/               # Public menü
│   ├── Account/, Admin/, Categories/, Products/, QrCodes/, Reports/,
│   │   Settings/, Announcements/, DailySpecials/, Error/
├── wwwroot/
│   ├── css/                # site.css (public), admin.css
│   ├── js/                 # site.js (public), admin.js
│   └── uploads/            # Kullanıcı yüklenen görseller (git ignore)
├── Program.cs              # DI + middleware pipeline + rate limiter + güvenlik headers
├── appsettings.json
└── appsettings.Development.json (git ignore)
```

## Scan Tracking & QR URL'leri

QR kodlar `/menu?qr={id}` formatında URL üretir. Bu URL ziyaret edildiğinde
`MenuController.Index` fire-and-forget bir `QrScanLog` kaydı oluşturur (kullanıcı
deneyimini engellemez). QR ID yoksa scan log `QrCodeId = NULL` olarak düşer.

QR için kullanılacak base URL şu sırayla seçilir:
1. `appsettings.json` → `AppSettings:MenuBaseUrl`
2. Ortam değişkeni `AppSettings__MenuBaseUrl`
3. Mevcut HTTP isteğinin scheme + host'u

## Deployment

Detaylı kılavuz: [DEPLOYMENT.md](DEPLOYMENT.md)

Özet:
- **Windows / IIS:** `dotnet publish -c Release` + IIS site + .NET Hosting Bundle
- **Linux / Nginx:** `dotnet publish -c Release` + systemd service + Nginx reverse proxy + Let's Encrypt SSL

## Production Öncesi Checklist

- [ ] `appsettings.Production.json` veya environment variable ile gerçek connection string
- [ ] `DefaultAdmin:Password` environment variable üzerinden güçlü şifreyle değiştir
- [ ] HTTPS sertifikası (Let's Encrypt veya kurumsal)
- [ ] `ASPNETCORE_ENVIRONMENT=Production` environment variable
- [ ] `AppSettings:MenuBaseUrl` production domain'i (QR kodları doğru URL üretmeli)
- [ ] MySQL backup stratejisi (mysqldump cron job)
- [ ] `wwwroot/uploads/` klasörü için disk kotası ve yazma izni
- [ ] `logs/` klasörü için disk kotası (Serilog 14 gün tutar, otomatik temizler)
- [ ] Firewall: dış erişim sadece 80/443, MySQL 3306 internal
- [ ] Default admin'e güvenli rotasyon: kendi admin hesabını oluştur, default'u sil/devre dışı bırak
- [ ] `[v2 için not]` Content-Security-Policy: CDN bağımlılıkları self-host'a alındıktan sonra eklenmeli

## Geliştirme Notları

- Tüm admin değişiklikleri `IMenuService.InvalidateCache()` çağırır → public sayfada anında yansır
- Türkçe karakterler view tarafında `HtmlEncoder.Create(UnicodeRanges.All)` ile raw UTF-8 olarak çıkar
- Drag-drop sıralama yalnızca admin oturumunda + antiforgery header ile çalışır
- Görsel yüklemeleri ImageSharp ile WebP @ Q80'e çevirilir, max 5 MB / 800×600 (ürün), 400×400 (logo), 1600×600 (kapak)
- ECC-H seviyesinde QR kodlar %30 hasara dayanıklı, ortaya logo eklemek okunabilirliği bozmaz

## Lisans

MIT — Bkz. [LICENSE](LICENSE) dosyası.
