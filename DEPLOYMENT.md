# Deployment Kılavuzu

Bu doküman Altın Kasap QR Menü sisteminin production ortamına nasıl yayınlanacağını anlatır.

## 1. Yayın Çıktısı (Build)

Geliştirme makinende:

```
dotnet publish AltinKasap.Web/AltinKasap.Web.csproj -c Release -o ./publish
```

`./publish` klasörü tüm çalıştırılabilir dosyaları içerir (`.dll`, `appsettings.json`, `wwwroot/`).

## 2. Windows + IIS

### Önkoşullar
- Windows Server 2019+ veya Windows 10/11
- IIS yüklü
- [.NET 8 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0) yüklü
- MySQL 8.0+ servisi çalışır durumda

### Adımlar

1. `./publish` klasörünü `C:\inetpub\wwwroot\altinkasap` altına kopyala.

2. IIS Manager'da yeni site oluştur:
   - Site adı: `AltinKasap`
   - Physical path: `C:\inetpub\wwwroot\altinkasap`
   - Binding: `http` veya `https`, port 80/443

3. Application Pool ayarları:
   - **.NET CLR Version: No Managed Code**
   - **Identity:** ApplicationPoolIdentity (veya custom)

4. Environment variable veya `appsettings.Production.json` üzerinden konfigürasyon:
   ```cmd
   setx ASPNETCORE_ENVIRONMENT "Production" /M
   setx ConnectionStrings__DefaultConnection "Server=...;Database=altinkasap;User=...;Password=...;" /M
   setx DefaultAdmin__Email "owner@altinkasap.com" /M
   setx DefaultAdmin__Password "GUCLU_SIFRE" /M
   setx AppSettings__MenuBaseUrl "https://menu.altinkasap.com" /M
   ```

5. Klasör izinleri:
   - `wwwroot\uploads\` ve `logs\` üzerinde IIS_IUSRS yazma izni

6. Migration'ları uygula:
   ```
   dotnet ef database update --project AltinKasap.Web
   ```

7. SSL sertifikası bağla (örnek: [win-acme](https://www.win-acme.com/) ile Let's Encrypt).

## 3. Linux + Nginx

### Önkoşullar
- Ubuntu 22.04+ veya Debian 11+
- .NET 8 Runtime: `sudo apt install dotnet-runtime-8.0 aspnetcore-runtime-8.0`
- MySQL 8.0: `sudo apt install mysql-server`
- Nginx: `sudo apt install nginx`
- Certbot (Let's Encrypt): `sudo apt install certbot python3-certbot-nginx`

### Adımlar

1. **Çıktıyı sunucuya kopyala:**
   ```
   scp -r ./publish/* user@sunucu:/var/www/altinkasap
   ```

2. **systemd service dosyası:** `/etc/systemd/system/altinkasap.service`
   ```ini
   [Unit]
   Description=Altin Kasap QR Menu
   After=network.target mysql.service

   [Service]
   WorkingDirectory=/var/www/altinkasap
   ExecStart=/usr/bin/dotnet /var/www/altinkasap/AltinKasap.Web.dll
   Restart=always
   RestartSec=10
   KillSignal=SIGINT
   SyslogIdentifier=altinkasap
   User=www-data
   Environment=ASPNETCORE_ENVIRONMENT=Production
   Environment=ASPNETCORE_URLS=http://localhost:5000
   Environment=ConnectionStrings__DefaultConnection=Server=localhost;Database=altinkasap;User=altinkasap;Password=GUCLU
   Environment=DefaultAdmin__Email=owner@altinkasap.com
   Environment=DefaultAdmin__Password=GUCLU_ADMIN_SIFRE
   Environment=AppSettings__MenuBaseUrl=https://menu.altinkasap.com

   [Install]
   WantedBy=multi-user.target
   ```

3. **Servisi başlat:**
   ```
   sudo systemctl daemon-reload
   sudo systemctl enable altinkasap
   sudo systemctl start altinkasap
   sudo systemctl status altinkasap
   ```

4. **Nginx reverse proxy:** `/etc/nginx/sites-available/altinkasap`
   ```nginx
   server {
       listen 80;
       server_name menu.altinkasap.com;

       client_max_body_size 12M;

       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
           proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
           proxy_set_header X-Forwarded-Proto $scheme;
           proxy_set_header X-Real-IP $remote_addr;
       }
   }
   ```

5. **Site'ı aktif et:**
   ```
   sudo ln -s /etc/nginx/sites-available/altinkasap /etc/nginx/sites-enabled/
   sudo nginx -t
   sudo systemctl reload nginx
   ```

6. **SSL sertifikası (Let's Encrypt):**
   ```
   sudo certbot --nginx -d menu.altinkasap.com
   ```

7. **MySQL kullanıcı oluştur** (root yerine ayrı kullanıcı):
   ```sql
   CREATE USER 'altinkasap'@'localhost' IDENTIFIED BY 'GUCLU_SIFRE';
   CREATE DATABASE altinkasap CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   GRANT ALL PRIVILEGES ON altinkasap.* TO 'altinkasap'@'localhost';
   FLUSH PRIVILEGES;
   ```

8. **Migration:**
   ```
   cd /var/www/altinkasap
   dotnet ef database update
   ```
   `dotnet-ef` global tool kurulu olmalı (`dotnet tool install --global dotnet-ef --version 8.*`).

## 4. Backup Stratejisi

### MySQL Backup (Linux cron)

`/etc/cron.daily/altinkasap-backup`:
```bash
#!/bin/bash
DATE=$(date +%Y%m%d-%H%M%S)
mysqldump -u altinkasap -p'GUCLU_SIFRE' altinkasap | gzip > /var/backups/altinkasap-$DATE.sql.gz
find /var/backups -name "altinkasap-*.sql.gz" -mtime +30 -delete
```

```
sudo chmod +x /etc/cron.daily/altinkasap-backup
```

### Uploads Backup
`/var/www/altinkasap/wwwroot/uploads/` klasörünü `rsync` veya `restic` ile uzak depolamaya yedekle.

## 5. Sorun Giderme

| Belirti | Çözüm |
|---------|-------|
| 502 Bad Gateway | `sudo systemctl status altinkasap` — service çöktü mü kontrol et, journalctl ile log'a bak |
| Migration hatası | MySQL bağlantı + connection string env var doğru mu kontrol et |
| Görseller yüklenmiyor | `wwwroot/uploads/` klasörü `www-data` sahipliğinde mi |
| `logs/` klasörü dolu | Serilog 14 gün tutar, otomatik temizler — manuel müdahaleye normalde gerek yok |
| Türkçe karakterler bozuk | MySQL collation `utf8mb4_unicode_ci` mi |
| 429 Too Many Requests | Rate limit tetiklendi (60/dk menu, 5/dk login) — IP bazlı, 1 dakika beklenmeli |
| Logout sonrası hâlâ giriş yapmış görünüyor | Cookie `AltinKasap.Auth` SameSite=Strict, tarayıcı önbelleğini temizleyin |

## 6. Güncelleme

```
# Yerel
dotnet publish AltinKasap.Web/AltinKasap.Web.csproj -c Release -o ./publish

# Sunucu (Linux örneği)
scp -r ./publish/* user@sunucu:/var/www/altinkasap
ssh user@sunucu "sudo systemctl restart altinkasap"
```

Migration gerektiren değişikliklerde:
```
ssh user@sunucu "cd /var/www/altinkasap && dotnet ef database update"
```
(önce uygulamayı durdurmak iyi pratiktir; production'da downtime hassasiyetinizi gözeterek yapın)
