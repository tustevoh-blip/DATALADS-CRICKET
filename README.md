DATALADS Cricket Scorer
A Windows desktop application for live cricket scoring with video integration and automated ball-by-ball clip saving, designed for broadcasters and digital cricket scoring.

🎯 Features
✔ Live Video Preview from any connected camera (Webcam, Capture Card, Cam Link 4K, etc.)
 ✔ HUD Overlay – Displays video metadata like resolution & FPS directly on the video feed
 ✔ Ball-by-ball Clip Recording:
Records 10 seconds before and 3 seconds after each ball is scored


Automatically names video clips using: Over_Bowler_to_Batsman_Result.mp4


Saves clips to C:\DATALADS APP CLIPS
 ✔ Real-time Cricket Scoring:


Buttons for 0,1,2,3,4,6, Wicket, No-Ball, Wide, Leg-Bye


Striker & Non-Striker stats shown live


Over & ball count automatically updated
 ✔ Undo Last Ball support
 ✔ Lightweight UI using WPF (.NET 8)
 ✔ FFmpeg-powered lightweight recording



🖥 Tech Stack
Component
Technology Used
Framework
.NET 8 (WPF – Windows Desktop)
Language
C#
Video Processing
FFmpeg & FFprobe
UI Library
XAML (WPF)
OS Support
Windows 10 / 11
Clip Format
MP4 (H.264, YUV420p)


📁 Project Structure
DATALADS CRICKET/
├── MainWindow.xaml              → UI Layout
├── MainWindow.xaml.cs           → Main logic (UI + Video + Scoring)
├── ClipManager.cs               → Handles FFmpeg-based video recording
├── BallEvent.cs                 → Stores ball info (batsman, bowler, result)
├── TeamSetup.xaml (optional)    → Team / Player setup UI
├── App.xaml                     → App startup
└── README.md                    → Project documentation


⚙ How to Run
✅ Requirements
✔ Windows 10 or 11


✔ .NET 8 Runtime / SDK installed


✔ FFmpeg installed & added to PATH


✔ Visual Studio 2022 (with WPF & .NET Desktop Development)


🚀 Run the App
Clone the project:

 git clone https://github.com/tustevoh-blip/DATALADS-CRICKET.git


Open .sln file in Visual Studio


Press Start / F5



🎥 Video Recording Logic
Recording Type
Duration
Pre-ball buffer
10 seconds before Save
Post-ball continuation
3 seconds after Save
Clip Format
.mp4
Saved To
C:\DATALADS APP CLIPS

Filename format example:
0.4_Smith_to_Joe_6runs.mp4


✅ To-Do / Upcoming Features
Add match setup (teams, overs, toss, venue)


Add ball-by-ball timeline panel


Export match stats to Excel / PDF


NDI Output for live TV broadcast overlays


Score overlay with team logos



🧑‍💻 Developer
Tusiime Steven (DataLads Media)
 📧 Email: sales@dataladsmedia.com
 📺 YouTube: @dataladsmedia

📝 License
This project is currently private and not open-source.
 Contact the author for collaboration or commercial use.

