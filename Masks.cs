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
            Slash[8] = 0x0000000000000000; // not applicable
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
            BackSlash[8] = 0x0000000000000000; // not applicable
            BackSlash[9] = 0x8000000000000000;
            BackSlash[10] = 0x4080000000000000;
            BackSlash[11] = 0x2040800000000000;
            BackSlash[12] = 0x1020408000000000;
            BackSlash[13] = 0x0810204080000000;
            BackSlash[14] = 0x0408102040800000;
            BackSlash[15] = 0x0204081020408000;

            North = new ulong[8];
            North[0] = 0xffffffffffffffff;
            North[1] = 0xffffffffffffff00;
            North[2] = 0xffffffffffff0000;
            North[3] = 0xffffffffff000000;
            North[4] = 0xffffffff00000000;
            North[5] = 0xffffff0000000000;
            North[6] = 0xffff000000000000;
            North[7] = 0xff00000000000000;

            East = new ulong[8];
            East[0] = 0xffffffffffffffff;
            East[1] = 0xfefefefefefefefe;
            East[2] = 0xfcfcfcfcfcfcfcfc;
            East[3] = 0xf8f8f8f8f8f8f8f8;
            East[4] = 0xf0f0f0f0f0f0f0f0;
            East[5] = 0xe0e0e0e0e0e0e0e0;
            East[6] = 0xc0c0c0c0c0c0c0c0;
            East[7] = 0x8080808080808080;

            South = new ulong[8];
            South[0] = 0x00000000000000ff;
            South[1] = 0x000000000000ffff;
            South[2] = 0x0000000000ffffff;
            South[3] = 0x00000000ffffffff;
            South[4] = 0x000000ffffffffff;
            South[5] = 0x0000ffffffffffff;
            South[6] = 0x00ffffffffffffff;
            South[7] = 0xffffffffffffffff;

            West = new ulong[8];
            West[0] = 0x0101010101010101;
            West[1] = 0x0303030303030303;
            West[2] = 0x0707070707070707;
            West[3] = 0x0f0f0f0f0f0f0f0f;
            West[4] = 0x1f1f1f1f1f1f1f1f;
            West[5] = 0x3f3f3f3f3f3f3f3f;
            West[6] = 0x7f7f7f7f7f7f7f7f;
            West[7] = 0xffffffffffffffff;

            NorthEast = new ulong[16];
            BackSlash[0] = 0xfffefcf8f0e0c080;
            BackSlash[1] = 0xfffffefcf8f0e0c0;
            BackSlash[2] = 0xfffffffefcf8f0e0;
            BackSlash[3] = 0xfffffffffefcf8f0;
            BackSlash[4] = 0xfffffffffffefcf8;
            BackSlash[5] = 0xfffffffffffffefc;
            BackSlash[6] = 0xfffffffffffffffe;
            BackSlash[7] = 0xffffffffffffffff;
            BackSlash[8] = 0x0000000000000000; // not applicable
            BackSlash[9] = 0x8000000000000000;
            BackSlash[10] = 0xc080000000000000;
            BackSlash[11] = 0xe0c0800000000000;
            BackSlash[12] = 0xf0e0c08000000000;
            BackSlash[13] = 0xf8f0e0c080000000;
            BackSlash[14] = 0xfcf8f0e0c0800000;
            BackSlash[15] = 0xfefcf8f0e0c08000;

            SouthEast = new ulong[16];
            SouthEast[0] = 0x80c0e0f0f8fcfeff;
            SouthEast[1] = 0xc0e0f0f8fcfeffff;
            SouthEast[2] = 0xe0f0f8fcfeffffff;
            SouthEast[3] = 0xf0f8fcfeffffffff;
            SouthEast[4] = 0xf8fcfeffffffffff;
            SouthEast[5] = 0xfcfeffffffffffff;
            SouthEast[6] = 0xfeffffffffffffff;
            SouthEast[7] = 0xffffffffffffffff;
            SouthEast[8] = 0x0000000000000000; // not applicable
            SouthEast[9] = 0x0000000000000080;
            SouthEast[10] = 0x00000000000080c0;
            SouthEast[11] = 0x000000000080c0e0;
            SouthEast[12] = 0x0000000080c0e0f0;
            SouthEast[13] = 0x00000080c0e0f0f8;
            SouthEast[14] = 0x000080c0e0f0f8fc;
            SouthEast[15] = 0x0080c0e0f0f8fcfe;

            SouthWest = new ulong[16];
            SouthWest[0] = 0x0103070f1f3f7fff;
            SouthWest[1] = 0x000103070f1f3f7f;
            SouthWest[2] = 0x00000103070f1f3f;
            SouthWest[3] = 0x0000000103070f1f;
            SouthWest[4] = 0x000000000103070f;
            SouthWest[5] = 0x0000000000010307;
            SouthWest[6] = 0x0000000000000103;
            SouthWest[7] = 0x0000000000000001;
            SouthWest[8] = 0x0000000000000000; // not applicable
            SouthWest[9] = 0xffffffffffffffff;
            SouthWest[10] = 0x7fffffffffffffff;
            SouthWest[11] = 0x3f7fffffffffffff;
            SouthWest[12] = 0x1f3f7fffffffffff;
            SouthWest[13] = 0x0f1f3f7fffffffff;
            SouthWest[14] = 0x070f1f3f7fffffff;
            SouthWest[15] = 0x03070f1f3f7fffff;

            NorthWest = new ulong[16];
            NorthWest[0] = 0xff7f3f1f0f070301;
            NorthWest[1] = 0x7f3f1f0f07030100;
            NorthWest[2] = 0x3f1f0f0703010000;
            NorthWest[3] = 0x1f0f070301000000;
            NorthWest[4] = 0x0f07030100000000;
            NorthWest[5] = 0x0703010000000000;
            NorthWest[6] = 0x0301000000000000;
            NorthWest[7] = 0x0100000000000000;
            NorthWest[8] = 0x0000000000000000; // not applicable
            NorthWest[9] = 0xffffffffffffffff;
            NorthWest[10] = 0xffffffffffffff7f;
            NorthWest[11] = 0xffffffffffff7f3f;
            NorthWest[12] = 0xffffffffff7f3f1f;
            NorthWest[13] = 0xffffffff7f3f1f0f;
            NorthWest[14] = 0xffffff7f3f1f0f07;
            NorthWest[15] = 0xffff7f3f1f0f0703;
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

        // diagonal lookups (use special diagonal indices)
        public static readonly ulong[] Slash, BackSlash;
        public static readonly ulong[] NorthEast, SouthEast, SouthWest, NorthWest;

        // rank and file lookups
        public static readonly ulong[] Ranks = { Rank1, Rank2, Rank3, Rank4, Rank5, Rank6, Rank7, Rank8 };
        public static readonly ulong[] Files = { FileA, FileB, FileC, FileD, FileE, FileF, FileG, FileH };
        public static readonly ulong[] North, East, South, West;

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
