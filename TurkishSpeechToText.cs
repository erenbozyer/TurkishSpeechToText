using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vosk;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace TurkishSpeechToText
{
    public partial class TurkishSpeechToText : Form
    {
        private Model model;
        private VoskRecognizer recognizer;
        private WaveInEvent waveIn;
        private bool isRecording = false;

        public TurkishSpeechToText()
        {
            InitializeComponent();
            InitializeModelSelector();
            LoadInputDevices();

        }
        //ComboBox (Model seçici) başlangıç ayarı
        private void InitializeModelSelector()
        {
            comboModels.Items.Add("English - vosk-model-small-en-us-0.15");
            comboModels.Items.Add("Turkish - vosk-model-small-tr-0.3");
            comboModels.SelectedIndex = 0; // varsayılan İngilizce
            LoadSelectedModel();
        }


        //Seçili modele göre model yükle
        private void LoadSelectedModel()
        {
            try
            {
                // Önce varsa eski model temizle
                recognizer?.Dispose();
                model?.Dispose();

                string selectedModel = comboModels.SelectedItem?.ToString() ?? "";

                if (selectedModel.Contains("English"))
                    model = new Model("Model-en");
                else if (selectedModel.Contains("Turkish"))
                    model = new Model("Model-tr");

                recognizer = new VoskRecognizer(model, 16000);

                lblStatus.Text = "✅ Model başarıyla yüklendi: " + selectedModel;
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"⚠️ Model yüklenemedi!\n📁 Model yolunu kontrol et.\n\n{ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }
        // 🔹 3️⃣ ComboBox değiştiğinde çağrılır
        private void comboModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopRecording();
            LoadSelectedModel();
        }

        private void LoadInputDevices()
        {
            cmbDevices.Items.Clear();
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var caps = WaveIn.GetCapabilities(i);
                cmbDevices.Items.Add($"{i}: {caps.ProductName}");
            }
            if (cmbDevices.Items.Count > 0)
                cmbDevices.SelectedIndex = 0;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadInputDevices();
        }
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isRecording)
                {
                    StartRecording();
                }
                else
                {
                    StopRecording();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Başlatma hatası: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }


        // 🔹 5️⃣ Kaydı başlat
        private void StartRecording()
        {
            if (recognizer == null)
            {
                lblStatus.Text = "⚠️ Model hazır değil!";
                lblStatus.ForeColor = Color.OrangeRed;
                return;
            }

            try
            {
                waveIn = new WaveInEvent();
                waveIn.DeviceNumber = 0; // varsayılan mikrofon
                waveIn.WaveFormat = new WaveFormat(16000, 1); // 16kHz mono
                waveIn.BufferMilliseconds = 1000; // 1 saniye buffer

                waveIn.DataAvailable += (s, a) =>
                {
                    if (recognizer.AcceptWaveform(a.Buffer, a.BytesRecorded))
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                string resultJson = recognizer.Result();
                                string text = ExtractTextFromJson(resultJson);
                                txtResult.Text = text;
                                lblStatus.Text = "🗣️ Konuşma algılandı!";
                                lblStatus.ForeColor = Color.MediumSeaGreen;
                            }));
                        }
                    }
                };

                waveIn.StartRecording();
                isRecording = true;
                btnStartStop.Text = "🛑 DURDUR";
                btnStartStop.BackColor = Color.Firebrick;
                lblStatus.Text = "🎙️ Dinleniyor...";
                lblStatus.ForeColor = Color.DodgerBlue;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"💥 Mikrofon başlatılamadı!\n🔧 {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }

        // 🔹 6️⃣ Kaydı durdur
        private void StopRecording()
        {
            try
            {
                waveIn?.StopRecording();
                waveIn?.Dispose();
                waveIn = null;

                isRecording = false;
                btnStartStop.Text = "▶️ BAŞLAT";
                btnStartStop.BackColor = Color.ForestGreen;
                lblStatus.Text = "⏹️ Durduruldu.";
                lblStatus.ForeColor = Color.Gray;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Durdurma hatası: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }

        // 🔹 7️⃣ JSON içinden "text" kısmını çıkar
        private string ExtractTextFromJson(string jsonResult)
        {
            try
            {
                if (jsonResult.Contains("\"text\""))
                {
                    int textStart = jsonResult.IndexOf("\"text\"");
                    int colonIndex = jsonResult.IndexOf(":", textStart);
                    int quoteStart = jsonResult.IndexOf("\"", colonIndex) + 1;
                    int quoteEnd = jsonResult.IndexOf("\"", quoteStart);
                    if (quoteStart > 0 && quoteEnd > quoteStart)
                    {
                        return jsonResult.Substring(quoteStart, quoteEnd - quoteStart);
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }



        // 🔹 8️⃣ Form kapanırken kaynakları temizle
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopRecording();
            recognizer?.Dispose();
            model?.Dispose();
            base.OnFormClosing(e);
        }

        private void AppendText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            txtResult.Invoke((MethodInvoker)delegate
            {
                txtResult.AppendText(Environment.NewLine + text);
            });
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (recognizer != null)
            {
                if (recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                {
                    var result = recognizer.Result();
                    AppendText(result);
                }
                else
                {
                    var partial = recognizer.PartialResult();
                    if (!string.IsNullOrWhiteSpace(partial))
                    {
                        // JSON içinden "partial" kısmını al
                        try
                        {
                            var json = JObject.Parse(partial);
                            var partialText = json["partial"]?.ToString();

                            if (!string.IsNullOrEmpty(partialText))
                            {
                                // TextBox’a anlık olarak göster
                                txtResult.Invoke((MethodInvoker)delegate
                                {
                                    txtResult.Text = partialText;
                                });
                            }
                        }
                        catch
                        {
                            // JSON parse hatası olursa sessiz geç
                        }
                    }
                }
            }
        }
    }
}
