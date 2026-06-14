# 📊 Proje Yönetimi ve İş Akışı Paneli

> **Zirai İlaç Üretim Kapasitesi Artırımı & Akıllı Depolama Entegrasyonu**  
> Akademik ve endüstriyel standartlarda zirai ilaç üretim kapasitesini 2 katına çıkaracak yeni reaktör montajı, saha genişletilmesi ve IoT tabanlı akıllı depolama sisteminin anahtar teslim entegrasyonu projesi.

---

## 📋 İçindekiler

- [Proje Özeti](#-proje-özeti)
- [Teknik Mimari](#-teknik-mimari)
- [Modüller ve Özellikler](#-modüller-ve-özellikler)
- [Gantt Şeması ve Görev Planı](#-gantt-şeması-ve-görev-planı)
- [Kritik Yol (CPM) Analizi](#-kritik-yol-cpm-analizi)
- [Proje Ekibi](#-proje-ekibi)
- [Bütçe ve Sermaye Yönetimi](#-bütçe-ve-sermaye-yönetimi)
- [Görev Durumları](#-görev-durumları)
- [API Dokümantasyonu](#-api-dokümantasyonu)
- [Kurulum ve Kullanım](#-kurulum-ve-kullanım)

---

## 🚀 Proje Özeti

| Alan | Bilgi |
|------|-------|
| **Proje Adı** | Zirai İlaç Üretim Kapasitesi Artırımı & Akıllı Depolama Entegrasyonu |
| **Durum** | 🟡 Devam Ediyor |
| **Öngörülen Bütçe** | 750.000 TL |
| **Aktif Sermaye** | 1.200.000 TL |
| **Başlangıç** | Proje başlangıcından itibaren dinamik olarak hesaplanır |
| **Yöntem** | Critical Path Method (CPM) + Gantt Şeması |
| **Platform** | ASP.NET Core MVC (.NET 8) |

### Proje Amacı

Bu ERP modülü, zirai ilaç sektöründe faaliyet gösteren işletmelerin proje süreçlerini yönetmelerini sağlamak amacıyla geliştirilmiştir. Temel hedefler:

- ✅ Üretim kapasitesini **%100 artırmak** (yeni reaktör montajı)
- ✅ IoT tabanlı **akıllı depolama sistemi** entegrasyonu (RFID + Sensör)
- ✅ **CPM algoritması** ile kritik görevlerin otomatik belirlenmesi
- ✅ Gerçek zamanlı **bütçe ve sermaye takibi**
- ✅ Ekip bazlı **iş yükü görselleştirmesi** (Gantt)

---

## 🏗 Teknik Mimari

```
zirai_ilac/
├── ZiraiIlacERPWeb/
│   ├── Controllers/
│   │   └── ProjectManagementController.cs   # CRUD + API endpoint'leri
│   ├── Models/
│   │   └── ProjectManagement.cs             # Veri modelleri
│   ├── Services/
│   │   └── ProjectManagementService.cs      # İş mantığı + CPM algoritması
│   ├── Views/
│   │   └── ProjectManagement/
│   │       └── Index.cshtml                 # UI (Gantt, CPM, Ekip, Bütçe)
│   └── wwwroot/
│       └── data/
│           └── project_management.json      # JSON tabanlı veri deposu
└── ZiraiIlacERPAPI/                         # REST API projesi
```

### Kullanılan Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| **Backend** | ASP.NET Core 8 MVC |
| **Veri Saklama** | JSON dosyası (`wwwroot/data/project_management.json`) |
| **Algoritma** | Critical Path Method (CPM) — İleri/Geri geçiş hesabı |
| **Frontend** | Vanilla JS + CSS (Dark glassmorphism tasarım) |
| **Görselleştirme** | Takvim uyumlu Gantt Şeması (CSS Grid tabanlı) |

---

## 🧩 Modüller ve Özellikler

### 1. 📈 Genel Durum Paneli (Dashboard)
- Toplam görev, tamamlanan görev ve kritik görev sayacı
- Proje ilerleme çubuğu (ağırlıklı ortalama)
- Sermaye ve bütçe dengesi kartları
- Proje finansal durumu özeti

### 2. 📅 Gantt Şeması
- **Göreve göre Gantt:** Her görevin başlangıç/bitiş tarihine göre takvim yerleşimi
- **Ekibe göre Gantt:** Her ekip üyesinin iş yükü çizelgesi
- Kritik görevler kırmızı (animasyonlu), normal görevler yeşil gösterilir
- İlerleme yüzdesi bar içinde görsel olarak belirtilir

### 3. 🏗️ Görevler ve CPM (Critical Path Method)
- CPM algoritması: **İleri geçiş (ES/EF)** + **Geri geçiş (LS/LF)** + **Slack** hesabı
- Kritik yol görevleri otomatik işaretlenir (`Slack = 0`)
- Görev ekleme, düzenleme ve silme (AJAX tabanlı)
- Bağımlılık ilişkileri (öncel görev tanımı)

### 4. 👥 Proje Ekibi
- Ekip üyesi kartları (avatar, rol, uzmanlık, iletişim)
- Üye ekleme/düzenleme/silme

### 5. 💰 Bütçe ve Sermaye Yönetimi
- Gelir/Gider takibi (hareket bazlı)
- Kasa bakiyesi hesabı
- Bütçe kullanım yüzdesi
- İşlem ekleme/silme

### 6. 📋 Raporlar
- Proje özet raporu
- Sermaye akış tablosu

---

## 📅 Gantt Şeması ve Görev Planı

Aşağıdaki tablo, projedeki görevlerin CPM değerleri ile birlikte listesini göstermektedir:

| ID | Görev Adı | Süre (Gün) | Öncel Görev | İlerleme | Kritik? |
|----|-----------|-----------|-------------|----------|---------|
| 1 | Fizibilite & Pazar Analizi Raporu | 8 gün | — | ✅ %100 | ✅ Kritik |
| 2 | İlaç Formülasyonu & Hammadde Onayı | 12 gün | 1 | ✅ %100 | ✅ Kritik |
| 3 | Reaktör ve Ekipman İthalat Siparişi | 15 gün | 2 | ✅ %100 | ✅ Kritik |
| 4 | Üretim Alanı Zemin & Altyapı Hazırlığı | 20 gün | 1 | 🟡 %90 | ❌ |
| 5 | Yeni Reaktör Kurulumu ve Montajı | 14 gün | 3, 4 | 🟡 %35 | ✅ Kritik |
| 6 | Akıllı Depo RFID & Sensör Yazılımı | 22 gün | 2 | 🟡 %50 | ❌ |
| 7 | Depo Sensör Altyapısı Donanım Montajı | 10 gün | 4, 6 | 🔴 %10 | ❌ |
| 8 | Sistem Entegrasyonu & Pilot Denemeler | 7 gün | 5, 7 | ⬜ %0 | ✅ Kritik |
| 9 | Kabul Testleri & Kapanış Raporlaması | 4 gün | 8 | ⬜ %0 | ✅ Kritik |

> **Toplam proje süresi:** CPM algoritması ile dinamik hesaplanır (yaklaşık 52 gün minimum)

---

## 🏗️ Kritik Yol (CPM) Analizi

### Algoritma Açıklaması

`ProjectManagementService.cs` içindeki `ComputeCriticalPath()` metodu şu adımları izler:

```
1. İleri Geçiş (Forward Pass)
   ES[i] = max(EF[j]) for all j in predecessors(i)
   EF[i] = ES[i] + Duration[i]

2. Geri Geçiş (Backward Pass)  
   LF[i] = min(LS[j]) for all j in successors(i)
   LS[i] = LF[i] - Duration[i]

3. Slack Hesabı
   Slack[i] = LS[i] - ES[i]
   IsCritical = (Slack == 0)
```

### Kritik Yol

```
[1] Fizibilite (8g) → [2] Formülasyon (12g) → [3] Reaktör Siparişi (15g)
                                                        ↓
                                               [5] Reaktör Montajı (14g)
                                                        ↓
                                               [8] Entegrasyon (7g)
                                                        ↓
                                               [9] Kabul Testleri (4g)
```

**Kritik yol süresi: ~60 gün** (bağımlılıklara ve eş zamanlı görevlere göre değişir)

---

## 👥 Proje Ekibi

| # | Ad | Rol | Uzmanlık |
|---|-----|-----|----------|
| 👨‍💼 | Doç. Dr. Selim Aksoy | Proje Yöneticisi | Proje Yönetimi & CPM |
| 👩‍🔬 | Dr. Elif Yılmaz | Kimya Ar-Ge Lideri | Kimyasal Formülasyon |
| 👨‍💻 | Murat Can | IoT & Otomasyon Müh. | Gömülü Sistemler & Yazılım |
| 👩‍💼 | Zeynep Demir | Tedarik & Satın Alma | Sözleşme & Lojistik |
| 👷 | Hasan Kaya | Saha Şefi | İnşaat & Donanım Kurulumu |

---

## 💰 Bütçe ve Sermaye Yönetimi

### Finansal Özet

| Kalem | Tutar |
|-------|-------|
| 💰 Öngörülen Proje Bütçesi | 750.000 TL |
| 💼 Aktif Kasa Sermayesi | 1.200.000 TL |
| 📥 Toplam Gelir/Destek | 1.200.000 TL |
| 📤 Toplam Harcama | 485.000 TL |
| 📊 Net Bakiye | +715.000 TL |

### Sermaye Hareketleri

| # | Tür | Kategori | Tutar | Açıklama |
|---|-----|----------|-------|----------|
| 1 | 🟢 Gelir | Sermaye | 800.000 TL | Kurucu ortaklar nakdi sermaye aktarımı |
| 2 | 🟢 Gelir | Hibe Desteği | 400.000 TL | KOSGEB/TÜBİTAK Proje 1. dönem Ar-Ge desteği |
| 3 | 🔴 Gider | Ekipman | 280.000 TL | Cam astarlı reaktör ithalat bedeli |
| 4 | 🔴 Gider | Altyapı | 95.000 TL | Tesis zemin epoksi kaplama ve havalandırma |
| 5 | 🔴 Gider | Yazılım & Lisans | 45.000 TL | RFID entegrasyonu ve bulut lisans bedeli |
| 6 | 🔴 Gider | Hammadde | 65.000 TL | Deneme üretimi için ön hammadde tedariki |

---

## 🔌 API Dokümantasyonu

`ProjectManagementController.cs` aşağıdaki endpoint'leri sunar:

### Görev (Task) Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `GET` | `/ProjectManagement` | Proje yönetimi ana sayfası |
| `POST` | `/ProjectManagement/AddTask` | Yeni görev ekle |
| `POST` | `/ProjectManagement/UpdateTaskProgress` | Görev ilerleme güncelle |
| `POST` | `/ProjectManagement/DeleteTask` | Görev sil |

### Ekip (Team) Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `POST` | `/ProjectManagement/AddTeamMember` | Ekip üyesi ekle |
| `POST` | `/ProjectManagement/DeleteTeamMember` | Ekip üyesi sil |

### Finansal Endpoint'ler

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `POST` | `/ProjectManagement/AddTransaction` | Gelir/Gider hareketi ekle |
| `POST` | `/ProjectManagement/DeleteTransaction` | Hareket sil |

### Proje Meta

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `POST` | `/ProjectManagement/UpdateProjectMeta` | Proje adı, bütçe, tarih güncelle |

---

## ⚙️ Kurulum ve Kullanım

### Gereksinimler

- .NET 8 SDK
- Visual Studio 2022 veya VS Code

### Çalıştırma

```bash
# Repoyu klonlayın
git clone https://github.com/Merve4717/zirai_ilac.git

# Web projesine gidin
cd zirai_ilac/ZiraiIlacERPWeb

# Bağımlılıkları yükleyin
dotnet restore

# Uygulamayı başlatın
dotnet run
```

Uygulama çalıştıktan sonra tarayıcıda şu adrese gidin:
```
https://localhost:5001/ProjectManagement
```

### Veri Dosyası

İlk çalıştırmada `wwwroot/data/project_management.json` dosyası otomatik olarak örnek verilerle oluşturulur. Bu dosyayı silinerek veri sıfırlanabilir.

---

## 📝 Geliştirme Notları

- CPM algoritması her veri okuma/yazma işleminde otomatik yeniden hesaplanır
- Veriler JSON dosyasında saklanmaktadır; veritabanına geçiş için `ProjectManagementService.cs` sınıfındaki CRUD metodları değiştirilmelidir
- Gantt şeması CSS Grid ile saf JavaScript kullanılarak render edilmekte, ekstra kütüphane bağımlılığı bulunmamaktadır
- Tüm modal formlar AJAX ile çalışır, sayfa yenileme gerekmez

---

## 🔗 İlgili Dosyalar

| Dosya | Açıklama |
|-------|----------|
| [`ProjectManagementController.cs`](ZiraiIlacERPWeb/Controllers/ProjectManagementController.cs) | HTTP endpoint'leri ve yönlendirme |
| [`ProjectManagementService.cs`](ZiraiIlacERPWeb/Services/ProjectManagementService.cs) | İş mantığı, CPM algoritması |
| [`ProjectManagement.cs`](ZiraiIlacERPWeb/Models/ProjectManagement.cs) | Veri modelleri (Project, Task, Team, Transaction) |
| [`Index.cshtml`](ZiraiIlacERPWeb/Views/ProjectManagement/Index.cshtml) | UI katmanı (Gantt, CPM, Dashboard) |
| [`project_management.json`](ZiraiIlacERPWeb/wwwroot/data/project_management.json) | JSON veri deposu |

---

*Bu modül [AgroERP Zirai İlaç ERP Sistemi](README.md) projesinin bir parçasıdır.*
