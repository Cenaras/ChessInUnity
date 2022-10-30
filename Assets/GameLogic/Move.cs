using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Move {
    public Move(BoardPosition fromPos, BoardPosition toPos) {
        From = fromPos;
        To = toPos;
        Type = MoveType.Normal;
    }

    public Move(BoardPosition fromPos, BoardPosition toPos, MoveType moveType) {
        From = fromPos;
        To = toPos;
        Type = moveType;
    }


    public BoardPosition From { get; }
    public BoardPosition To { get; }
    public MoveType Type { get; }

    public override string ToString() {
        return $"{From} --> {To}";
    }

    public enum MoveType {
        Normal,
        KingCastle,
        QueenCastle
    }


    public static bool IsCastleQueenSideMove(BoardPosition fromPos, BoardPosition toPos) {
        bool isWhiteKingFrom = fromPos.file == 4 && fromPos.rank == 0;
        bool isBlackKingFrom = fromPos.file == 4 && fromPos.rank == 7;

        if (isWhiteKingFrom && toPos.file == 2 && toPos.rank == 0)
            return true;
        
        if (isBlackKingFrom && toPos.file == 2 && toPos.rank == 7)
            return true;

        return false;
    }

    public static bool IsCastleKingSideMove(BoardPosition fromPos, BoardPosition toPos) {
        bool isWhiteKingFrom = fromPos.file == 4 && fromPos.rank == 0;
        bool isBlackKingFrom = fromPos.file == 4 && fromPos.rank == 7;

        //Debug.Log("White king: " + isWhiteKingFrom);
        //Debug.Log("From file: " + fromPos.file);
        //Debug.Log("From rank: " + fromPos.rank);
        //Debug.Log("To file: " + toPos.file);
        //Debug.Log("To rank: " + toPos.rank);

        if (isWhiteKingFrom && toPos.file == 6 && toPos.rank == 0)
            return true;
        
        if (isBlackKingFrom && toPos.file == 6 && toPos.rank == 7)
            return true;
        
        return false;

    }

}