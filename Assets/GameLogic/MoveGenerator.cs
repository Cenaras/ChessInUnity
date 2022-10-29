using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MoveGenerator {
    public List<Move> GenerateValidMoves(Piece piece, Board board) {
        if (piece != null) {
            return GenerateValidMovesForPiece(piece, board);
        }
        return new List<Move>();
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
        if (piece.GetPieceType() == Piece.PieceType.Bishop) {
            return ValidBishopMoves(piece as Bishop, board);
        }
        if (piece.GetPieceType() == Piece.PieceType.Rook) {
            return ValidRookMoves(piece as Rook, board);
        }
        return new List<Move>();
    }

    /* TODO: Filter if squares are blocked */
    private List<Move> ValidBishopMoves(Bishop bishop, Board board) {
        BoardPosition piecePos = bishop.GetPosition();
        List<BoardPosition> candidateSquares = bishop.CandidateSquares();

        // This is wrong
        List<Move> validMoves = new List<Move>();
        foreach (BoardPosition pos in candidateSquares) {
            bool visible = IsSquareVisibleFrom(piecePos, pos, board, bishop.PieceColor());
            //Debug.Log("Visible for " + pos + ": " + visible);
            if (IsSquareVisibleFrom(piecePos, pos, board, bishop.PieceColor())) {
                validMoves.Add(new Move(piecePos, pos));
            }
        }


        return validMoves;
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

    private List<Move> ValidRookMoves(Rook rook, Board board) {
        List<Move> validMoves = new List<Move>();
        BoardPosition pos = rook.GetPosition();
        //foreach (BoardPosition candidate in rook.CandidateSquares())


        return new List<Move>();
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

        // Capture of piece if visible and enemy
        foreach (BoardPosition candidateCapture in pawn.CandidateCaptureSquares()) {
            Piece visiblePiece = board.PieceAt(candidateCapture);
            if (visiblePiece != null && visiblePiece.PieceColor() != pawn.PieceColor()) {
                validMoves.Add(new Move(pos, candidateCapture));
            }
        }

        return validMoves;
    }

    // Should be flipped for black pieces?
    private BoardPosition DirectionOfPiece(BoardPosition piecePosition, BoardPosition targetSquare) {
        bool forward = targetSquare.file > piecePosition.file;
        bool right = targetSquare.rank > piecePosition.rank;


        if (forward && right) {
            return new BoardPosition(1, 1);
        }
        if (forward && !right) {
            return new BoardPosition(1, -1);
        }
        if (!forward && right) {
            return new BoardPosition(-1, 1);
        }
        return new BoardPosition(-1, -1);

    }


    /** Use this to determine if a square is visible given start and direction - return false if a piece blocks squares in the direction */
    private bool IsSquareVisibleFrom(BoardPosition start, BoardPosition target, Board board, GameConstants.GameColor color) {

        // TODO: Compute the direction vector. Figure out if a piece is present. Remove everything after the piece on the vector. Profit.
        List<BoardPosition> visibleSquares = new List<BoardPosition>();
        BoardPosition direction = DirectionOfPiece(start, target);

        // Compute all squares in the direction:
        List<BoardPosition> posInDirection = new List<BoardPosition>();
        for(int i = 0; i < 8; i++) {
            BoardPosition pos = BoardPosition.Add(start, BoardPosition.ScalarMult(direction, i));
            Debug.Log("A position in the direction: " + pos);
            Piece.AddPositionIfValid(posInDirection, pos);
        }

        // Loop over all squares - if one is occupied, break from loop - else add the square - exlude own square
        for (int i = 1; i < posInDirection.Count; i++) {
            BoardPosition possiblePos = posInDirection[i];
            Debug.Log("Looking at possible pos " + possiblePos);
            Piece piece = board.PieceAt(possiblePos);
            // If a piece is there, add it and break. //TODO: Own vs enemy piece.
            if (piece != null) {
                // If piece is enemy, allow for capture
                if (piece.PieceColor() != color) {
                    visibleSquares.Add(possiblePos);
                }
                break;
            }
            // No piece, we can move to the square
            visibleSquares.Add(possiblePos);
        }

        // Check if the target square is a visible square
        foreach (BoardPosition visibleSquare in visibleSquares) {
            if (BoardPosition.Equals(visibleSquare, target))
                return true;
        }
        return false;
    }

}
