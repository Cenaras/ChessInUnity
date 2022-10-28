using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerator
{
    public List<Move> GenerateValidMoves(Board board) {

        List<Move> validMoves = new List<Move>();

        // Loop over all pieces and generate valid moves for the right color
        for (int rank = 0; rank < 7; rank++) {
            for (int file = 0; file < 7; file++) {
                BoardPosition pos = new BoardPosition(file, rank);
                Piece piece = board.PieceAt(pos);
                if (piece.PieceColor() == board.colorToMove) {
                    // Add the valid moves for this piece to total list of valid moves
                    List<Move> validMovesForPiece = GenerateValidMovesForPiece(piece);
                    validMoves.AddRange(validMovesForPiece);
                }
            }
        }

        return validMoves;
    }

    private List<Move> GenerateValidMovesForPiece(Piece piece) {
        if (piece.GetPieceType() == Piece.PieceType.Pawn) {
            return ValidPawnMoves(piece as Pawn);
        }


        return new List<Move>();
    }

    private List<Move> ValidPawnMoves(Pawn pawn) {
        List<Move> validMoves = new List<Move>();


        return validMoves;
    }

}
