namespace LittleOwl {
    using System;
    using System.Collections.Generic;

    internal static class MoveGen {
        public static IEnumerable<MoveBoardPair> EachMove(Board board) {
            var Buffer = new List<Move>();
            Board NextBoard;

            // add all of the active player's moves to the buffer
            Buffer.AddRange(PawnMoves(board, true));
            Buffer.AddRange(KnightMoves(board, true));
            Buffer.AddRange(BishopMoves(board, true));
            Buffer.AddRange(RookMoves(board, true));
            Buffer.AddRange(QueenMoves(board, true));
            Buffer.AddRange(KingMoves(board, true));

            // group the moves by type
            var Captures = new List<Move>();
            var Promotions = new List<Move>();
            var Standard = new List<Move>();
            foreach (Move move in Buffer) {
                if ((move.To.Position & board.InactivePlayer.All) != 0) Captures.Add(move);
                else if (move.MoveType < PieceMoveType.Pawn) Promotions.Add(move);
                else Standard.Add(move);
            }

            // look at captures
            foreach (Move move in Utilities.RandomComprehensiveAccess<Move>.Access(Captures)) {
                NextBoard = Engine.ApplyMove(board, move);

                // if current king not in check in next board, yield as a valid move
                if ((MoveMap(NextBoard, true) & NextBoard.InactivePlayer.King) == 0) yield return new MoveBoardPair(NextBoard, move);
            }

            // look at promos
            foreach (Move move in Utilities.RandomComprehensiveAccess<Move>.Access(Promotions)) {
                NextBoard = Engine.ApplyMove(board, move);

                // if current king not in check in next board, yield as a valid move
                if ((MoveMap(NextBoard, true) & NextBoard.InactivePlayer.King) == 0) yield return new MoveBoardPair(NextBoard, move);
            }

            // look at standard moves
            foreach (Move move in Utilities.RandomComprehensiveAccess<Move>.Access(Standard)) {
                NextBoard = Engine.ApplyMove(board, move);

                // if current king not in check in next board, yield as a valid move
                if ((MoveMap(NextBoard, true) & NextBoard.InactivePlayer.King) == 0) yield return new MoveBoardPair(NextBoard, move);
            }
        }
        public static MoveBoardPair[] AllMoves(Board board) {
            var Result = new Queue<MoveBoardPair>();
            foreach (MoveBoardPair mb in EachMove(board))
                Result.Enqueue(mb);
            return Result.ToArray();
        }

        public static ulong MoveMap(Board board, bool activePlayer) {
            ulong Result = 0;
            Result |= PawnMoveMap(board, activePlayer);
            Result |= KnightMoveMap(board, activePlayer);
            Result |= BishopMoveMap(board, activePlayer);
            Result |= RookMoveMap(board, activePlayer);
            Result |= QueenMoveMap(board, activePlayer);
            Result |= KingMoveMap(board, activePlayer);
            return Result;
        }
        public static ulong AttackMap(Board board, bool activePlayer) {
            return MoveMap(board, activePlayer) & board.InactivePlayer.All;
        }

