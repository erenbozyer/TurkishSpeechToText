# 🎙️ Speech to Text (C# - Windows Forms - Vosk)

Bu proje, **mikrofondan alınan sesi gerçek zamanlı olarak yazıya çeviren** bir Windows Forms uygulamasıdır.  
Vosk açık kaynak konuşma tanıma kütüphanesi ve NAudio ses işleme bileşeni kullanılmıştır.  
Proje **Azure veya Google API** gibi ücretli servisler kullanmadan, tamamen **yerel (offline)** çalışır.

---

## 🧩 Özellikler

- 🎤 **Gerçek zamanlı konuşma tanıma**
- 🌍 **Birden fazla dil modeli desteği (İngilizce / Türkçe)**
- 🔄 **Hızlı Model değiştirme (comboBox ile dinamik)**
- ⏸️ **Tek butonla başlat / durdur**
- 🚫 **İnternet bağlantısı gerekmez**
- 💾 **Yerel olarak çalışan, gizlilik dostu sistem**

---

## 🛠️ Kullanılan Teknolojiler

| Teknoloji | Açıklama |
|------------|-----------|
| **C# (.NET)** | Ana geliştirme dili |
| **Windows Forms** | Arayüz için |
| **Vosk API** | Ses tanıma motoru |
| **NAudio** | Mikrofon ses verisini almak için |

---

## ⚙️ Kurulum

1. **Gereksinimler:**
   - Visual Studio 2022+
   - .NET 6.0 SDK veya üzeri
   - NuGet paketleri:
     ```
     Vosk
     NAudio
     ```

2. **Vosk modellerini indir:**
   - İngilizce model:  
     👉 [vosk-model-en-us-0.22](https://alphacephei.com/vosk/models)
   - Türkçe model (küçük boyutlu):  
     👉 [vosk-model-small-tr-0.3](https://alphacephei.com/vosk/models)

3. **Klasörlere çıkar:**

---

## ▶️ Kullanım

1. Uygulamayı çalıştır.
2. Üstteki **ComboBox**’tan kullanmak istediğin dil modelini seç.
3. 🎙️ **“Başlat”** butonuna bas.
4. Mikrofonuna konuş.
5. Konuşma tanınırsa metin kutusunda gerçek zamanlı olarak yazıya döner.
6. **“Durdur”** butonuna basarak kaydı sonlandır.

---

## 🧠 Notlar

- İlk başlatmada model yüklenmesi birkaç saniye sürebilir.
- Mikrofon seçimi sistem varsayılan girişine göre yapılır.
- Uygulama kapanırken tüm kaynaklar (`Model`, `Recognizer`, `WaveIn`) güvenli biçimde serbest bırakılır.

---
