﻿namespace LittleOwl {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

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

        // create a move from a diff of two boards
        private Move BoardDiff(Board before, Board after) {
            // cache player handles
            PiecePositions.Player ActivePlayerBefore = ActivePlayer(before);
            PiecePositions.Player InactivePlayerAfter = InactivePlayer(after);

            var Result = new Move();

            /// get the starting positions of the moving pieces ///
            ulong MultiPositions = ActivePlayerBefore.All & ~InactivePlayerAfter.All;
            if (MultiPositions == 0) { Debug.WriteLine("none of the active player's pieces moved"); return null; }
            bool TwoPieceFlag = false;
            ulong RookStartPosition = 0;
            ulong RookEndPosition = 0;
            ulong[] Positions = new List<ulong>(Utilities.BitSplit(MultiPositions)).ToArray();
            PieceMoveType FromPiece, ToPiece;

            if (Positions.Length > 2) { // more than two moved pieces
                Debug.WriteLine("more than two pieces moved");
                return null;
            } else if (Positions.Length == 2) { // double piece move (castle)
                TwoPieceFlag = true;

                FromPiece = PieceTypeAtAddress(ActivePlayerBefore, new BoardAddress(Positions[0]));
                if (FromPiece != PieceMoveType.King) {
                    if (FromPiece != PieceMoveType.Rook) {
                        Debug.WriteLine("two pieces moved, but a rook was not one of them");
                        return null;
                    }

                    FromPiece = PieceTypeAtAddress(ActivePlayerBefore, new BoardAddress(Positions[1]));
                    if (FromPiece != PieceMoveType.King) { Debug.WriteLine("two pieces moved, but the king was not one of them"); return null; }

                    Result.From = new BoardAddress(Positions[1]);
                    RookStartPosition = Positions[0];
                } else {
                    if (PieceTypeAtAddress(ActivePlayerBefore, new BoardAddress(Positions[1])) != PieceMoveType.Rook) {
                        Debug.WriteLine("two pieces moved, but a rook was not one of them");
                        return null;
                    }

                    Result.From = new BoardAddress(Positions[0]);
                    RookStartPosition = Positions[1];
                }
            } else { // single piece moved
                Result.From = new BoardAddress(Positions[0]);
                FromPiece = PieceTypeAtAddress(ActivePlayerBefore, new BoardAddress(Positions[0]));
            }

            /// get the ending position of the moving piece ///
            MultiPositions = InactivePlayerAfter.All & ~ActivePlayerBefore.All;
            Positions = new List<ulong>(Utilities.BitSplit(MultiPositions)).ToArray();
            if (MultiPositions == 0) { Debug.WriteLine("the active player lost a piece on thier turn"); return null; }

            if (Positions.Length > 2) { // more than two pieces moved
                Debug.WriteLine("a piece was added to the board");
                return null;
            } else if (Positions.Length == 2) { // double piece move (castle)
                if (!TwoPieceFlag) { Debug.WriteLine("a piece was added to the board"); return null; }

                ToPiece = PieceTypeAtAddress(InactivePlayerAfter, new BoardAddress(Positions[0]));
                if (ToPiece != PieceMoveType.King) {
                    if (ToPiece != PieceMoveType.Rook) { Debug.WriteLine("two pieces moved, but a rook was not one of them"); return null; }
                    ToPiece = PieceTypeAtAddress(InactivePlayerAfter, new BoardAddress(Positions[1]));
                    if (ToPiece != PieceMoveType.King) { Debug.WriteLine("two pieces moved, but the king was not one of them"); return null; }

                    Result.To = new BoardAddress(Positions[1]);
                    RookEndPosition = Positions[0];
                } else {
                    if (PieceTypeAtAddress(InactivePlayerAfter, new BoardAddress(Positions[1])) != PieceMoveType.Rook) {
                        Debug.WriteLine("two pieces moved, but a rook was not one of them");
                        return null;
                    }

                    Result.To = new BoardAddress(Positions[0]);
                    RookEndPosition = Positions[1];
                }
            } else { // single piece moved
                if (TwoPieceFlag) { Debug.WriteLine("the active player lost a piece on thier turn"); return null; }
                Result.To = new BoardAddress(Positions[0]);
                ToPiece = PieceTypeAtAddress(InactivePlayerAfter, new BoardAddress(Positions[0]));
            }

            Result.MoveType = MoveTypeFromPieceTypes(FromPiece, ToPiece);
            if (TwoPieceFlag) { // update king move to a castling move
                if (Result.MoveType != PieceMoveType.King) { // sanity check
                    Debug.WriteLine("the move indicates a castle, but the king didn't move");
                    return null;
                }

                if (before.ActiveColorWhite) { // white player's move
                    if ((RookStartPosition ^ Masks.WhiteKingSideRookInitPos) == 0) { // king side castle
                        if ((RookEndPosition ^ Masks.WhiteKingSideRookPostCastlePos) != 0) { // does the rook's end position agree?
                            Debug.WriteLine("the move indicates a kingside castle, but the rook didn't move to its proper position");
                            return null;
                        }

                        Result.MoveType = PieceMoveType.KingKingside;
                    } else if ((RookStartPosition ^ Masks.WhiteQueenSideRookInitPos) == 0) { // queenside castle
                        if ((RookEndPosition ^ Masks.WhiteQueenSideRookPostCastlePos) != 0) { // does the rook's end position agree?
                            Debug.WriteLine("the move indicates a queenside castle, but the rook didn't move to its proper position");
                            return null;
                        }

                        Result.MoveType = PieceMoveType.KingQueenside;
                    } else { // invalid
                        Debug.WriteLine("the move indicates a castle, but the rook was not in its proper position");
                        return null;
                    }
                } else { // black player's move
                    if ((RookStartPosition ^ Masks.BlackKingSideRookInitPos) == 0) { // king side castle
                        if ((RookEndPosition ^ Masks.BlackKingSideRookPostCastlePos) != 0) { // does the rook's end position agree?
                            Debug.WriteLine("the move indicates a kingside castle, but the rook didn't move to its proper position");
                            return null;
                        }

                        Result.MoveType = PieceMoveType.KingKingside;
                    } else if ((RookStartPosition ^ Masks.BlackQueenSideRookInitPos) == 0) { // queenside castle
                        if ((RookEndPosition ^ Masks.BlackQueenSideRookPostCastlePos) != 0) { // does the rook's end position agree?
                            Debug.WriteLine("the move indicates a queenside castle, but the rook didn't move to its proper position");
                            return null;
                        }

                        Result.MoveType = PieceMoveType.KingQueenside;
                    } else { // invalid
                        Debug.WriteLine("the move indicates a castle, but the rook was not in its proper position");
                        return null;
                    }
                }
            }

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

        // get the type of piece at a given board address
        private PieceMoveType PieceTypeAtAddress(PiecePositions.Player player, BoardAddress address) {
            if ((address.Position & player.Pawns) != 0) return PieceMoveType.Pawn;
            else if ((address.Position & player.Knights) != 0) return PieceMoveType.Knight;
            else if ((address.Position & player.Bishops) != 0) return PieceMoveType.Bishop;
            else if ((address.Position & player.Rooks) != 0) return PieceMoveType.Rook;
            else if ((address.Position & player.Queens) != 0) return PieceMoveType.Queen;
            else if ((address.Position & player.King) != 0) return PieceMoveType.King;
            else { Debug.WriteLine("no piece found at specified position"); return PieceMoveType.Undefined; }
        }

        // get the move type using the before and after piece types
        private PieceMoveType MoveTypeFromPieceTypes(PieceMoveType before, PieceMoveType after) {
            // argument guard
            if (before < PieceMoveType.Pawn || before > PieceMoveType.King || after < PieceMoveType.Pawn || after > PieceMoveType.King)
                return PieceMoveType.Undefined;

            if (before == after) { // standard or castling move
                return before;
            } else if (before == PieceMoveType.Pawn) { // pawn promo
                switch (after) {
                    case PieceMoveType.Queen:
                        return PieceMoveType.PawnQueen;
                    case PieceMoveType.Rook:
                        return PieceMoveType.PawnRook;
                    case PieceMoveType.Bishop:
                        return PieceMoveType.PawnBishop;
                    case PieceMoveType.Knight:
                        return PieceMoveType.PawnKnight;
                    default:
                        return PieceMoveType.Undefined;
                }
            }

            return PieceMoveType.Undefined;
        }

        private Queue<Move> PastMoves = new Queue<Move>();
        private Board LastBoard;
    }
}
