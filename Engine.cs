using System;

namespace LittleOwl {
    public class Engine {
        // read a board state and select a suitable move to return
        public string SelectMove(string fen) {
            var Board = new Board(fen);
            var Move = new Move();

            // todo search for the best move

            return Move.ToString();
        }

        // apply a move to a given board
        private Board ApplyMove(Board board, Move move) {
            var Result = new Board();
            Result.Pieces = new PiecePositions(board.Pieces);
            PiecePositions.Player FriendsAfter, EnemiesBefore;
            if (board.ActiveColorWhite) {
                EnemiesBefore = board.Pieces.Black;
                FriendsAfter = Result.Pieces.White;
            } else {
                EnemiesBefore = board.Pieces.White;
                FriendsAfter = Result.Pieces.Black;
            }

            // update piece positions
            Result.Pieces.All &= ~(move.From.Position | move.To.Position);
            switch (move.MoveType) {
                case PieceMoveType.Pawn:
                    FriendsAfter.Pawns |= move.To.Position;
                    break;
                case PieceMoveType.Knight:
                case PieceMoveType.PawnKnight:
                    FriendsAfter.Knights |= move.To.Position;
                    break;
                case PieceMoveType.Bishop:
                case PieceMoveType.PawnBishop:
                    FriendsAfter.Bishops |= move.To.Position;
                    break;
                case PieceMoveType.Rook:
                case PieceMoveType.PawnRook:
                    FriendsAfter.Rooks |= move.To.Position;
                    break;
                case PieceMoveType.Queen:
                case PieceMoveType.PawnQueen:
                    FriendsAfter.Queens |= move.To.Position;
                    break;
                case PieceMoveType.King:
                    FriendsAfter.King |= move.To.Position;
                    break;
                default:
                    throw new ArgumentException("invalid move type");
            }

            // update the draw counter
            bool PawnMoved = move.MoveType < PieceMoveType.Knight;
            bool CaptureOccured = (move.To.Position & (EnemiesBefore.All | board.EnPassantTarget.Position)) != 0;
            if (PawnMoved || CaptureOccured) Result.HalfMoveClock = 100; // reset
            else Result.HalfMoveClock = (byte)(board.HalfMoveClock - 1); // decrement

            // check for en passant target
            if (PawnMoved && (move.From.Position & Masks.Rank27) != 0) {
                if (board.ActiveColorWhite && ((move.From.Position + 16) & move.To.Position) != 0)
                    Result.EnPassantTarget = new BoardAddress(move.From.Position + 8);
                else if (!board.ActiveColorWhite && ((move.From.Position - 16) & move.To.Position) != 0)
                    Result.EnPassantTarget = new BoardAddress(move.From.Position - 8);
            }

            // todo update castling availability
            
            // update full move counter
            if (!board.ActiveColorWhite) Result.FullMoveNumber++; // update on black player's move

            // switch the active player
            Result.ActiveColorWhite = !board.ActiveColorWhite;

            return Result;
        }

        // create a move by doing a diff of two boards
        private Move BoardDiff(Board before, Board after) {
            var Result = new Move();

            // setup handles for the active player
            PiecePositions.Player ActivePiecesBefore, ActivePiecesAfter;
            if (before.ActiveColorWhite) {
                ActivePiecesBefore = before.Pieces.White;
                ActivePiecesAfter = after.Pieces.White;
            } else {
                ActivePiecesBefore = before.Pieces.Black;
                ActivePiecesAfter = after.Pieces.Black;
            }

            // get the starting position of the moving piece
            ulong Position = ActivePiecesBefore.All & ~ActivePiecesAfter.All;
            int NumPositions = Utilities.NumActiveBits(Position);
            if (NumPositions == 0) throw new ArgumentException("none of the active player's pieces moved");

            // could this be a castling move?
            if (NumPositions > 1) { // yes
                // get the king
                Position &= ActivePiecesBefore.King;
                if (Position == 0) throw new ArgumentException("active player moved more than one piece on a non-castling move");
            }

            Result.From = new BoardAddress(Position);

            // get the ending position of the moving piece

            throw new NotImplementedException();
            return Result;
        }
    }
}
