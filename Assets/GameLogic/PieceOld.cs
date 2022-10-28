
using System;
using System.IO;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/* TODO: Make Piece interface and class for each type of piece. */

public class PieceOld {

    private PieceType type;
    private bool isWhite;
    private Sprite pieceSprite;


    public PieceOld(PieceType type, bool isWhite) {
        this.type = type;
        this.isWhite = isWhite;
        pieceSprite = getSpriteForPieceType(type, isWhite);
    }


    public PieceType getType() {
        return type;
    }

    public GameColor PieceColor() {
        return isWhite ? GameColor.WHITE : GameColor.BLACK;
    }

    public enum PieceType : ushort {
        Pawn = 0,
        Knight = 1,
        Bishop = 2,
        Rook = 3,
        Queen = 4,
        King = 5,
    }


    private String shortname(PieceType type) {        
        switch (type) {
            case PieceType.Knight: return "N";
            default: return type.ToString().Substring(0, 1);
        }
    }

    private Sprite coloredSprite(PieceType type, bool isWhite) {
        String id = shortname(type);
        String color = isWhite ? "w" : "b";
        String spritePath = $"Sprites/{color}{id}";

        return Resources.Load<Sprite>(spritePath);
    }

    private Sprite getSpriteForPieceType(PieceType type, bool isWhite) {
        return coloredSprite(type, isWhite);
    }

    public Sprite sprite() {
        return pieceSprite;
    }

}
