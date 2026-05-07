# Production Geçiş Checklist

Bu sistem production'a alınmadan önce aşağıdaki adımların TÜMÜ tamamlanmalıdır.

---

## 1. Sunucu Hazırlığı

- [ ] .NET 8 Hosting Bundle (Windows IIS) veya `aspnetcore-runtime-8.0` (Linux) kurulu
- [ ] MySQL 8.0+ kurulu, varsayılan collation `utf8mb4_unicode_ci`
- [ ] HTTPS sertifikası hazır (Let's Encrypt veya kurumsal CA)
- [ ] Domain DNS kaydı sunucu IP'sine yönlendirilmiş

---

## 2. MySQL Kullanıcı (root kullanma!)

```sql
CREATE USER 'altinkasap'@'localhost' IDENTIFIED BY 'GUCLU_SIFRE';
CREATE DATABASE altinkasap CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
GRANT ALL PRIVILEGES ON altinkasap.* TO 'altinkasap'@'localhost';
FLUSH PRIVILEGES;
```

---

## 3. Environment Variables (ZORUNLU)

- [ ] `ASPNETCORE_ENVIRONMENT=Production`
- [ ] `ConnectionStrings__DefaultConnection` (production MySQL connection string)
- [ ] `DefaultAdmin__Email` (gerçek email)
- [ ] `DefaultAdmin__Password` (güçlü şifre, min 12 karakter — ⚠️ `Admin123!` ASLA OLMAZ)
- [ ] `AppSettings__MenuBaseUrl` (örn. `https://menu.altinkasap.com`) — ⚠️ kritik, QR kodlar bu URL ile üretilecek

---

## 4. Dağıtım

- [ ] `dotnet publish -c Release -o ./publish` çalıştırıldı
- [ ] Çıktı sunucuya kopyalandı
- [ ] systemd (Linux) veya IIS site (Windows) yapılandırıldı (DEPLOYMENT.md referansıyla)
- [ ] `dotnet ef database update` çalıştırıldı, 17 tablo oluştu

---

## 5. İlk Çalıştırma

- [ ] Uygulama başlatıldı, `Now listening` log'u görüldü
- [ ] HTTPS ile menü sayfası açılıyor
- [ ] `/admin/login` → ENV'deki email + şifre ile giriş çalışıyor
- [ ] Default seed admin (`admin@altinkasap.local` / `Admin123!`) varsa DERHAL SİL veya şifresini değiştir

---

## 6. Güvenlik

- [ ] HTTPS yönlendirmesi aktif (`UseHttpsRedirection`)
- [ ] HSTS header geliyor (production)
- [ ] `X-Frame-Options`, `X-Content-Type-Options`, `Referrer-Policy` header'ları geliyor (curl ile doğrula)
- [ ] Firewall: 80/443 dış, 3306 sadece localhost
- [ ] `wwwroot/uploads/` ve `logs/` klasörlerinde uygulama kullanıcısı yazma izni

---

## 7. Yedekleme

- [ ] MySQL backup cron job kuruldu (DEPLOYMENT.md'deki örnek)
- [ ] `wwwroot/uploads/` rsync veya restic ile uzak depoya yedekleniyor
- [ ] `logs/` klasörü disk kotası kontrol edildi (Serilog 14 gün rotasyon)
- [ ] İlk yedek manuel test edildi (restore'a hazır)

---

## 8. İzleme

- [ ] Sunucu uptime izleme (UptimeRobot, Pingdom vb.)
- [ ] Disk kullanım uyarısı kuruldu
- [ ] Serilog dosyaları periyodik gözden geçirilecek

---

## 9. Müşteri Rolü

- [ ] Restoran Ayarları dolduruldu (gerçek logo, kapak görseli, adres, telefon, sosyal medya)
- [ ] Ürün görselleri yüklendi (29 ürünün her biri için)
- [ ] Ürünlere uygun etiketler atandı (Glutensiz, Vejetaryen, Acılı vb.)
- [ ] Masa sayısı kadar QR kod oluşturuldu (Masa 1, Masa 2, …)
- [ ] QR kodlar A4 olarak yazdırıldı, masa standlarına yerleştirildi

---

## 10. İlk 24 Saat

- [ ] İlk gerçek scan'leri `/admin/reports/qr-codes` üzerinden gözle
- [ ] Mobil cihazlarda farklı tarayıcılarda menü sayfasını test et
- [ ] Türkçe karakterler düzgün görünüyor mu kontrol et
