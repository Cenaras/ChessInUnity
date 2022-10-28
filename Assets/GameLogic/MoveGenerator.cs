using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerator {
    public List<Move> GenerateValidMoves(Board board) {

        List<Move> validMoves = new List<Move>();

        // Loop over all pieces and generate valid moves for the right color
        for (int rank = 0; rank < 8; rank++) {
            for (int file = 0; file < 8; file++) {
                BoardPosition pos = new BoardPosition(file, rank);
                Piece piece = board.PieceAt(pos);
                // Check if there is a piece and if it is correct color
                if (piece != null && piece.PieceColor() == board.colorToMove) {
                    // Add the valid moves for this piece to total list of valid moves
                    //Debug.Log("Generating move for " + piece.PieceColor() + " " + piece.GetPieceType());
                    List<Move> validMovesForPiece = GenerateValidMovesForPiece(piece, board);
                    validMoves.AddRange(validMovesForPiece);
                }
            }
        }

        return validMoves;
    }

    /* TODO: We need valid checks for a lot of things besides just the move rules:
        - is the piece pinned?
        - is the piece blocked?
        - are we in check?
        - are we in double check? (Has to move king)
        - Have we castled? Is pawn first move?
        - Can we en pessant?
        - Pawn capture moves?
     
     
     */

    private List<Move> GenerateValidMovesForPiece(Piece piece, Board board) {
        if (piece.GetPieceType() == Piece.PieceType.Pawn) {
            return ValidPawnMoves(piece as Pawn, board);
        }


        return new List<Move>();
    }

    /* TODO: Difference between notion of NoPiece and null? Might have issues with pieces leaving the board otherwise. */

    private List<Move> ValidPawnMoves(Pawn pawn, Board board) {
        List<Move> validMoves = new List<Move>();
        BoardPosition pos = pawn.GetPosition();
        BoardPosition infront = pawn.PieceColor() == GameConstants.GameColor.White
            ? new BoardPosition(pos.file + 1, pos.rank)
            : new BoardPosition(pos.file - 1, pos.rank);

        //Debug.Log("Looking for valid pawn moves");
        //Debug.Log("Pos: " + pos);
        //Debug.Log("Infront: " + infront);

        // If the square infront of the pawn in empty
        if (board.PieceAt(infront) == null) {
            validMoves.Add(new Move(pos, infront));
            //Debug.Log("Adding valid move");
        }

        // If pawn hasn't moved, we can move twice
        if (!pawn.HasMoved) {
            BoardPosition twoInfront = pawn.PieceColor() == GameConstants.GameColor.White
                ? new BoardPosition(pos.file + 2, pos.rank)
                : new BoardPosition(pos.file - 2, pos.rank);

            if (board.PieceAt(twoInfront) == null) {
                validMoves.Add(new Move(pos, twoInfront));
            }

        }

        return validMoves;
    }

}