        // get a map of all of a player's moves for a particular piece type
        private static ulong PawnMoveMap(Board board, bool activePlayer) {
            ulong Result = 0;
            ulong PawnLocations;
            if (activePlayer) PawnLocations = board.ActivePlayer.Pawns;
            else PawnLocations = board.InactivePlayer.Pawns;

            // combine each pawn's move map
            foreach (BoardAddress location in BoardAddresses(PawnLocations))
                Result |= APawnMoveMap(board, activePlayer, location);

            return Result;
        }
        private static ulong KnightMoveMap(Board board, bool activePlayer) {
            ulong Result = 0;
            ulong KnightLocations;
            if (activePlayer) KnightLocations = board.ActivePlayer.Knights;
            else KnightLocations = board.InactivePlayer.Knights;

            // combine each knight's move map
            foreach (BoardAddress location in BoardAddresses(KnightLocations))
                Result |= AKnightMoveMap(board, activePlayer, location);

            return Result;
        }
        private static ulong BishopMoveMap(Board board, bool activePlayer) {
            ulong Result = 0;
            ulong BishopLocations;
            if (activePlayer) BishopLocations = board.ActivePlayer.Bishops;
            else BishopLocations = board.InactivePlayer.Bishops;

            // combine each bishop's move map
            foreach (BoardAddress location in BoardAddresses(BishopLocations))
                Result |= ABishopMoveMap(board, activePlayer, location);

            return Result;
        }
        private static ulong RookMoveMap(Board board, bool activePlayer) {
            ulong Result = 0;
            ulong RookLocations;
            if (activePlayer) RookLocations = board.ActivePlayer.Rooks;
            else RookLocations = board.InactivePlayer.Rooks;

            // combine each rook's move map
            foreach (BoardAddress location in BoardAddresses(RookLocations))
                Result |= ARookMoveMap(board, activePlayer, location);

            return Result;
        }
        private static ulong QueenMoveMap(Board board, bool activePlayer) {
            ulong Result = 0;
            ulong QueenLocations;
            if (activePlayer) QueenLocations = board.ActivePlayer.Queens;
            else QueenLocations = board.InactivePlayer.Queens;

            // combine each queen's move map
            foreach (BoardAddress location in BoardAddresses(QueenLocations))
                Result |= AQueenMoveMap(board, activePlayer, location);

            return Result;
        }
        private static ulong KingMoveMap(Board board, bool activePlayer) {
            PiecePositions.Player Player;
            if (activePlayer) Player = board.ActivePlayer;
            else Player = board.InactivePlayer;

            ulong Result = MoveMaps.King[new BoardAddress(Player.King).Index];

            // mask off locations with player's other pieces
            Result &= ~Player.All;

            // todo add castling moves

            return Result;
        }

