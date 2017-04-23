namespace LittleOwl {
    internal static class MoveMaps {
        public static readonly ulong[] BlackPawn;
        public static readonly ulong[] WhitePawn;
        public static readonly ulong[] BlackPawnCapture;
        public static readonly ulong[] WhitePawnCapture;
        public static readonly ulong[] Knight;
        public static readonly ulong[] Bishop;
        public static readonly ulong[] Rook;
        public static readonly ulong[] Queen;
        public static readonly ulong[] King;

        static MoveMaps() {
            BlackPawn = PawnMoves(false);
            WhitePawn = PawnMoves(true);
            BlackPawnCapture = PawnCaptures(false);
            WhitePawnCapture = PawnCaptures(true);
            Knight = KnightMoves();
            Bishop = BishopMoves();
            Rook = RookMoves();
            Queen = QueenMoves(Bishop, Rook);
            King = KingMoves();
        }

        private static ulong[] PawnMoves(bool white) {
            var Result = new ulong[64];
            ulong Position = 1;
            var Address = new BoardAddress(0);
            for (int i = 0; i < 64; i++) {
                Address.Index = i;

                if (white) { // white
                    if (Address.Rank > 0) {
                        Result[i] = Position << 8; // single move
                        if (Address.Rank == 1) Result[i] |= Position << 16; // double move
                    } else {
                        Result[i] = 0; // not applicable
                    }
                } else { // black
                    if (Address.Rank < 7) {
                        Result[i] = Position >> 8; // single move
                        if (Address.Rank == 6) Result[i] |= Position >> 16; // double move
                    } else {
                        Result[i] = 0; // not applicable
                    }
                }

                Position <<= 1;
            }

            return Result;
        }

        private static ulong[] PawnCaptures(bool white) {
            var Result = new ulong[64];
            ulong Position = 1;
            ulong Moves = 0;
            var Address = new BoardAddress(0);
            for (int i = 0; i < 64; i++) {
                Address.Index = i;

                if (white) { // white
                    if (Address.Rank > 0) {
                        Moves = (Position << 7) & ~Masks.FileH;
                        Moves |= (Position << 9) & ~Masks.FileA;
                        Result[i] = Moves;
                    } else {
                        Result[i] = 0; // not applicable
                    }
                } else { // black
                    if (Address.Rank < 7) {
                        Moves = (Position >> 7) & ~Masks.FileA;
                        Moves |= (Position >> 9) & ~Masks.FileH;
                        Result[i] = Moves;
                    } else {
                        Result[i] = 0; // not applicable
                    }
                }

                Position <<= 1;
            }
            return Result;
        }

        private static ulong[] KnightMoves() {
            var Result = new ulong[64];
            ulong Position = 1;
            ulong East, West;
            for (int i = 0; i < 64; i++) {
                // reset move board
                East = 0;
                West = 0;

                // build the move board
                East |= Position << 10; // NEE
                East |= Position >> 6; // SEE
                East &= ~Masks.FileB; // mask 'b'
                East |= Position << 17; // NNE
                East |= Position >> 15; // SSE
                East &= ~Masks.FileA; // mask 'a'

                West |= Position << 6; // NWW
                West |= Position >> 10; // SWW
                West &= ~Masks.FileG; // mask 'g'
                West |= Position << 15; // NNW
                West |= Position >> 17; // SSW
                West &= ~Masks.FileH; // mask 'h'

                Result[i] = East | West;
                Position <<= 1;
            }
            return Result;
        }

        private static ulong[] BishopMoves() {
            var Result = new ulong[64];
            var Address = new BoardAddress(0);
            for (int i = 0; i < 64; i++) {
                Address.Index = i;
                Result[i] = Masks.Slash[Address.SlashIndex] ^ Masks.BackSlash[Address.BackSlashIndex];
            }
            return Result;
        }

        private static ulong[] RookMoves() {
            var Result = new ulong[64];
            var Address = new BoardAddress(0);
            for (int i = 0; i < 64; i++) {
                Address.Index = i;
                Result[i] = Masks.Ranks[Address.Rank] ^ Masks.Files[Address.File];
            }
            return Result;
        }

        private static ulong[] QueenMoves(ulong[] bishop, ulong[] rook) {
            var Result = new ulong[64];
            for (int i = 0; i < 64; i++)
                Result[i] = bishop[i] | rook[i];
            return Result;
        }

        private static ulong[] KingMoves() {
            var Result = new ulong[64];
            ulong Position = 1;
            ulong North, East, South, West;
            for (int i = 0; i < 64; i++) {
                // reset move board
                East = 0;
                West = 0;

                // build the move board
                East |= (Position << 9); // NE
                East |= (Position << 1); // E
                East |= (Position >> 7); // SE
                East &= ~Masks.FileA; // mask 'a'
                West |= (Position << 7); // NW
                West |= (Position >> 1); // W
                West |= (Position >> 9); // SW
                West &= ~Masks.FileH; // mask 'h'
                North = (Position << 8); // N
                South = (Position >> 8); // S

                Result[i] = North | East | South | West;
                Position <<= 1;
            }
            return Result;
        }
    }
}
