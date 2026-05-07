# Roadmap

Sistem v1.0.0 ile production'a hazır. Aşağıdaki özellikler gelecek sürümler için planlanmıştır.

## v1.1 — Polish & Analitik (önerilen sıradaki sürüm)

- **Ürün popülerlik raporu** — En çok görüntülenen ürünler
  - `ProductView` entity (`ProductId`, `ViewedAt`, `IpHash`)
  - IntersectionObserver ile viewport-based tracking (sayfa başına 1 kez)
  - Yatay sütun grafiği `/admin/reports/products`
- **Saat bazlı yoğunluk** — Hangi saatlerde daha çok scan oluyor (mevcut `QrScanLog` verisinden hesaplanır)
- **Dil altyapısı (TR/EN)** — `IStringLocalizer` + Resource dosyaları (düşük öncelikli)
- **Content Security Policy header** — CDN URL'lerine whitelist

## v2.0 — Yeni Özellikler

- **Masadan garson çağırma** (QR ID üzerinden masa kimliği zaten var)
- **Push bildirim altyapısı** (admin'e yeni scan ve garson çağrı bildirimi)
- **Online ödeme** (iyzico veya Param entegrasyonu)
- **Çoklu şube yönetimi** (Restaurant tablosu zaten N kayıt destekliyor)
- **Müşteri yorumu / puanlama** (ürün bazlı 1–5 yıldız)
- **Stok takibi** (ürün satıldıkça otomatik `IsSoldOut`)

## v3.0 — İleri Düzey

- **Mobil uygulama** (React Native veya Flutter)
- **WhatsApp sipariş entegrasyonu**
- **AI destekli menü önerisi** (mevsim, hava, geçmiş davranış)
