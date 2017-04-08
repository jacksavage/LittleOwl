using System.Text.RegularExpressions;
using System.Text;
using System;

namespace LittleOwl {
    // fully represents a state in a chess game
    public class Board {
        // create a board with a FEN string
        internal Board(string fen) {
            Match Match = FenParser.Match(fen);
            if (!Match.Success) throw new ArgumentException(string.Format("invalid FEN string \"{0}\"", fen));
            string[] Fields = {Match.Groups[1].Value, Match.Groups[2].Value, Match.Groups[3].Value, Match.Groups[4].Value, Match.Groups[5].Value, Match.Groups[6].Value};

        }

        // create a FEN string from the calling board
        public override string ToString() {
            var Result = new StringBuilder();
            ulong Position = ulong.MaxValue;
            byte EmptyCount;

            // cache relevant piece position bitboards
            ulong All = Pieces.All;
            ulong WhitePawns = Pieces.White.Pawns;
            ulong WhiteKnights = Pieces.White.Knights;
            ulong WhiteBishops = Pieces.White.Bishops;
            ulong WhiteRooks = Pieces.White.Rooks;
            ulong WhiteQueens = Pieces.White.Queens;
            ulong WhiteKing = Pieces.White.King;
            ulong BlackPawns = Pieces.Black.Pawns;
            ulong BlackKnights = Pieces.Black.Knights;
            ulong BlackBishops = Pieces.Black.Bishops;
            ulong BlackRooks = Pieces.Black.Rooks;
            ulong BlackQueens = Pieces.Black.Queens;
            ulong BlackKing = Pieces.Black.King;

            // step through ranks
            for (int rank = 0; rank < 8; rank++) {
                // reset empty space counter
                EmptyCount = 0;

                // step through files
                for (int file = 0; file < 8; file++) {
                    // is there a piece at the current position?
                    if ((Position & All) != 0) { // yes
                        // were there empty spaces preceeding this position?
                        if (EmptyCount != 0) { // yes
                            Result.Append(EmptyCount);
                            EmptyCount = 0;
                        }

                        // which type of piece was at this position?
                        if ((Position & WhitePawns) != 0) Result.Append("P"); // white pawn
                        else if ((Position & BlackPawns) != 0) Result.Append("p"); // black pawn
                        else if ((Position & WhiteKnights) != 0) Result.Append("N"); // white knight
                        else if ((Position & BlackKnights) != 0) Result.Append("n"); // black knight
                        else if ((Position & WhiteBishops) != 0) Result.Append("B"); // white bishop
                        else if ((Position & BlackBishops) != 0) Result.Append("b"); // black bishop
                        else if ((Position & WhiteRooks) != 0) Result.Append("R"); // white rook
                        else if ((Position & BlackRooks) != 0) Result.Append("r"); // black rook
                        else if ((Position & WhiteQueens) != 0) Result.Append("Q"); // white queen
                        else if ((Position & BlackQueens) != 0) Result.Append("q"); // black queen
                        else if ((Position & WhiteKing) != 0) Result.Append("K"); // white king
                        else if ((Position & BlackKing) != 0) Result.Append("k"); // black king
                        else throw new Exception("inconsistent piece positioning info"); // undefined
                    } else { // no
                        EmptyCount++;
                    }

                    // get next position
                    Position /= 2; 
                }

                // add proper delimiter
                if (rank != 7) Result.Append("/");
                else Result.Append(" ");
            }

            // who is at move?
            if (ActiveColorWhite) // white
                Result.Append("w ");
            else // black
                Result.Append("b ");

            // who can castle?
            if (CastlingAvailability.White == Castling.Move.Disallowed && CastlingAvailability.Black == Castling.Move.Disallowed) { // nobody!
                Result.Append("- ");
            } else { // someone can
                   // can white?
                switch (CastlingAvailability.White) {
                    case Castling.Move.BothSides:
                        Result.Append("KQ");
                        break;
                    case Castling.Move.QueenSide:
                        Result.Append("Q");
                        break;
                    case Castling.Move.KingSide:
                        Result.Append("K");
                        break;
                }

                // can black?
                switch (CastlingAvailability.Black) {
                    case Castling.Move.BothSides:
                        Result.Append("kq");
                        break;
                    case Castling.Move.QueenSide:
                        Result.Append("q");
                        break;
                    case Castling.Move.KingSide:
                        Result.Append("k");
                        break;
                }
            }

            // is there an enpassant move possible?
            if (EnPassantTarget == null) // no
                Result.Append("- ");
            else // yes
                Result.Append(EnPassantTarget);

            // add the halfmove clock
            Result.AppendFormat("{0} ", HalfMoveClock);

            // add the move number
            Result.Append(FullMoveNumber);

            // and wallah, here is a FEN string
            return Result.ToString();
        }

        private const string FenPattern = @"(?i)^\s*([pnbrkq1-8\/]{17,})\s+([wb])\s+([kq\-]{4})\s+((?:[a-h][1-8])|\-)\s+(\d{1,2})\s+(\d+)\s*$";
        private static Regex FenParser = new Regex(FenPattern);

        internal PiecePositions Pieces;
        internal bool ActiveColorWhite;
        internal BoardAddress EnPassantTarget;
        internal Castling CastlingAvailability;
        internal byte HalfMoveClock;
        internal int FullMoveNumber;

        internal struct Castling {
            public Move White, Black;
            public enum Move : byte { Disallowed, QueenSide, KingSide, BothSides }
        }
    }
}
