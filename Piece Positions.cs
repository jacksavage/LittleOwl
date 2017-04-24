using System;

namespace LittleOwl {
    // bitboard representation of chess piece positions
    internal class PiecePositions {
        public PiecePositions() {
            Black = new Player(this);
            White = new Player(this);
            Black.Opponent = White;
            White.Opponent = Black;
        }

        // deep copy constructor
        public PiecePositions(PiecePositions source) {
            _Pawns = source._Pawns;
            _Knights = source._Knights;
            _Bishops = source._Bishops;
            _Rooks = source.Rooks;
            _Queens = source._Queens;
            _Kings = source._Kings;

            Black = new Player(this);
            Black.All = source.Black.All;
            White = new Player(this);
            White.All = source.White.All;
        }

        private ulong _Pawns;
        private ulong _Knights;
        private ulong _Bishops;
        private ulong _Rooks;
        private ulong _Queens;
        private ulong _Kings;

        public ulong All {
            get { return Black.All | White.All; }
            set {
                Black.All &= value;
                White.All &= value;
            }
        }

        public ulong Pawns { get { return _Pawns; } set { _Pawns = value; } } // todo setters should update 'All'
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
                    Pawns &= _All;
                    Knights &= _All;
                    Bishops &= _All;
                    Rooks &= _All;
                    Queens &= _All;
                    King &= _All;
                }
            }

            public ulong Pawns {
                get { return _All & Parent.Pawns; }
                set {
                    _All &= ~Pawns; // clear my pawns from all
                    if ((_All & value) != 0) throw new ArgumentException("tried to put ontop of an existing one");
                    Parent._Pawns = value | (Parent._Pawns & ~_All); // update the pawn board
                    _All |= value; // add back new pawns to all
                }
            } // todo setters should update all
            public ulong Knights {
                get { return _All & Parent.Knights; }
                set {
                    _All &= ~Knights;
                    if ((_All & value) != 0) throw new ArgumentException("tried to put ontop of an existing one");
                    Parent._Knights = value | (Parent._Knights & ~_All);
                    _All |= value;
                }
            }
            public ulong Bishops {
                get { return _All & Parent.Bishops; }
                set {
                    _All &= ~Bishops;
                    if ((_All & value) != 0) throw new ArgumentException("tried to put ontop of an existing one");
                    Parent._Bishops = value | (Parent._Bishops & ~_All);
                    _All |= value;
                }
            }
            public ulong Rooks {
                get { return _All & Parent.Rooks; }
                set {
                    _All &= ~Rooks;
                    if ((_All & value) != 0) throw new ArgumentException("tried to put ontop of an existing one");
                    Parent._Rooks = value | (Parent._Rooks & ~_All);
                    _All |= value;
                }
            }
            public ulong Queens {
                get { return _All & Parent.Queens; }
                set {
                    _All &= ~Queens;
                    if ((_All & value) != 0) throw new ArgumentException("tried to put ontop of an existing one");
                    Parent.Queens = value | (Parent.Queens & ~_All);
                    _All |= value;
                }
            }
            public ulong King {
                get { return _All & Parent.Kings; }
                set {
                    _All &= ~King;
                    if ((_All & value) != 0) throw new ArgumentException("tried to put ontop of an existing one");
                    Parent._Kings = value | (Parent._Kings & ~_All);
                    _All |= value;
                }
            }

            private ulong _All;
            private PiecePositions Parent;
            public Player Opponent = null;
        }
    }
}
