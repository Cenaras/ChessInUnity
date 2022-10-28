using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants 
{
    public enum GameColor {
        White,
        Black,
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
