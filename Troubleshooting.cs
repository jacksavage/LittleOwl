namespace LittleOwl {
    using System;
    using System.Text;

    internal class Troubleshooting {
        // make a board rep string with file/rank labels and letters used for each piece
        public string HumanReadable(Board board) {
            var Result = new StringBuilder();
            ulong CurPos;
            string Letter = string.Empty;

            Result.AppendLine("  a b c d e f g h"); // print file header

            // step through the starting index of each rank (left to right)
            for (int start = 56; start > -1; start -= 8) {
                // add rank number (relative to white)
                Result.AppendFormat("{0} ", (start / 8) + 1);

                // step through the other files relative to the starting index
                for (int i = 0; i < 8; i++) {
                    // note this op is slow but this is just for debug
                    CurPos = (ulong)Math.Pow(2, start + i);

                    // if that location is not populated
                    if (((board.Pieces.All & CurPos) == 0)) { Result.Append(". "); } // add a dot
                    else { // if location is populated
                        // get the letter for this piece
                        if ((board.Pieces.Pawns & CurPos) != 0) Letter = "p";
                        else if ((board.Pieces.Knights & CurPos) != 0) Letter = "n";
                        else if ((board.Pieces.Bishops & CurPos) != 0) Letter = "b";
                        else if ((board.Pieces.Rooks & CurPos) != 0) Letter = "r";
                        else if ((board.Pieces.Queens & CurPos) != 0) Letter = "q";
                        else if ((board.Pieces.Kings & CurPos) != 0) Letter = "k";

                        // capitalize if this is a white piece
                        if ((board.Pieces.White.All & CurPos) != 0) Letter = Letter.ToUpper();

                        Result.AppendFormat("{0} ", Letter); // add the letter 
                    }
                }

                Result.AppendLine();
            }

            return Result.ToString();
        }
    }
}
