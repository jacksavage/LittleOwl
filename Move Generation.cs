namespace LittleOwl {
    using System;

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
    }
}