        // get a map of all of a player's moves for a particular piece
        private static ulong APawnMoveMap(Board board, bool activePlayer, BoardAddress address) {
            // get moves and captures
            ulong Moves, Caps;
            if (board.ActiveColorWhite) {
                if (activePlayer) {
                    Moves = MoveMaps.WhitePawn[address.Index];
                    Caps = MoveMaps.WhitePawnCapture[address.Index];
                } else {
                    Moves = MoveMaps.BlackPawn[address.Index];
                    Caps = MoveMaps.BlackPawnCapture[address.Index];
                }
            } else { // black
                if (activePlayer) {
                    Moves = MoveMaps.BlackPawn[address.Index];
                    Caps = MoveMaps.BlackPawnCapture[address.Index];
                } else {
                    Moves = MoveMaps.WhitePawn[address.Index];
                    Caps = MoveMaps.WhitePawnCapture[address.Index];
                }
            }

            // is a double move allowed?
            if (Utilities.NumActiveBits(Moves) == 2) { // yes
                // is at least one move unallowed?
                if ((Moves & board.Pieces.All) != 0) { //yes
                    // get the single move
                    ulong SingleMove = Moves & ((address.Position << 8) | (address.Position >> 8));

                    // if single move allowed, then it is the only move
                    if ((SingleMove & board.Pieces.All) == 0)
                        Moves = SingleMove;
                    else // otherwise no moves are allowed
                        Moves = 0;
                } // else no, leave both moves
            } else { // no
                // can't move into a another piece
                Moves &= ~board.Pieces.All;
            }

            // can only move to en passant position or into an enemy position
            if (activePlayer)  Caps &= board.InactivePlayer.All | board.EnPassantTarget; // only active player can cap en passant
            else Caps &= board.ActivePlayer.All;

            return Moves | Caps;
        }
        private static ulong AKnightMoveMap(Board board, bool activePlayer, BoardAddress address) {
            if (activePlayer) return MoveMaps.Knight[address.Index] & ~board.ActivePlayer.All;
            else return MoveMaps.Knight[address.Index] & ~board.InactivePlayer.All;
        }
        private static ulong ABishopMoveMap(Board board, bool activePlayer, BoardAddress address) {
            // get the bishop's move bitboard
            ulong Result = MoveMaps.Bishop[address.Index];

            // get the file and rank of the bishop
            int BishopFile = address.File;
            int BishopRank = address.Rank;

            // get slash and backslash masks
            ulong SlashMask = Masks.Slash[address.SlashIndex];
            ulong BackslashMask = Masks.BackSlash[address.SlashIndex];

            // get friends and foes in same slash and backslash as bishop
            ulong FriendsInSlash, FriendsInBackslash, FoesInSlash, FoesInBackslash;
            if (activePlayer) {
                FriendsInSlash = Result & board.ActivePlayer.All & SlashMask;
                FriendsInBackslash = Result & board.ActivePlayer.All & BackslashMask;
                FoesInSlash = Result & board.InactivePlayer.All & SlashMask;
                FoesInBackslash = Result & board.InactivePlayer.All & BackslashMask;
            } else {
                FriendsInSlash = Result & board.InactivePlayer.All & SlashMask;
                FriendsInBackslash = Result & board.InactivePlayer.All & BackslashMask;
                FoesInSlash = Result & board.ActivePlayer.All & SlashMask;
                FoesInBackslash = Result & board.ActivePlayer.All & BackslashMask;
            }

            int TempFile, TempRank;

            // step through friends in the same slash
            foreach (BoardAddress friend in BoardAddresses(FriendsInSlash)) {
                TempFile = friend.File;
                if (TempFile > BishopFile) // clear moves at and to the northeast of this piece
                    Result &= ~Masks.NorthEast[friend.BackSlashIndex];
                else // clear moves at and to the southwest of this piece
                    Result &= ~Masks.SouthWest[friend.BackSlashIndex];
            }

            // step through foes in the same slash
            foreach (BoardAddress foe in BoardAddresses(FoesInSlash)) {
                TempFile = foe.File;
                if (TempFile > BishopFile) { // clear moves to the northeast of this piece
                    if (foe.BackSlashIndex != 9)
                        Result &= ~Masks.NorthEast[(foe.BackSlashIndex - 1) & 0xF];
                } else { // clear moves to the southwest of this piece
                    if (foe.BackSlashIndex != 7)
                        Result &= ~Masks.SouthWest[(foe.BackSlashIndex + 1) & 0xF];
                }
            }

            // step through friends in the same backslash
            foreach (BoardAddress friend in BoardAddresses(FriendsInBackslash)) {
                TempRank = friend.Rank;
                if (TempRank > BishopRank) // clear moves at and to the northwest of this piece
                    Result &= ~Masks.NorthWest[friend.SlashIndex];
                else // clear moves at and to the southeast of this piece
                    Result &= ~Masks.SouthEast[friend.SlashIndex];
            }

            // step through foes in the same backslash
            foreach (BoardAddress foeInBackslash in BoardAddresses(FoesInBackslash)) {
                TempRank = foeInBackslash.Rank;
                if (TempRank > BishopRank) { // clear moves to the northwest of this piece
                    if (foeInBackslash.SlashIndex != 7)
                        Result &= ~Masks.NorthWest[(foeInBackslash.SlashIndex + 1) & 0xF];
                } else { // clear moves to the southeast of this piece
                    if (foeInBackslash.SlashIndex != 9)
                        Result &= ~Masks.SouthEast[(foeInBackslash.SlashIndex - 1) & 0xF];
                }
            }

            return Result;
        }
        private static ulong ARookMoveMap(Board board, bool activePlayer, BoardAddress address) {
            // get the rook's move bitboard
            ulong Result = MoveMaps.Rook[address.Index];

            // get the file and rank of the rook
            int RookFile = address.File;
            int RookRank = address.Rank;

            // get file and rank masks
            ulong FileMask = Masks.Files[RookFile];
            ulong RankMask = Masks.Ranks[RookRank];

            // get friends and foes in the same rank and file as the rook
            ulong FriendsInFile, FriendsInRank, FoesInFile, FoesInRank;
            if (activePlayer) {
                FriendsInFile = Result & board.ActivePlayer.All & FileMask;
                FriendsInRank = Result & board.ActivePlayer.All & RankMask;
                FoesInFile = Result & board.InactivePlayer.All & FileMask;
                FoesInRank = Result & board.InactivePlayer.All & RankMask;
            } else {
                FriendsInFile = Result & board.InactivePlayer.All & FileMask;
                FriendsInRank = Result & board.InactivePlayer.All & RankMask;
                FoesInFile = Result & board.ActivePlayer.All & FileMask;
                FoesInRank = Result & board.ActivePlayer.All & RankMask;
            }

            int TempFile, TempRank;

            // step through friends in the same file
            foreach (BoardAddress friend in BoardAddresses(FriendsInFile)) {
                TempRank = friend.Rank;
                if (TempRank > RookRank) // clear moves at and above this piece
                    Result &= ~Masks.North[TempRank];
                else // clear moves at and below this piece
                    Result &= ~Masks.South[TempRank];
            }

            // step through foes in the same file
            foreach (BoardAddress foe in BoardAddresses(FoesInFile)) {
                TempRank = foe.Rank;
                if (TempRank > RookRank) { // clear moves above this piece
                    if (TempRank != 7)
                        Result &= ~Masks.North[TempRank + 1];
                } else { // clear moves below this piece
                    if (TempRank != 0)
                        Result &= ~Masks.South[TempRank - 1];
                }
            }

            // step through friends in the same rank
            foreach (BoardAddress friend in BoardAddresses(FriendsInRank)) {
                TempFile = friend.File;
                if (TempFile > RookFile) // clear moves at and to the right of this piece
                    Result &= ~Masks.East[TempFile];
                else // clear moves at and to the left of this piece
                    Result &= ~Masks.West[TempFile];
            }

            // step through foes in the same rank
            foreach (BoardAddress foe in BoardAddresses(FoesInRank)) {
                TempFile = foe.File;
                if (TempFile > RookFile) { // clear moves to the right of this piece
                    if (TempFile != 7)
                        Result &= ~Masks.East[TempFile + 1];
                } else { // clear moves to the left of this piece
                    if (TempFile != 0)
                        Result &= ~Masks.West[TempFile - 1];
                }
            }

            return Result;
        }
        private static ulong AQueenMoveMap(Board board, bool activePlayer, BoardAddress address) {
            return ARookMoveMap(board, activePlayer, address) | ABishopMoveMap(board, activePlayer, address);
        }

