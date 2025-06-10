using FaceRecognitionClient.Commands;
using FaceRecognitionClient.StateMachine;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.Authentication
{
    /// <summary>
    /// ViewModel responsible for CAPTCHA generation and validation.
    /// Acts as a bot protection step in the authentication flow.
    /// </summary>
    public class CaptchaViewModel : BaseViewModel, IStateNotifier
    {
        public event Action<ApplicationTrigger> OnTriggerOccurred;

        // CAPTCHA settings
        private const int CaptchaWidth = 160;
        private const int CaptchaHeight = 50;
        private const int CaptchaLength = 6;
        private const int MinFontSize = 20;
        private const int MaxFontSize = 26;
        private const int NoiseDotCount = 100;
        private const int NoiseLineCount = 5;
        private const int RotationAngleRange = 15;
        private const int CharYOffsetMin = 5;
        private const int CharYOffsetMax = 15;
        private const int CharXJitter = 3;

        // Allowed CAPTCHA characters (excluding confusing ones like I, l, 0, O)
        private static readonly string AllowedCharacters = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789abcdefghijkmnopqrstuvwxyz";

        private string m_CaptchaCode;
        private string m_UserInput;
        private BitmapImage m_CaptchaImage;

        public string UserInput
        {
            get => m_UserInput;
            set { m_UserInput = value; OnPropertyChanged(); }
        }

        public BitmapImage CaptchaImage
        {
            get => m_CaptchaImage;
            private set { m_CaptchaImage = value; OnPropertyChanged(); }
        }

        public RelayCommand RefreshCaptchaCommand { get; }
        public RelayCommand VerifyCaptchaCommand { get; }

        public CaptchaViewModel()
        {
            RefreshCaptchaCommand = new RelayCommand(_ => GenerateCaptcha());
            VerifyCaptchaCommand = new RelayCommand(_ => VerifyCaptcha());
            GenerateCaptcha(); // generate a CAPTCHA on startup
        }

        /// <summary>
        /// Generates a new CAPTCHA code and image, resets user input.
        /// </summary>
        private void GenerateCaptcha()
        {
            m_CaptchaCode = GenerateRandomCode();                       // Step 1: generate new code
            CaptchaImage = GenerateCaptchaImage(m_CaptchaCode);        // Step 2: render new image
            UserInput = string.Empty;                                  // Step 3: clear previous user input
        }

        /// <summary>
        /// Validates the user's input against the generated CAPTCHA code.
        /// If successful, triggers navigation to the next window.
        /// Otherwise, regenerates a new CAPTCHA.
        /// </summary>
        private void VerifyCaptcha()
        {
            if (string.Equals(UserInput, m_CaptchaCode, StringComparison.OrdinalIgnoreCase))
            {
                OnTriggerOccurred?.Invoke(ApplicationTrigger.CaptchaPassed);
            }
            else
            {
                GenerateCaptcha(); // refresh challenge on failure
            }
        }

        /// <summary>
        /// Generates a random alphanumeric string to be used as the CAPTCHA challenge.
        /// </summary>
        private string GenerateRandomCode()
        {
            var rand = new Random();
            return new string(Enumerable.Range(0, CaptchaLength)
                .Select(_ => AllowedCharacters[rand.Next(AllowedCharacters.Length)]).ToArray());
        }

        /// <summary>
        /// Creates a distorted CAPTCHA image using drawing tools.
        /// Adds noise, lines, rotation, and jitter to each character to prevent OCR-based attacks.
        /// </summary>
        private BitmapImage GenerateCaptchaImage(string code)
        {
            using var bmp = new Bitmap(CaptchaWidth, CaptchaHeight);
            using var g = Graphics.FromImage(bmp);
            var rand = new Random();

            // === Step 1: Background Color ===
            g.Clear(Color.FromArgb(rand.Next(220, 255), rand.Next(220, 255), rand.Next(220, 255)));

            // === Step 2: Add Noise Dots ===
            for (int i = 0; i < NoiseDotCount; i++)
            {
                int x = rand.Next(CaptchaWidth);
                int y = rand.Next(CaptchaHeight);
                var dotColor = Color.FromArgb(rand.Next(150, 200), rand.Next(150, 200), rand.Next(150, 200));
                bmp.SetPixel(x, y, dotColor);
            }

            // === Step 3: Add Noise Lines ===
            for (int i = 0; i < NoiseLineCount; i++)
            {
                var penColor = Color.FromArgb(rand.Next(100, 180), rand.Next(100, 180), rand.Next(100, 180));
                var pen = new Pen(penColor);
                var x1 = rand.Next(CaptchaWidth);
                var y1 = rand.Next(CaptchaHeight);
                var x2 = rand.Next(CaptchaWidth);
                var y2 = rand.Next(CaptchaHeight);
                g.DrawLine(pen, x1, y1, x2, y2);
            }

            // === Step 4: Draw CAPTCHA Characters ===
            int charWidth = CaptchaWidth / code.Length;
            for (int i = 0; i < code.Length; i++)
            {
                // Random font size and style for each character
                var fontSize = rand.Next(MinFontSize, MaxFontSize);
                var style = (FontStyle)(1 << rand.Next(0, 3));
                using var font = new Font("Arial", fontSize, style);

                // Choose random dark color for the character
                using var brush = new SolidBrush(Color.FromArgb(rand.Next(10, 80), rand.Next(10, 80), rand.Next(10, 80)));

                // Randomize character position and rotation
                float x = i * charWidth + rand.Next(-CharXJitter, CharXJitter);
                float y = rand.Next(CharYOffsetMin, CharYOffsetMax);
                float angle = rand.Next(-RotationAngleRange, RotationAngleRange);

                // Rotate and draw each character
                g.TranslateTransform(x, y);
                g.RotateTransform(angle);
                g.DrawString(code[i].ToString(), font, brush, 0, 0);
                g.ResetTransform(); // reset for the next character
            }

            // === Step 5: Convert to BitmapImage ===
            return ConvertBitmapToBitmapImage(bmp);
        }

        /// <summary>
        /// Converts System.Drawing.Bitmap to WPF BitmapImage so it can be displayed in XAML.
        /// </summary>
        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze(); // makes it thread-safe

            return image;
        }
    }
}
