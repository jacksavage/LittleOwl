namespace LittleOwl {
    internal static class Masks {
        static Masks() {
            // todo setup masks
        }

        // file constants
        public const ulong FileA = 0x8080808080808080;
        public const ulong FileB = 0x4040404040404040;
        public const ulong FileC = 0x2020202020202020;
        public const ulong FileD = 0x1010101010101010;
        public const ulong FileE = 0x0808080808080808;
        public const ulong FileF = 0x0404040404040404;
        public const ulong FileG = 0x0202020202020202;
        public const ulong FileH = 0x0101010101010101;

        // rank constants
        public const ulong Rank1 = 0x00000000000000ff;
        public const ulong Rank2 = 0x000000000000ff00;
        public const ulong Rank3 = 0x0000000000ff0000;
        public const ulong Rank4 = 0x00000000ff000000;
        public const ulong Rank5 = 0x000000ff00000000;
        public const ulong Rank6 = 0x0000ff0000000000;
        public const ulong Rank7 = 0x00ff000000000000;
        public const ulong Rank8 = 0xff00000000000000;

        // rank and file lookups
        public static readonly ulong[] Ranks = { Rank1, Rank2, Rank3, Rank4, Rank5, Rank6, Rank7, Rank8 };
        public static readonly ulong[] Files = { FileA, FileB, FileC, FileD, FileE, FileF, FileG, FileH };

        // special use cases
        public const ulong Ranks27 = Rank2 | Rank7;
        public const ulong WhiteQueenSideRookInitPos = Rank1 & FileA;
        public const ulong WhiteKingSideRookInitPos = Rank1 & FileH;
        public const ulong BlackQueenSideRookInitPos = Rank8 & FileA;
        public const ulong BlackKingSideRookInitPos = Rank8 & FileH;
        public const ulong WhiteQueenSideRookPostCastlePos = Rank1 & FileF;
        public const ulong WhiteKingSideRookPostCastlePos = Rank1 & FileD;
        public const ulong BlackQueenSideRookPostCastlePos = Rank8 & FileF;
        public const ulong BlackKingSideRookPostCastlePos = Rank8 & FileD;
    }
}
