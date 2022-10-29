using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerator {
    public List<Move> GenerateValidMoves(Board board) {

        // TODO: Only generate moves for selected piece - pass piece as param to this.

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
        if (piece.GetPieceType() == Piece.PieceType.Knight) {
            return ValidKnightMoves(piece as Knight, board);
        }

        return new List<Move>();
    }

    private List<Move> ValidKnightMoves(Knight knight, Board board) {
        GameConstants.GameColor pieceColor = knight.PieceColor();
        List<Move> validMoves = new List<Move>();
        BoardPosition pos = knight.GetPosition();

        // For all potential moves - if square is empty or enemy piece is there it is a valid move
        foreach (BoardPosition candidatePos in knight.CandidateSquares()) {
            Piece pieceAtTargetSquare = board.PieceAt(candidatePos);
            if (pieceAtTargetSquare == null || pieceAtTargetSquare.PieceColor() != pieceColor) {
                validMoves.Add(new Move(pos, candidatePos));
            }
        }
        return validMoves;
    }

    /* TODO: Difference between notion of NoPiece and null? Might have issues with pieces leaving the board otherwise. */

    private List<Move> ValidPawnMoves(Pawn pawn, Board board) {
        List<Move> validMoves = new List<Move>();
        BoardPosition pos = pawn.GetPosition();

        // Movement of pawn
        foreach (BoardPosition candidatePos in pawn.CandidateSquares()) {
            if (board.PieceAt(candidatePos) == null) {
                validMoves.Add(new Move(pos, candidatePos));
            }
        }

        // Capture of piece
        foreach (BoardPosition candidateCapture in pawn.CandidateCaptureSquares()) {
            if (board.PieceAt(candidateCapture) != null) {
                validMoves.Add(new Move(pos, candidateCapture));
            }
        }

        return validMoves;
    }
}
