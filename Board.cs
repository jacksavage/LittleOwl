using System.Text.RegularExpressions;
using System.Text;
using System;

namespace LittleOwl {
    // fully represents a state in a chess game
    public class Board {
        // create a board with a FEN string
        public Board(string fen) {
            Match Match = FenParser.Match(fen);
            if (!Match.Success) throw new ArgumentException("invalid FEN string");
            string[] Fields = {Match.Groups[1].Value, Match.Groups[2].Value, Match.Groups[3].Value, Match.Groups[4].Value, Match.Groups[5].Value, Match.Groups[6].Value};

        }

        // create a FEN string from the calling board
        public override string ToString() {
            var Result = new StringBuilder();

            return Result.ToString();
        }

        private const string FenPattern = @"(?i)^\s*([pnbrkq1-8\/]{17,})\s+([wb])\s+([kq\-]{4})\s+((?:[a-h][1-8])|\-)\s+(\d{1,2})\s+(\d+)\s*$";
        private static Regex FenParser = new Regex(FenPattern);

        internal PiecePositions Pieces;
        internal bool ActiveColorWhite;
        internal ulong EnPassantTarget;
        // todo internal CastlingAvailability;
        internal byte HalfMoveClock;
        internal int FullMoveNumber;
    }
}
