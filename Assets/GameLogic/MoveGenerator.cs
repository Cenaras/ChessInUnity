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
                PieceOld piece = board.PieceAt(pos);
                if (piece.PieceColor() == board.colorToMove) {
                    List<Move> validMovesForPiece = GenerateValidMovesForPiece(piece);



                }
            }
        }

        return validMoves;
    }

    private List<Move> GenerateValidMovesForPiece(PieceOld piece) {
        return new List<Move>();
    }
}
