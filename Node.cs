namespace LittleOwl {
    using System;
    using System.Collections.Generic;

    internal partial class Searcher {
        private class Node {
            // for root node (leave Parent/LastMove null)
            public Node(Board board) { this.Board = board; }

            // for every other node
            public Node(Node parent, Board board, Move lastMove) {
                this.Parent = parent;
                this.Board = board;
                this.LastMove = lastMove;
            }

            // does the active player have insufficient material?
            private bool InsufficientMaterial {
                get {
                    if ((Board.ActivePlayer.Pawns | Board.ActivePlayer.Rooks | Board.ActivePlayer.Queens) == 0) {
                        int NumKnights = Utilities.NumActiveBits(Board.ActivePlayer.Knights);
                        int NumBishops = Utilities.NumActiveBits(Board.ActivePlayer.Bishops);

                        if (NumBishops == 1 && NumKnights == 0) return true; // king and a bishop
                        if (NumBishops == 0 && NumKnights == 1) return true; // king and knight
                        if (NumBishops == 0 && NumKnights == 0) return true; // only a king remaining
                    }

                    return false;
                }
            }

            // check for simplified three move repetition
            private bool MoveRepetitionOccurred {
                get {
                    if (Board.PastMoves.Count < 8) return false;

                    Move[] Moves = Board.PastMoves.ToArray();
                    if (Moves[0] == Moves[4] &&
                        Moves[1] == Moves[5] &&
                        Moves[2] == Moves[6] &&
                        Moves[3] == Moves[7]) return true;

                    return false;
                }
            }

            // is the active player in draw?
            private bool InDraw {
                get {
                    if (Board.HalfMoveClock < 1) return true; // half move clock ran out
                    if (InsufficientMaterial) return true; // insufficient material
                    if (MoveRepetitionOccurred) return true; // move repetition
                    if (MoveGen.MoveMap(Board, true) == 0) return true; // stalemate
                    return false; // not in draw
                }
            }

            // is a given player in check?
            private bool InCheck { get { return (MoveGen.MoveMap(Board, false) & Board.ActivePlayer.King) != 0; } }

            // is the active player in check mate?
            private bool InCheckMate { get { return InCheck && (MoveGen.MoveMap(Board, true) == 0); } }

            // is this an end-of-game state?
            public bool Terminal { get { return InCheckMate || InDraw; } }

            // the material value for a given player
            private int MaterialValue(PiecePositions.Player player) {
                int Result = 0;
                foreach (ulong pawn in Utilities.BitSplit(player.Pawns)) Result += PawnValue;
                foreach (ulong knight in Utilities.BitSplit(player.Knights)) Result += KnightValue;
                foreach (ulong bishop in Utilities.BitSplit(player.Knights)) Result += BishopValue;
                foreach (ulong rook in Utilities.BitSplit(player.Knights)) Result += RookValue;
                foreach (ulong queen in Utilities.BitSplit(player.Knights)) Result += QueenValue;
                return Result;
            }

            // get the material advantage for the active top level player
            private int MaterialAdvantage {
                get {
                    if (TopLevelActivePlayerWhite) return MaterialValue(Board.Pieces.White) - MaterialValue(Board.Pieces.Black);
                    else return MaterialValue(Board.Pieces.Black) - MaterialValue(Board.Pieces.White);
                }
            }

            // is this an inactive state?
            public bool Quiescent {
                get {
                    throw new NotImplementedException(); // todo Searcher.Node.Quiescent propert
                }
            }

            // next possible board states
            public IEnumerable<Node> Children {
                get {
                    foreach (MoveGen.MoveBoardPair mbp in MoveGen.EachMove(Board))
                        yield return new Node(this, mbp.Board, mbp.Move);
                }
            }

            // the utility of a given board to the active player
            public int Utility {
                get {
                    if (InCheckMate) {
                        if (Board.ActiveColorWhite == TopLevelActivePlayerWhite) return int.MinValue; // top level player in check mate
                        else return int.MaxValue; // top level player's opponent is in check mate 
                    }

                    if (InDraw) return int.MinValue + 1; // as close to a loss as you can get (for either player)

                    return MaterialAdvantage; // for top level active player
                }
            }

            private Node Parent;
            public Board Board;
            public Move LastMove = null; // move that got us to this node
            public static bool TopLevelActivePlayerWhite;

            private const int PawnValue = 1; // todo store this in external settings
            private const int KnightValue = 3;
            private const int BishopValue = 3;
            private const int RookValue = 5;
            private const int QueenValue = 9;
        }
    }
}
