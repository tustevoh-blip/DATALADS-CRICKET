using System;
using System.Collections.Generic;

namespace DATALADS_CRICKET
{
    public class Player
    {
        public string Name { get; set; }
        public int Runs { get; set; }
        public int Balls { get; set; }
        public int Fours { get; set; }
        public int Sixes { get; set; }
        public bool IsOut { get; set; }

        public Player(string name)
        {
            Name = name;
        }

        public string Display()
        {
            double sr = Balls > 0 ? (Runs * 100.0 / Balls) : 0;
            return $"{Name}: {Runs}({Balls}) SR {sr:0.0}";
        }
    }

    public class Bowler
    {
        public string Name { get; set; }
        public int Overs { get; set; }
        public int BallsInOver { get; set; }
        public int RunsConceded { get; set; }
        public int Wickets { get; set; }

        public Bowler(string name)
        {
            Name = name;
        }

        public string Display()
        {
            return $"{Name}: {Wickets}/{RunsConceded} in {Overs}.{BallsInOver}";
        }
    }

    public class ScoreManager
    {
        public int TotalRuns { get; private set; }
        public int Wickets { get; private set; }
        public int Overs { get; private set; }
        public int BallsInOver { get; private set; }

        public Player Striker { get; private set; }
        public Player NonStriker { get; private set; }
        public Bowler CurrentBowler { get; private set; }

        public List<string> DeliveryLog { get; } = new List<string>();

        public ScoreManager(string striker, string nonStriker, string bowler)
        {
            Striker = new Player(striker);
            NonStriker = new Player(nonStriker);
            CurrentBowler = new Bowler(bowler);
        }

        public void RecordRun(int runs)
        {
            TotalRuns += runs;
            Striker.Runs += runs;
            Striker.Balls++;
            CurrentBowler.RunsConceded += runs;
            AddBall();

            // Strike rotation for odd runs
            if (runs % 2 != 0)
                SwapStrike();

            LogDelivery($"{runs} run(s)");
        }

        public void RecordExtra(string type)
        {
            TotalRuns++;
            CurrentBowler.RunsConceded++;
            LogDelivery($"Extra: {type}");
        }

        public void RecordWicket()
        {
            Wickets++;
            Striker.IsOut = true;
            Striker.Balls++;
            CurrentBowler.Wickets++;
            AddBall();
            LogDelivery("WICKET");
        }

        private void AddBall()
        {
            BallsInOver++;
            if (BallsInOver >= 6)
            {
                BallsInOver = 0;
                Overs++;
                SwapStrike();
            }
        }

        public void UndoLast()
        {
            if (DeliveryLog.Count > 0)
                DeliveryLog.RemoveAt(DeliveryLog.Count - 1);
        }

        public double GetRunRate()
        {
            double totalBalls = Overs * 6 + BallsInOver;
            return totalBalls > 0 ? (TotalRuns * 6.0 / totalBalls) : 0;
        }

        private void LogDelivery(string desc)
        {
            string overDisplay = $"{Overs}.{BallsInOver}";
            DeliveryLog.Add($"{overDisplay}: {desc}");
        }

        public string ScoreSummary()
        {
            return $"{TotalRuns}/{Wickets} ({Overs}.{BallsInOver} ov)";
        }

        public void SwapStrike()
        {
            var temp = Striker;
            Striker = NonStriker;
            NonStriker = temp;
        }
    }
}


