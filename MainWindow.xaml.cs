using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DATALADS_CRICKET
{
    public partial class MainWindow : Window
    {
        // ✅ Video + HUD
        private Process? ffmpegProcess;
        private CancellationTokenSource? cts;
        private bool isPreviewRunning = false;
        private string? currentDevice;
        private DispatcherTimer hudTimer;
        private string lastMetaHud = "";

        // ✅ Match State
        private int totalRuns = 0;
        private int totalWickets = 0;
        private int ballsThisOver = 0;
        private int overs = 0;
        private string striker = "Unknown Batsman";
        private string nonStriker = "Unknown Non-Striker";
        private string bowler = "Unknown Bowler";

        public MainWindow()
        {
            InitializeComponent();
            LoadVideoDevices();
            InitializeHud();
            UpdateScoreDisplay();
        }

        // ✅ HUD always on
        private void InitializeHud()
        {
            hudTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            hudTimer.Tick += (s, e) => { TxtHud.Text = lastMetaHud; };
            hudTimer.Start();
        }

        // ✅ Load Cameras
        private void LoadVideoDevices()
        {
            try
            {
                var devices = new List<string>();
                var ffmpeg = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = "-hide_banner -f dshow -list_devices true -i dummy",
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                ffmpeg.Start();
                string output = ffmpeg.StandardError.ReadToEnd();
                ffmpeg.WaitForExit();

                var matches = Regex.Matches(output, @"\[dshow @.+?\]\s+\""(.*?)\""");
                foreach (Match match in matches)
                {
                    string device = match.Groups[1].Value;
                    if (!device.Contains("Alternative"))
                        devices.Add(device);
                }

                CmbVideoSource.ItemsSource = devices;
                if (devices.Count > 0)
                    CmbVideoSource.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading devices: {ex.Message}");
            }
        }

        // ✅ Start Preview
        private void BtnStartPreview_Click(object sender, RoutedEventArgs e)
        {
            var device = CmbVideoSource.SelectedItem as string;
            if (string.IsNullOrEmpty(device))
            {
                MessageBox.Show("Select a video source first.");
                return;
            }

            StartPreview(device);
        }

        // ✅ Stop Preview
        private void BtnStopPreview_Click(object sender, RoutedEventArgs e)
        {
            StopPreview();
        }

        private void StartPreview(string deviceName)
        {
            try
            {
                StopPreview();
                cts = new CancellationTokenSource();

                currentDevice = deviceName;
                int width = 640, height = 360, bytesPerPixel = 3;
                int frameSize = width * height * bytesPerPixel;

                string args =
                    $"-hwaccel auto -threads 2 -f dshow -rtbufsize 256M -i video=\"{deviceName}\" " +
                    $"-pix_fmt bgr24 -vf scale={width}:{height}:flags=bicubic -f rawvideo -";

                ffmpegProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = args,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                ffmpegProcess.Start();
                lastMetaHud = $"{width}x{height} | {deviceName}";

                Dispatcher.Invoke(() =>
                {
                    ImgPreview.Source = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);
                });

                var bmp = (WriteableBitmap)ImgPreview.Source;
                var buffer = new byte[frameSize];
                var stream = ffmpegProcess.StandardOutput.BaseStream;
                isPreviewRunning = true;

                Task.Run(() =>
                {
                    try
                    {
                        while (!ffmpegProcess.HasExited && !cts.Token.IsCancellationRequested)
                        {
                            int bytesRead = stream.Read(buffer, 0, frameSize);
                            if (bytesRead < frameSize) break;

                            Dispatcher.Invoke(() =>
                            {
                                bmp.WritePixels(new Int32Rect(0, 0, width, height), buffer, width * bytesPerPixel, 0);
                            });
                        }
                    }
                    catch { }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Preview failed: {ex.Message}");
            }
        }

        private void StopPreview()
        {
            try
            {
                if (isPreviewRunning)
                {
                    cts?.Cancel();
                    if (ffmpegProcess != null && !ffmpegProcess.HasExited)
                    {
                        ffmpegProcess.Kill();
                        ffmpegProcess.Dispose();
                    }
                    isPreviewRunning = false;
                }
            }
            catch { }
        }

        // ✅ SCORE BUTTONS
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag == null) return;

            int runs = int.Parse(btn.Tag.ToString());
            totalRuns += runs;
            ballsThisOver++;

            SaveClipForBall($"{runs} runs");
            UpdateScoreDisplay();
        }

        private void WicketButton_Click(object sender, RoutedEventArgs e)
        {
            totalWickets++;
            ballsThisOver++;
            SaveClipForBall("W");

            UpdateScoreDisplay();
        }

        private void ExtraButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag == null) return;

            string extra = btn.Tag.ToString();
            totalRuns++;
            SaveClipForBall(extra);

            UpdateScoreDisplay();
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Undo logic coming soon...");
        }

        private void UpdateScoreDisplay()
        {
            LblScore.Text = $"Total: {totalRuns}/{totalWickets} ({overs}.{ballsThisOver} ov)";
            LblStriker.Text = $"Striker: {striker}";
            LblNonStriker.Text = $"Non-Striker: {nonStriker}";
        }

        // ✅ SAVE CLIP
        private void SaveClipForBall(string eventName)
        {
            if (currentDevice == null) return;

            string clipDir = @"C:\DATALADS APP CLIPS";
            if (!Directory.Exists(clipDir))
                Directory.CreateDirectory(clipDir);

            string filename = $"{DateTime.Now:HHmmss}_{eventName.Replace(" ", "")}.mp4";
            string clipPath = Path.Combine(clipDir, filename);

            lastMetaHud = $"Clip queued: {filename}";
            TxtHud.Text = lastMetaHud;

            Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $"-y -f dshow -i video=\"{currentDevice}\" -t 00:00:05 -c:v libx264 -preset ultrafast \"{clipPath}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();
            });
        }
    }
}


