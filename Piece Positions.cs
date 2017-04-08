namespace LittleOwl {
    internal class PiecePositions {
        public PiecePositions() {
            Black = new Player(this);
            White = new Player(this);
        }

        private ulong _Pawns;
        private ulong _Knights;
        private ulong _Bishops;
        private ulong _Rooks;
        private ulong _Queens;
        private ulong _Kings;

        public ulong Pawns { get { return _Pawns; } set { _Pawns = value; } }
        public ulong Knights { get { return _Knights; } set { _Knights = value; } }
        public ulong Bishops { get { return _Bishops; } set { _Bishops = value; } }
        public ulong Rooks { get { return _Rooks; } set { _Rooks = value; } }
        public ulong Queens { get { return _Queens; } set { _Queens = value; } }
        public ulong Kings { get { return _Kings; } set { _Kings = value; } }
        public ulong All { get { return Black.All & White.All; } } // todo PiecePosition.All setter
        public Player Black;
        public Player White;

        public class Player {
            public Player(PiecePositions p) { Parent = p; }
            public ulong All { get { return _All; } } // todo Player.All setter
            public ulong Pawns { get { return _All & Parent.Pawns; } }
            public ulong Knights { get { return _All & Parent.Knights; } }
            public ulong Bishops { get { return _All & Parent.Bishops; } }
            public ulong Rooks { get { return _All & Parent.Rooks; } }
            public ulong Queens { get { return _All & Parent.Queens; } }
            public ulong King { get { return _All & Parent.Queens; } }

            private ulong _All;
            private PiecePositions Parent;
        }
    }
}
