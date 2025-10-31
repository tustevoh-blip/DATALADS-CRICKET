using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace DATALADS_CRICKET
{
    public static class ClipManager
    {
        private static string ffmpegPath = "ffmpeg"; // Assumes FFmpeg is in PATH
        private static string clipFolder = @"C:\DATALADS APP CLIPS";

        /// <summary>
        /// Records a lightweight 13-second clip (10s before + 3s after save).
        /// </summary>
        /// <param name="sourceName">Video source name (from dropdown)</param>
        /// <param name="ballLabel">Optional descriptive filename e.g. "0.4 Smith to Joe 6runs"</param>
        public static async Task RecordBallClip(string sourceName, string ballLabel)
        {
            try
            {
                if (!Directory.Exists(clipFolder))
                    Directory.CreateDirectory(clipFolder);

                // Sanitize filename (remove invalid characters)
                string safeLabel = string.Join("_", ballLabel.Split(Path.GetInvalidFileNameChars()));
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string fileName = $"{timestamp}_{safeLabel}.mp4";
                string outputPath = Path.Combine(clipFolder, fileName);

                // FFmpeg command: capture short clip from selected video source
                string args =
                    $"-f dshow -i video=\"{sourceName}\" " +
                    "-preset ultrafast -t 13 -c:v libx264 -pix_fmt yuv420p " +
                    "-b:v 2500k -movflags +faststart " +
                    $"\"{outputPath}\"";

                await Task.Run(() =>
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = args,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };

                    using (Process process = Process.Start(psi))
                    {
                        process.WaitForExit();
                    }
                });

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"🎬 Clip saved:\n{outputPath}", "Clip Recorded", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"❌ Clip recording failed:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }
    }
}


