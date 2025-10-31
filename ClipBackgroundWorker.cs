using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DATALADS_CRICKET
{
    public class ClipBackgroundWorker
    {
        private readonly BlockingCollection<(string device, string clipPath)> _clipQueue = new();

        public ClipBackgroundWorker()
        {
            // Start worker thread
            Task.Run(ProcessQueue);
        }

        public void EnqueueClip(string deviceName, string clipPath)
        {
            _clipQueue.Add((deviceName, clipPath));
        }

        private async Task ProcessQueue()
        {
            foreach (var (device, clipPath) in _clipQueue.GetConsumingEnumerable())
            {
                try
                {
                    await Task.Run(() =>
                    {
                        string? dir = Path.GetDirectoryName(clipPath);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir!);

                        string logPath = Path.Combine(dir!, "ffmpeg_log_async.txt");

                        var proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "ffmpeg",
                                Arguments = $"-y -f dshow -i video=\"{device}\" -t 00:00:13 -c:v libx264 -preset ultrafast -pix_fmt yuv420p \"{clipPath}\"",
                                RedirectStandardError = true,
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };

                        proc.Start();
                        string log = proc.StandardError.ReadToEnd();
                        proc.WaitForExit();

                        File.WriteAllText(logPath, log);
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Clip save failed: {ex.Message}");
                }
            }
        }
    }
}


