using System;
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
        QueenCastle,
        PawnDoubleMove,
        EnPassantCapture
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

        if (isWhiteKingFrom && toPos.file == 6 && toPos.rank == 0)
            return true;
        
        if (isBlackKingFrom && toPos.file == 6 && toPos.rank == 7)
            return true;
        
        return false;

    }

    internal static bool IsPawnDoubleMove(Piece piece, BoardPosition fromSquare, BoardPosition targetSquare) {
        if (piece.GetPieceType() != Piece.PieceType.Pawn)
            return false;
        int distance = Math.Abs(fromSquare.rank - targetSquare.rank);
        if (distance == 2 && piece.HasMoved() == false)
            return true;
        return false;
    }

    internal static bool IsEnPassantCapture(Piece piece, Pawn enPassantPawn, BoardPosition fromSquare, BoardPosition targetSquare) {
        if (piece.GetPieceType() != Piece.PieceType.Pawn)
            return false;
 
        if (enPassantPawn == null)
            return false;


        BoardPosition enPassantPos = enPassantPawn.GetPosition();
        bool sameFile = enPassantPos.file == targetSquare.file;
        bool enPassantRank = Math.Abs(enPassantPos.rank - targetSquare.rank) == 1;

        if (sameFile && enPassantRank)
            return true;
        return false;


    }
}