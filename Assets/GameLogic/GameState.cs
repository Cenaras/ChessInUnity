using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public struct GameState {

    public bool WhiteCastleQueenSide { get; set; }
    public bool WhiteCastleKingSide { get; set; }
    public bool BlackCastleQueenSide { get; set; }
    public bool BlackCastleKingSide { get; set; }
    public int EnPassantSquare { get; set; }


    public static GameState Initialize() {
        return new GameState {
            WhiteCastleQueenSide = true,
            WhiteCastleKingSide = true,
            BlackCastleQueenSide = true,
            BlackCastleKingSide = true,
            EnPassantSquare = -1,
        };
    }

    public GameState Clone() {
        bool newWhiteQueen = WhiteCastleQueenSide;
        bool newWhiteKing = WhiteCastleKingSide;
        bool newBlackQueen = BlackCastleQueenSide;
        bool newBlackKing = BlackCastleQueenSide;
        int newEp = EnPassantSquare;

        return new GameState {
            WhiteCastleQueenSide = newWhiteQueen,
            WhiteCastleKingSide = newWhiteKing,
            BlackCastleQueenSide = newBlackQueen,
            BlackCastleKingSide = newBlackKing,
            EnPassantSquare = newEp,
        };

    }

    public override string ToString() {
        return $"White Queen Side Castle: {WhiteCastleQueenSide}\n" +
            $"White King Side Castle: {WhiteCastleKingSide}\n" +
            $"Black Queen Side Castle: {BlackCastleQueenSide}\n" +
            $"Black King Side Castle: {BlackCastleKingSide}\n" +
            $"En-Passant Square: {EnPassantSquare}";
    }
}
