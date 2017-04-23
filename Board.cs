namespace LittleOwl {
    using System.Text.RegularExpressions;
    using System.Text;
    using System;
    using System.Collections.Generic;

    // fully represents a state in a chess game
    internal class Board {
        // create a board with a FEN string
        public Board(string fen) {
            Match Match = FenParser.Match(fen);
            if (!Match.Success) throw new ArgumentException(string.Format("invalid FEN string \"{0}\"", fen));
            
            // piece placement
            string Field = Match.Groups[1].Value.ToLower();
            string[] RankPlacement = Field.Split('/');
            BoardAddress Location;
            int File;
            for (int rank = 0; rank < 8; rank++) {
                File = 0;
                foreach (char c in RankPlacement[rank]) {
                    Location = new BoardAddress((char)('a' + File), rank);

                    switch (c) {
                        case 'p':
                            Pieces.Black.Pawns |= Location.Position;
                            break;
                        case 'P':
                            Pieces.White.Pawns |= Location.Position;
                            break;
                        case 'n':
                            Pieces.Black.Knights |= Location.Position;
                            break;
                        case 'N':
                            Pieces.White.Knights |= Location.Position;
                            break;
                        case 'b':
                            Pieces.Black.Bishops |= Location.Position;
                            break;
                        case 'B':
                            Pieces.White.Bishops |= Location.Position;
                            break;
                        case 'r':
                            Pieces.Black.Rooks |= Location.Position;
                            break;
                        case 'R':
                            Pieces.White.Rooks |= Location.Position;
                            break;
                        case 'q':
                            Pieces.Black.Queens |= Location.Position;
                            break;
                        case 'Q':
                            Pieces.White.Queens |= Location.Position;
                            break;
                        case 'k':
                            Pieces.Black.King |= Location.Position;
                            break;
                        case 'K':
                            Pieces.White.King |= Location.Position;
                            break;
                        default: // num empty spaces
                            if (!char.IsDigit(c)) throw new ArgumentException(string.Format("invalid character \"{0}\" in piece placement field", c));
                            File += c - '0'; // convert char to int
                            break;
                    }

                    File++;
                }
            }

            // active color
            Field = Match.Groups[2].Value.ToLower();
            if (Field == "w") ActiveColorWhite = true;
            else ActiveColorWhite = false;

            // castling availablity
            Field = Match.Groups[3].Value;
            if (Field == "-") { // not available
                CastlingAvailability.White = Castling.Move.Disallowed;
                CastlingAvailability.Black = Castling.Move.Disallowed;
            } else { // available to at least one player
                // check white
                if (Field.Contains("K") && Field.Contains("Q")) CastlingAvailability.White = Castling.Move.BothSides;
                else if (Field.Contains("K")) CastlingAvailability.White = Castling.Move.KingSide;
                else if (Field.Contains("Q")) CastlingAvailability.White = Castling.Move.QueenSide;
                else CastlingAvailability.White = Castling.Move.Disallowed;

                // check black
                if (Field.Contains("k") && Field.Contains("q")) CastlingAvailability.Black = Castling.Move.BothSides;
                else if (Field.Contains("k")) CastlingAvailability.Black = Castling.Move.KingSide;
                else if (Field.Contains("q")) CastlingAvailability.Black = Castling.Move.QueenSide;
                else CastlingAvailability.Black = Castling.Move.Disallowed;
            }

            // enpassant location
            Field = Match.Groups[4].Value;
            if (Field == "-") EnPassantTarget = null;
            else EnPassantTarget = new BoardAddress(Field);

            // halfmove clock
            Field = Match.Groups[5].Value;
            if (!byte.TryParse(Field, out HalfMoveClock)) throw new ArgumentException(string.Format("could not parse halfmove clock \"{0}\"", Field));

            // halfmove clock
            Field = Match.Groups[6].Value;
            if (!int.TryParse(Field, out FullMoveNumber)) throw new ArgumentException(string.Format("could not parse full move number \"{0}\"", Field));
        }

        // explicit empty constructor
        public Board() {}

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

        private const string FenPattern = @"(?i)^\s*([pnbrkq1-8\/]{17,})\s+([wb])\s+([kq\-]{1,4})\s+((?:[a-h][1-8])|\-)\s+(\d{1,2})\s+(\d+)\s*$";
        private static Regex FenParser = new Regex(FenPattern);

        public PiecePositions Pieces;
        public bool ActiveColorWhite;
        public BoardAddress EnPassantTarget;
        public Castling CastlingAvailability;
        public byte HalfMoveClock;
        public int FullMoveNumber;

        public Queue<Move> PastMoves; // stores past eight moves
        public PiecePositions.Player ActivePlayer { get { if (ActiveColorWhite) return Pieces.White; else return Pieces.Black; } }
        public PiecePositions.Player InactivePlayer { get { return ActivePlayer.Opponent; } }

        public class Castling {
            public Move White, Black;
            public enum Move : byte { Disallowed, QueenSide, KingSide, BothSides }

            // deep copy constructor
            public Castling(Castling source) {
                White = source.White;
                Black = source.Black;
            }
        }
    }
}
