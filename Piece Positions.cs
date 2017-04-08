﻿namespace LittleOwl {
    // bitboard representation of chess piece positions
    internal class PiecePositions {
        public PiecePositions() {
            Black = new Player(this);
            White = new Player(this);
        }

        // deep copy constructor
        public PiecePositions(PiecePositions from) {
            _Pawns = from._Pawns;
            _Knights = from._Knights;
            _Bishops = from._Bishops;
            _Rooks = from.Rooks;
            _Queens = from._Queens;
            _Kings = from._Kings;

            Black = new Player(this);
            Black.All = from.Black.All;
            White = new Player(this);
            White.All = from.White.All;
        }

        private ulong _Pawns;
        private ulong _Knights;
        private ulong _Bishops;
        private ulong _Rooks;
        private ulong _Queens;
        private ulong _Kings;

        public ulong All {
            get { return Black.All & White.All; }
            set {
                Black.All &= value;
                White.All &= value;
            }
        }

        public ulong Pawns { get { return _Pawns; } set { _Pawns = value; } }
        public ulong Knights { get { return _Knights; } set { _Knights = value; } }
        public ulong Bishops { get { return _Bishops; } set { _Bishops = value; } }
        public ulong Rooks { get { return _Rooks; } set { _Rooks = value; } }
        public ulong Queens { get { return _Queens; } set { _Queens = value; } }
        public ulong Kings { get { return _Kings; } set { _Kings = value; } }
        public Player Black;
        public Player White;

        // container for player specific pieces
        public class Player {
            public Player(PiecePositions p) { Parent = p; }

            public ulong All {
                get { return _All; }
                set {
                    _All = value;
                    Parent._Pawns &= Parent.All;
                    Parent._Knights &= Parent.All;
                    Parent._Bishops &= Parent.All;
                    Parent._Rooks &= Parent.All;
                    Parent._Queens &= Parent.All;
                    Parent._Kings &= Parent.All;
                }
            }

            public ulong Pawns { get { return _All & Parent.Pawns; } set { Parent._Pawns = value & (Parent._Pawns & ~_All); } }
            public ulong Knights { get { return _All & Parent.Knights; } set { Parent._Knights = value & (Parent._Knights & ~_All); } }
            public ulong Bishops { get { return _All & Parent.Bishops; } set { Parent._Bishops = value & (Parent._Bishops & ~_All); } }
            public ulong Rooks { get { return _All & Parent.Rooks; } set { Parent._Rooks = value & (Parent._Rooks & ~_All); } }
            public ulong Queens { get { return _All & Parent.Queens; } set { Parent.Queens = value & (Parent.Queens & ~_All); } }
            public ulong King { get { return _All & Parent.Kings; } set { Parent._Kings = value & (Parent._Kings & ~_All); } }

            private ulong _All;
            private PiecePositions Parent;
        }
    }
}
