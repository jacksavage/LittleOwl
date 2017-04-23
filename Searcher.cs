namespace LittleOwl {
    using System;
    using System.Timers;

    internal partial class Searcher {
        public Searcher() {
            Countdown = new Timer();
            Countdown.Elapsed += ClockElapsed; // attach handler to elapsed event
            Countdown.AutoReset = false; // only raise event once
            Timeout = false;
        }

        public Move Search(Board root, int depth, int qDepth, int alpha, int beta, TimeSpan time) {
            // guards
            if (depth < 1 || qDepth < 1) throw new ArgumentException("valid depth limits are greater than zero");
            if (alpha > beta) throw new ArgumentException("alpha cannot be greater than beta");

            // start countdown
            Countdown.Interval = time.TotalMilliseconds;
            Countdown.Start();

            Node RootNode = new Node(root);
            Node.TopLevelActivePlayerWhite = root.ActiveColorWhite; // store for utility eval

            Move Result = null;
            int Value = int.MinValue;
            int NextDepth;

            for (int d = 1; d <= depth; d++) { // step through depths
                NextDepth = d - 1;
                foreach (Node child in RootNode.Children) { // step through children
                    if (Result == null) Result = child.LastMove; // default to first move
                    if (Timeout) return Result; // return best move if timed out
                    Value = MinV(child, NextDepth, qDepth, alpha, beta); // get minimized value from new board
                    if (Value > alpha) { alpha = Value; Result = child.LastMove; } // update alpha and best move if better val found
                }
            }

            return Result;
        }

        // find minimized utility relative to the active player from a given board
        private int MinV(Node n, int d, int q, int a, int b) {
            if (Timeout || d == 0 || n.Terminal) return n.Utility;

            int Value = int.MaxValue;
            int NextDepth = d - 1;

            foreach (Node child in n.Children) {
                Value = MaxV(child, NextDepth, q, a, b);
                if (Value <= a) return Value; // failed low (prune)
                if (Value < b) b = Value;
            }

            return Value;
        }

        // find maximized utility relative to the active player from a given board
        private int MaxV(Node n, int d, int q, int a, int b) {
            if (Timeout || d == 0 || n.Terminal) return n.Utility;

            int Value = int.MinValue;
            int NextDepth = d - 1;

            foreach (Node child in n.Children) {
                Value = MinV(child, NextDepth, q, a, b);
                if (Value >= b) return Value; // failed high (prune)
                if (Value > a) a = Value;
            }

            return Value;
        }

        private Timer Countdown;
        private bool Timeout;
        private void ClockElapsed(object sender, ElapsedEventArgs e) { Timeout = true; }
    }
}
