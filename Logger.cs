namespace ChessEngine {
    using System;
    using System.Collections.Generic;

    public class Logger {
        public DateTime StartTime; // when the game started
        public string EnemyName; // name of the enemy
        public TimeSpan TimeAllowed; // time given to each player at game start
        public string InitBoard; // board starting state

        public DateTime StopTime; // when the game ended
        public TimeSpan FriendTime; // time left on friend clock at end of game
        public TimeSpan EnemyTime; // time left on enemy clock at end of game
        public string FinalBoard; // board ending state
        public List<string> Moves = new List<string>(); // what moves were made?
        public WinLossDraw Disposition; // how did the game end?
        public enum WinLossDraw :int {
            Unknown,

            // loss
            LossCheckMate, // friendly player in check mate
            LossInvalidMove, // friendly player made invalid move
            LossThreefoldRep, // friendly player made a threefold rep move
            LossOutOfTime, // friendly player ran out of time
            LossDisconnected, // friendly player disconnected from the game

            // draw
            DrawFiftyMoveRule, // fifty moves without capture or pawn movement
            DrawFriendlyStaleMate, // friendly player in stalemate
            DrawEnemyStaleMate, // enemy player in stalemate
            DrawInsufficientFriendlyMaterial, // friendly player had insufficient material
            DrawInsufficientEnemyMaterial, // enemy player had insufficient material

            // win
            WinCheckMate, // enemy player in check mate
            WinInvalidMove, // enemy player made invalid move
            WinThreefoldRep, // enemy player made a threefold rep move
            WinOutOfTime, // enemy player ran out of time
            WinDisconnected // enemy player disconnected from the game
        }
        public List<string> Log = new List<string>(); // game logs
    }
}