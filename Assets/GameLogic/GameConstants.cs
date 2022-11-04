using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants 
{
    public static BoardPosition BlackQueenSideCastlePos = new BoardPosition(2, 7);
    public static BoardPosition BlackKingSideCastlePos = new BoardPosition(6,7);

    public static BoardPosition WhiteQueenSideCastlePos = new BoardPosition(2, 0);
    public static BoardPosition WhiteKingSideCastlePos = new BoardPosition(6, 0);

    public static BoardPosition BlackQueenRookStartPos = new BoardPosition(0, 7);
    public static BoardPosition BlackKingRookStartPos = new BoardPosition(7, 7);
    public static BoardPosition WhiteQueenRookStartPos = new BoardPosition(0, 0);
    public static BoardPosition WhiteKingRookStartPos = new BoardPosition(7, 0);


    public enum GameColor {
        White,
        Black,
    }

    public static GameColor OppositeColor(GameColor color) {
        switch (color) {
            case GameColor.White: return GameColor.Black;
            default: return GameColor.White;
        }
    }

    public static Sprite GetPieceSprite(Piece piece) {
        string id = PieceId(piece);
        string color = piece.PieceColor() == GameColor.White ? "w" : "b";
        string SpritePath = $"Sprites/{color}{id}";

        return Resources.Load<Sprite>(SpritePath);

    }

    private static string PieceId (Piece piece) {
        Piece.PieceType type = piece.GetPieceType();
        switch (type) {
            case Piece.PieceType.Knight: return "N";
            default: return piece.ToString().Substring(0, 1);
        }
    }



}
