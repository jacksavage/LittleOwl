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

        // create a move from ACN (algebraic chess notation) string and the Piece positioning bitboards
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
                    return string.Format("{0}{1}{2}", From, To, 'k');
                case PieceMoveType.PawnQueen:
                    return string.Format("{0}{1}{2}", From, To, 'q');
                default:
                    return string.Format("{0}{1}", From, To);
            }
        }
    }

    internal enum PieceMoveType : int {
        Pawn = 0,
        Knight = 1,
        Bishop = 2,
        Rook = 3,
        Queen = 4,
        King = 5,
        PawnKnight = 6,
        PawnBishop = 7,
        PawnRook = 8,
        PawnQueen = 9
    }
}
