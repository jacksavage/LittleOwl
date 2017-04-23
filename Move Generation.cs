namespace LittleOwl {
    using System;
    using System.Collections.Generic;

    internal static class MoveGen {
        // all of the spaces a given player can capture {
        public static ulong AttackMap(PiecePositions.Player player) { return MoveMap(player) & player.Opponent.All; }

        // all of the spaces a given player covers
        public static ulong MoveMap(PiecePositions.Player player) {
            ulong Result = 0;
            Result |= PawnMoveMap(player);
            Result |= KnightMoveMap(player);
            Result |= BishopMoveMap(player);
            Result |= RookMoveMap(player);
            Result |= QueenMoveMap(player);
            Result |= TheKingMoveMap(player);
            return Result;
        }

        // get the moves/boards one by one
        public static IEnumerable<MoveBoardPair> EachMove(Board board) {
            var Buffer = new List<Move>();
            Board NextBoard;

            // add all of the moves to the buffer
            Buffer.AddRange(PawnMoves(board));
            Buffer.AddRange(KnightMoves(board));
            Buffer.AddRange(BishopMoves(board));
            Buffer.AddRange(RookMoves(board));
            Buffer.AddRange(QueenMoves(board));
            Buffer.AddRange(KingMoves(board));

            // look at pawn moves
            foreach (Move move in Buffer) {
                // get next board state by applying this move to the current board
                NextBoard = Engine.ApplyMove(board, move);

                // if move doesn't put friendly king in check then yield it
                if (!NextBoard.InCheck) yield return new MoveBoardPair(NextBoard, move);
            }
        }

        // get all of the moves/boards at once
        public static MoveBoardPair[] AllMoves(Board board) {
            var Result = new Queue<MoveBoardPair>();
            foreach (MoveBoardPair mb in Moves(board))
                Result.Enqueue(mb);
            return Result.ToArray();
        }

        private static ulong TheKingMoveMap(PiecePositions.Player player) {
            throw new NotImplementedException();
        }

        private static ulong QueenMoveMap(PiecePositions.Player player) {
            throw new NotImplementedException();
        }

        private static ulong RookMoveMap(PiecePositions.Player player) {
            throw new NotImplementedException();
        }

        private static ulong BishopMoveMap(PiecePositions.Player player) {
            throw new NotImplementedException();
        }

        private static ulong KnightMoveMap(PiecePositions.Player player) {
            throw new NotImplementedException();
        }

        private static ulong PawnMoveMap(PiecePositions.Player player) {
            throw new NotImplementedException();
        }

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
