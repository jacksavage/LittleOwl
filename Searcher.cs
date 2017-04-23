namespace LittleOwl {
    using System;
    using System.Collections.Generic;
    using System.Timers;

    internal class Searcher {
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

            WhiteAtMove = root.ActiveColorWhite; // store for utility eval
            Move Result = null;
            int Value = int.MinValue;
            int NextDepth;

            for (int d = 1; d <= depth; d++) { // step through depths
                NextDepth = d - 1;
                foreach (BoardMovePair child in Children(root)) { // step through children
                    if (Result == null) Result = child.move; // default to first move
                    if (Timeout) return Result; // return best move if timed out
                    Value = MinV(child.board, NextDepth, qDepth, alpha, beta); // get minimized value from new board
                    if (Value > alpha) { alpha = Value; Result = child.move; } // update alpha and best move if better val found
                }
            }

            return Result;
        }

        // find minimized utility relative to the active player from a given board
        private int MinV(Board board, int depth, int qDepth, int alpha, int beta) {
            if (Timeout || depth == 0 || Terminal(board)) return Utility(board);

            int Value = int.MaxValue;
            int NextDepth = depth - 1;

            foreach (BoardMovePair child in Children(board)) {
                Value = MaxV(child.board, NextDepth, qDepth, alpha, beta);
                if (Value <= alpha) return Value; // failed low (prune)
                if (Value < beta) beta = Value;
            }

            return Value;
        }

        // find maximized utility relative to the active player from a given board
        private int MaxV(Board board, int depth, int qDepth, int alpha, int beta) {
            if (Timeout || depth == 0 || Terminal(board)) return Utility(board);

            int Value = int.MinValue;
            int NextDepth = depth - 1;

            foreach (BoardMovePair child in Children(board)) {
                Value = MinV(child.board, NextDepth, qDepth, alpha, beta);
                if (Value >= beta) return Value; // failed high (prune)
                if (Value > alpha) alpha = Value;
            }

            return Value;
        }

        private IEnumerable<BoardMovePair> Children(Board board) {
            throw new NotImplementedException();
        }

        // is this an end-of-game state?
        private bool Terminal(Board board) { return InCheckMate(board) || InDraw(board); }

        // the utility of a given board to the active player
        private int Utility(Board board) {
            if (InCheckMate(board)) return int.MinValue;
            if (InDraw(board)) return int.MinValue + 1;
            return MaterialAdvantage(board);
        }

        // is this an inactive state?
        private bool Quiescent(Board board) {
            throw new NotImplementedException();
        }

        // is the active player in check mate?
        private bool InCheckMate(Board board) {
            throw new NotImplementedException();
        }

        // is the active player in draw?
        private bool InDraw(Board board) {
            throw new NotImplementedException();
        }

        // the material advantange of the active player
        private int MaterialAdvantage(Board board) {
            if (WhiteAtMove) return MaterialValue(board.Pieces.White) - MaterialValue(board.Pieces.Black);
            else return MaterialValue(board.Pieces.Black) - MaterialValue(board.Pieces.White);
        }

        // the material value for a player
        private int MaterialValue(PiecePositions.Player player) {
            int Result = 0;
            foreach (ulong pawn in Utilities.BitSplit(player.Pawns)) Result += PawnValue;
            foreach (ulong knight in Utilities.BitSplit(player.Knights)) Result += KnightValue;
            foreach (ulong bishop in Utilities.BitSplit(player.Knights)) Result += BishopValue;
            foreach (ulong rook in Utilities.BitSplit(player.Knights)) Result += RookValue;
            foreach (ulong queen in Utilities.BitSplit(player.Knights)) Result += QueenValue;
            return Result;
        }

        private const int PawnValue = 1;
        private const int KnightValue = 3;
        private const int BishopValue = 3;
        private const int RookValue = 5;
        private const int QueenValue = 9;

        private bool WhiteAtMove;
        private Timer Countdown;
        private bool Timeout;
        private void ClockElapsed(object sender, ElapsedEventArgs e) { Timeout = true; }

        private struct BoardMovePair {
            public Board board;
            public Move move;
        }
    }
}
