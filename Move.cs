using System;

namespace LittleOwl {
    // chess move representation
    internal struct Move {
        public BoardAddress From;
        public BoardAddress To;
        public PieceMoveType MoveType;

        public Move(BoardAddress f, BoardAddress t, PieceMoveType m) {
            From = f;
            To = t;
            MoveType = m;
        }

        // create a move from ACN string and the piece positions
        public Move(string acn, PiecePositions pieces) {
            int Len = acn.Length;
            if (Len < 4 || Len > 5) throw new ArgumentException(string.Format("ACN strings must be 4 or 5 characters long: \"{0}\"", acn));
            string f = acn.Substring(0, 2);
            string t = acn.Substring(2, 2);

            From = new BoardAddress(f);
            To = new BoardAddress(t);

            // determine the type of piece that is moving
            if ((pieces.Pawns & From.Position) != 0) MoveType = PieceMoveType.Pawn;
            else if ((pieces.Knights & From.Position) != 0) MoveType = PieceMoveType.Knight;
            else if ((pieces.Bishops & From.Position) != 0) MoveType = PieceMoveType.Bishop;
            else if ((pieces.Rooks & From.Position) != 0) MoveType = PieceMoveType.Rook;
            else if ((pieces.Queens & From.Position) != 0) MoveType = PieceMoveType.Queen;
            else if ((pieces.Kings & From.Position) != 0) MoveType = PieceMoveType.King;
            else throw new ArgumentException(string.Format("move \"{0}\" invalid given current piece positions", acn));

            // promotion on this move?
            if (Len == 5) { // yes
                if (MoveType != PieceMoveType.Pawn) throw new ArgumentException("tried to promote from a piece type other than pawn");
                char promo = char.ToLower(acn[4]);
                switch (promo) {
                    case 'n':
                        MoveType = PieceMoveType.PawnKnight;
                        break;
                    case 'b':
                        MoveType = PieceMoveType.PawnBishop;
                        break;
                    case 'r':
                        MoveType = PieceMoveType.PawnRook;
                        break;
                    case 'q':
                        MoveType = PieceMoveType.PawnQueen;
                        break;
                    default:
                        throw new ArgumentException(string.Format("ACN string contains invalid promotion character \"{0}\"", promo));
                }
            }
        }

        // create an ACN string from the calling move
        public override string ToString() {
            switch (MoveType) {
                case PieceMoveType.PawnKnight:
                    return string.Format("{0}{1}{2}", From, To, 'n');
                case PieceMoveType.PawnBishop:
                    return string.Format("{0}{1}{2}", From, To, 'b');
                case PieceMoveType.PawnRook:
                    return string.Format("{0}{1}{2}", From, To, 'r');
                case PieceMoveType.PawnQueen:
                    return string.Format("{0}{1}{2}", From, To, 'q');
                case PieceMoveType.Knight:
                    return string.Format("{0}{1}{2}", 'n', From, To);
                case PieceMoveType.Bishop:
                    return string.Format("{0}{1}{2}", 'b', From, To);
                case PieceMoveType.Rook:
                    return string.Format("{0}{1}{2}", 'r', From, To);
                case PieceMoveType.Queen:
                    return string.Format("{0}{1}{2}", 'q', From, To);
                case PieceMoveType.King:
                    return string.Format("{0}{1}{2}", 'k', From, To);
                default:
                    return string.Format("{0}{1}", From, To);
            }
        }

        public static bool operator ==(Move left, Move right) {
            if (((object)left) == null && ((object)right) == null) return true;
            if (((object)left) == null || ((object)right) == null) return false;
            if ((left.From != right.From) || (left.To != right.To)) return false;
            if (left.MoveType != right.MoveType) return false;
            return true;
        }

        public static bool operator !=(Move left, Move right) { return !(left == right); }
    }

    internal enum PieceMoveType : byte {
        Pawn = 0,
        PawnKnight = 1,
        PawnBishop = 2,
        PawnRook = 3,
        PawnQueen = 4,
        Knight = 5,
        Bishop = 6,
        Rook = 7,
        Queen = 8,
        King = 9
    }
}
