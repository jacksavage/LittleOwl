namespace LittleOwl {
    internal static class Masks {
        static Masks() {
            Slash = new ulong[16];
            Slash[0] = 0x8040201008040201;
            Slash[1] = 0x4020100804020100;
            Slash[2] = 0x2010080402010000;
            Slash[3] = 0x1008040201000000;
            Slash[4] = 0x0804020100000000;
            Slash[5] = 0x0402010000000000;
            Slash[6] = 0x0201000000000000;
            Slash[7] = 0x0100000000000000;
            Slash[8] = 0x0; // not applicable
            Slash[9] = 0x0000000000000080;
            Slash[10] = 0x0000000000008040;
            Slash[11] = 0x0000000000804020;
            Slash[12] = 0x0000000080402010;
            Slash[13] = 0x0000008040201008;
            Slash[14] = 0x0000804020100804;
            Slash[15] = 0x0080402010080402;

            BackSlash = new ulong[16];
            BackSlash[0] = 0x0102040810204080;
            BackSlash[1] = 0x0001020408102040;
            BackSlash[2] = 0x0000010204081020;
            BackSlash[3] = 0x0000000102040810;
            BackSlash[4] = 0x0000000001020408;
            BackSlash[5] = 0x0000000000010204;
            BackSlash[6] = 0x0000000000000102;
            BackSlash[7] = 0x0000000000000001;
            BackSlash[8] = 0x0; // not applicable
            BackSlash[9] = 0x8000000000000000;
            BackSlash[10] = 0x4080000000000000;
            BackSlash[11] = 0x2040800000000000;
            BackSlash[12] = 0x1020408000000000;
            BackSlash[13] = 0x0810204080000000;
            BackSlash[14] = 0x0408102040800000;
            BackSlash[15] = 0x0204081020408000;
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

        // diagonal lookups
        public static readonly ulong[] Slash;
        public static readonly ulong[] BackSlash;

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
