using System;
using System.IO;

namespace DATALADS_CRICKET
{
    public class BallEvent
    {
        public string Over { get; set; } = "0.0";       // Example: 12.4
        public string Bowler { get; set; } = "Unknown";
        public string Batsman { get; set; } = "Unknown";
        public string Result { get; set; } = "0 runs";  // Example: 4 runs, Wicket, etc.

        public BallEvent() { }

        public BallEvent(string over, string bowler, string batsman, string result)
        {
            Over = over;
            Bowler = bowler;
            Batsman = batsman;
            Result = result;
        }

        // ✅ Filename-friendly clip name
        public string ToFileName()
        {
            string safe = $"{Over}_{Bowler}_to_{Batsman}_{Result}".Replace(" ", "");
            foreach (char c in Path.GetInvalidFileNameChars())
                safe = safe.Replace(c, '_');

            return safe + ".mp4";
        }

        // ✅ For HUD display / logs
        public string ToDisplayString()
        {
            return $"{Over} | {Bowler} to {Batsman} | {Result}";
        }
    }
}


