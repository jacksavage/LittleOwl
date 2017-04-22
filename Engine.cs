namespace LittleOwl {
    using System;
    using System.Collections.Generic;

    public class Engine {
        // read a board state and select a suitable move to return
        public string SelectMove(string fen) {
            var Board = new Board(fen); // load the board

            // store the enemy's last move
            if (LastBoard != null) {
                var LastMove = BoardDiff(LastBoard, Board);
                if (PastMoves.Count == 8) PastMoves.Dequeue();
                PastMoves.Enqueue(LastMove);
            }

            Board.PastMoves = PastMoves; // assign engine's past moves to the board
            var Move = new Move(); // todo search for the best move

            return Move.ToString();
        }

        // apply a move to a given board
        private Board ApplyMove(Board board, Move move) {
            var Result = new Board();
            Result.ActiveColorWhite = board.ActiveColorWhite;

            /// update piece positions ///
            Result.Pieces = new PiecePositions(board.Pieces);
            Result.Pieces.All &= ~(move.From.Position | move.To.Position);
            PiecePositions.Player ActiveP = ActivePlayer(Result);
            switch (move.MoveType) {
                case PieceMoveType.Pawn:
                    ActiveP.Pawns |= move.To.Position;
                    break;
                case PieceMoveType.Knight:
                case PieceMoveType.PawnKnight:
                    ActiveP.Knights |= move.To.Position;
                    break;
                case PieceMoveType.Bishop:
                case PieceMoveType.PawnBishop:
                    ActiveP.Bishops |= move.To.Position;
                    break;
                case PieceMoveType.Rook:
                case PieceMoveType.PawnRook:
                    ActiveP.Rooks |= move.To.Position;
                    break;
                case PieceMoveType.Queen:
                case PieceMoveType.PawnQueen:
                    ActiveP.Queens |= move.To.Position;
                    break;
                case PieceMoveType.King:
                    ActiveP.King |= move.To.Position;
                    break;
                case PieceMoveType.KingQueenside:
                    ActiveP.King |= move.To.Position;
                    if (board.ActiveColorWhite) {
                        ActiveP.Rooks &= ~Masks.WhiteQueenSideRookInitPos;
                        ActiveP.Rooks |= Masks.WhiteQueenSideRookPostCastlePos;
                    } else {
                        ActiveP.Rooks &= ~Masks.BlackQueenSideRookInitPos;
                        ActiveP.Rooks |= Masks.BlackQueenSideRookPostCastlePos;
                    }
                    break;
                case PieceMoveType.KingKingside:
                    ActiveP.King |= move.To.Position;
                    if (board.ActiveColorWhite) {
                        ActiveP.Rooks &= ~Masks.WhiteKingSideRookInitPos;
                        ActiveP.Rooks |= Masks.WhiteQueenSideRookPostCastlePos;
                    } else {
                        ActiveP.Rooks &= ~Masks.BlackKingSideRookInitPos;
                        ActiveP.Rooks |= Masks.BlackKingSideRookPostCastlePos;
                    }
                    break;
                default:
                    throw new ArgumentException("invalid move type");
            }

            /// update the draw counter ///
            bool PawnMoved = move.MoveType < PieceMoveType.Knight;
            bool CaptureOccured = (move.To.Position & (InactivePlayer(board).All | board.EnPassantTarget.Position)) != 0;
            if (PawnMoved || CaptureOccured) Result.HalfMoveClock = 100; // reset
            else Result.HalfMoveClock = (byte)(board.HalfMoveClock - 1); // decrement

            /// check for en passant target ///
            if (PawnMoved && (move.From.Position & Masks.Ranks27) != 0) {
                if (board.ActiveColorWhite && ((move.From.Position << 16) & move.To.Position) != 0)
                    Result.EnPassantTarget = new BoardAddress(move.From.Position << 8);
                else if (!board.ActiveColorWhite && ((move.From.Position >> 16) & move.To.Position) != 0)
                    Result.EnPassantTarget = new BoardAddress(move.From.Position >> 8);
            }

            /// update castling availability ///
            // copy the availablity of the inactive player and store the past availablity for the active player
            Board.Castling.Move PastActiveCastlingStatus, NewActiveCastlingStatus;
            if (board.ActiveColorWhite) {
                Result.CastlingAvailability.Black = board.CastlingAvailability.Black;
                PastActiveCastlingStatus = board.CastlingAvailability.White;
            } else {
                Result.CastlingAvailability.White = board.CastlingAvailability.White;
                PastActiveCastlingStatus = board.CastlingAvailability.Black;
            }

            // was castling already disallowed or did the king just move?
            if (PastActiveCastlingStatus == Board.Castling.Move.Disallowed || (move.MoveType >= PieceMoveType.King)) { // yes
                NewActiveCastlingStatus = Board.Castling.Move.Disallowed;
            } else { // no
                switch (PastActiveCastlingStatus) {
                    case Board.Castling.Move.KingSide:
                        if (KingSideRookMoved(board)) NewActiveCastlingStatus = Board.Castling.Move.Disallowed;
                        else NewActiveCastlingStatus = Board.Castling.Move.KingSide;
                        break;
                    case Board.Castling.Move.QueenSide:
                        if (QueenSideRookMoved(board)) NewActiveCastlingStatus = Board.Castling.Move.Disallowed;
                        else NewActiveCastlingStatus = Board.Castling.Move.QueenSide;
                        break;
                    case Board.Castling.Move.BothSides:
                        if (QueenSideRookMoved(board)) NewActiveCastlingStatus = Board.Castling.Move.KingSide;
                        else if (KingSideRookMoved(board)) NewActiveCastlingStatus = Board.Castling.Move.KingSide;
                        else NewActiveCastlingStatus = Board.Castling.Move.BothSides;
                        break;
                    default:
                        throw new Exception(string.Format("invalid castling status \"{0}\"", PastActiveCastlingStatus));
                }

                if (board.ActiveColorWhite) Result.CastlingAvailability.White = NewActiveCastlingStatus;
                else Result.CastlingAvailability.Black = NewActiveCastlingStatus;
            }

            /// update full move counter ///
            if (!board.ActiveColorWhite) Result.FullMoveNumber++; // on black player's move

            /// switch the active player ///
            Result.ActiveColorWhite = !board.ActiveColorWhite;

            /// update past move tracker ///
            Result.PastMoves = new Queue<Move>(board.PastMoves); // copy references to the past moves
            if (Result.PastMoves.Count == 8) Result.PastMoves.Dequeue(); // limit at 8 moves
            Result.PastMoves.Enqueue(move); // add the current move

            return Result;
        }

        // does the active player have a rook in the init position of the queenside rook?
        private bool QueenSideRookMoved(Board board) {
            if (board.ActiveColorWhite) return (board.Pieces.White.Rooks & Masks.WhiteQueenSideRookInitPos) != 0;
            else return (board.Pieces.Black.Rooks & Masks.BlackQueenSideRookInitPos) != 0;
        }

        // does the active player have a rook in the init position of the kingside rook?
        private bool KingSideRookMoved(Board board) {
            if (board.ActiveColorWhite) return (board.Pieces.White.Rooks & Masks.WhiteKingSideRookInitPos) != 0;
            else return (board.Pieces.Black.Rooks & Masks.BlackKingSideRookInitPos) != 0;
        }

        // todo create a move by doing a diff of two boards
        private Move BoardDiff(Board before, Board after) {
            var Result = new Move();

            // get the starting position of the moving piece
            ulong Position = ActivePlayer(before).All & ~ActivePlayer(after).All;
            int NumPositions = Utilities.NumActiveBits(Position);
            if (NumPositions == 0) throw new ArgumentException("none of the active player's pieces moved");

            // could this be a castling move?
            if (NumPositions > 1) { // yes
                // get the king
                Position &= ActivePlayer(before).King;
                if (Position == 0) throw new ArgumentException("active player moved more than one piece on a non-castling move");
            }

            Result.From = new BoardAddress(Position);

            // get the ending position of the moving piece

            throw new NotImplementedException();
            return Result;
        }

        // check for simplified three move repetition
        private bool MoveRepetitionOccurred(Board board) {
            if (board.PastMoves.Count < 8) return false;

            Move[] Moves = PastMoves.ToArray();
            if (Moves[0] == Moves[4] &&
                Moves[1] == Moves[5] &&
                Moves[2] == Moves[6] &&
                Moves[3] == Moves[7]) return true;

            return false;
        }

        private PiecePositions.Player ActivePlayer(Board board) { if (board.ActiveColorWhite) return board.Pieces.White; else return board.Pieces.Black; }
        private PiecePositions.Player InactivePlayer(Board board) { if (board.ActiveColorWhite) return board.Pieces.Black; else return board.Pieces.White; }

        private Queue<Move> PastMoves = new Queue<Move>();
        private Board LastBoard;
    }
}