        // get all of a player's moves for a particular piece type
        private static Move[] PawnMoves(Board board, bool activePlayer) {
            var Buffer = new Stack<Move>();
            ulong MM;
            int Rank;
            ulong PawnLocations;
            if (activePlayer) PawnLocations = board.ActivePlayer.Pawns;
            else PawnLocations = board.InactivePlayer.Pawns;

            // step through friendly pawn locations
            foreach (BoardAddress pawnLocation in BoardAddresses(PawnLocations)) {
                // get the move map for that pawn
                MM = APawnMoveMap(board, activePlayer, pawnLocation);

                // step through each destination
                foreach (BoardAddress destination in BoardAddresses(MM)) {
                    // pawn moving into final rank?
                    Rank = destination.Rank;
                    if (Rank == 7 || Rank == 0) { // yes
                        // add promo moves
                        Buffer.Push(new Move(pawnLocation, destination, PieceMoveType.PawnQueen));
                        Buffer.Push(new Move(pawnLocation, destination, PieceMoveType.PawnRook));
                        Buffer.Push(new Move(pawnLocation, destination, PieceMoveType.PawnBishop));
                        Buffer.Push(new Move(pawnLocation, destination, PieceMoveType.PawnKnight));
                    } else { // no
                        // add standard move
                        Buffer.Push(new Move(pawnLocation, destination, PieceMoveType.Pawn));
                    }
                }
            }

            return Buffer.ToArray();
        }
        private static Move[] KnightMoves(Board board, bool activePlayer) {
            var Buffer = new Stack<Move>();
            ulong MM;
            ulong KnightLocations;
            if (activePlayer) KnightLocations = board.ActivePlayer.Knights;
            else KnightLocations = board.InactivePlayer.Knights;

            // step through friendly knight locations
            foreach (BoardAddress knightLocation in BoardAddresses(KnightLocations)) {
                // get this knight's move map
                MM = AKnightMoveMap(board, activePlayer, knightLocation);

                // step through each destination and add move
                foreach (BoardAddress destination in BoardAddresses(MM))
                    Buffer.Push(new Move(knightLocation, destination, PieceMoveType.Knight));
            }

            return Buffer.ToArray();
        }
        private static Move[] BishopMoves(Board board, bool activePlayer) {
            var Buffer = new Stack<Move>();
            ulong MM;
            ulong BishopLocations;
            if (activePlayer) BishopLocations = board.ActivePlayer.Bishops;
            else BishopLocations = board.InactivePlayer.Bishops;

            // step through friendly bishop locations
            foreach (BoardAddress bishopLocation in BoardAddresses(BishopLocations)) {
                // get this bishop's move map
                MM = ABishopMoveMap(board, activePlayer, bishopLocation);

                // step through each destination and add move
                foreach (BoardAddress destination in BoardAddresses(MM))
                    Buffer.Push(new Move(bishopLocation, destination, PieceMoveType.Bishop));
            }

            return Buffer.ToArray();
        }
        private static Move[] RookMoves(Board board, bool activePlayer) {
            var Buffer = new Stack<Move>();
            ulong MM;
            ulong RookLocations = board.ActivePlayer.Rooks;

            // step through friendly rook locations
            foreach (BoardAddress rookLocation in BoardAddresses(RookLocations)) {
                // get this rook's move map
                MM = ARookMoveMap(board, activePlayer, rookLocation);

                // step through each destination and add move
                foreach (BoardAddress destination in BoardAddresses(MM))
                    Buffer.Push(new Move(rookLocation, destination, PieceMoveType.Rook));
            }

            return Buffer.ToArray();
        }
        private static Move[] QueenMoves(Board board, bool activePlayer) {
            var Buffer = new Stack<Move>();
            ulong MM;
            ulong QueenLocations;
            if (activePlayer) QueenLocations = board.ActivePlayer.Queens;
            else QueenLocations = board.InactivePlayer.Queens;

            // step through friendly queen locations
            foreach (BoardAddress queenLocation in BoardAddresses(QueenLocations)) {
                // get this queen's move map
                MM = AQueenMoveMap(board, activePlayer, queenLocation);

                // step through each destination and add move
                foreach (BoardAddress destination in BoardAddresses(MM))
                    Buffer.Push(new Move(queenLocation, destination, PieceMoveType.Queen));
            }

            return Buffer.ToArray();
        }
        private static Move[] KingMoves(Board board, bool activePlayer) {
            var Buffer = new Stack<Move>();

            // get the king's move map
            ulong MM = KingMoveMap(board, activePlayer);

            // step through each destination and add move
            foreach (ulong destination in Utilities.BitSplit(MM)) {
                if (activePlayer) Buffer.Push(new Move(board.ActivePlayer.King, destination, PieceMoveType.King));
                else Buffer.Push(new Move(board.InactivePlayer.King, destination, PieceMoveType.King));
            }

            return Buffer.ToArray();
        }

        private static IEnumerable<BoardAddress> BoardAddresses(ulong source) {
            foreach (ulong bit in Utilities.BitSplit(source))
                yield return new BoardAddress(bit);
        }

        // container for the move and the resulting board since the Board is already being generated
        // to check that the active player is not moving into check (removes need for regerenation by
        // the Searcher
        public struct MoveBoardPair {
            public MoveBoardPair(Board board, Move move) {
                this.Board = board;
                this.Move = move;
            }
            public Board Board;
            public Move Move;
        }
    }
}
