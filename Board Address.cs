﻿namespace LittleOwl {
    using System;
    using System.Collections.Generic;

    internal class BoardAddress {
        public int File { get { return Index % 8; } } // zero based
        public int Rank { get { return Index / 8; } } // zero based

        private static ulong[] PositionLookup;
        private static Dictionary<ulong, int> IndexLookup;
        static BoardAddress() {
            PositionLookup = new ulong[64];
            ulong BB = 1;
            for (uint i = 0; i < 64; i++) {
                PositionLookup[i] = BB;
                BB <<= 1;
            }

            IndexLookup = new Dictionary<ulong, int>();
            ulong Bitboard = 1;
            for (int i = 0; i < 64; i++) {
                IndexLookup.Add(Bitboard, i);
                Bitboard <<= 1;
            }
        }

        public int Index { get { return IndexLookup[Position]; } set { Position = PositionLookup[value]; } }
        public int SlashIndex { get { return (Rank - File) & 0xF; } } // todo consider replacing these with array lookups
        public int BackSlashIndex { get { return (Rank + File) ^ 0x7; } }

        private ulong _Position;
        public ulong Position {
            get { return _Position; }
            set {
                if (Utilities.NumActiveBits(value) != 1) throw new ArgumentException(string.Format("num of active bits in \"{0}\" is not equal to one", value));
                _Position = value;
            }
        }

        public BoardAddress(ulong pos) { Position = pos; }

        public BoardAddress(int i) { Index = i; } // todo is this bad form?

        public BoardAddress(char f, int r) { Position = PositionFromFileRank(f, r); }

        public BoardAddress(string fileRank) {
            fileRank = fileRank.Trim();
            if (fileRank.Length != 2) throw new ArgumentException(string.Format("board address \"{0}\" is two characters", fileRank));
            int R;
            if (!int.TryParse(fileRank[1].ToString(), out R)) throw new ArgumentException(string.Format("could not parse rank \"{0}\"", fileRank[1]));

            Position = PositionFromFileRank(fileRank[0], R);
        }

        private ulong PositionFromFileRank(char f, int r) {
            f = char.ToLower(f);
            if (f < 'a' || f > 'h') throw new ArgumentException(string.Format("invalid file \"{0}\"", f));
            if (r < 0 || r > 7) throw new ArgumentException(string.Format("invalid rank \"{0}\"", r));

            int FileIndex = f - 'a';
            int BoardIndex = FileIndex + (r * 8);
            return PositionLookup[BoardIndex];
        }

        public override string ToString() { return string.Format("{0}{1}", (char)(File + 'a'), Rank + 1); }
    } // todo add 'Cached' flag for calculted properties
}
