using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveGenerator {
    public List<Move> GenerateValidMoves(Piece piece, Board board) {
        List<Move> pseudoLegalMoves = new List<Move>();
        if (piece != null) {
            pseudoLegalMoves = GenerateValidMovesForPiece(piece, board);
        }
        //return FilterNonLegalMoves(pseudoLegalMoves, board);
        return pseudoLegalMoves;
    }

    /* TODO: Filter the non legal moves somehow in a pretty way. */
    private List<Move> FilterNonLegalMoves(List<Move> pseudoLegalMoves, Board board) {

        List<Move> legalMoves = new List<Move>();

        // For now, trivial inefficient approach. Ideas for later; look at last moved piece - see if it attacks, or if another piece of same color attacked it before it moved
        // If after move, for all pieces who attacked it before it moved, continue in the direction to see if they see king.
        // Might have to rewrite "IsSquareVisible" to something like "AttacksSquare" and include own pieces and then filter them out when trying to make the move?

        /* Trivial solution to check - for all pseudolegal moves m, play m, look at all responses and see if one captures your king. If not, add m to legal, else don't */
        foreach (Move pseudoLegal in pseudoLegalMoves) {
            Piece movingPiece = board.PieceAt(pseudoLegal.From);
            board.MakeMoveNew(movingPiece, pseudoLegal);

            List<Move> responses = new List<Move>();
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    Piece opponentPiece = board.PieceAt(new BoardPosition(i,j));
                    if (opponentPiece != null && opponentPiece.PieceColor() == board.colorToMove) {
                        responses.AddRange(GenerateValidMovesForPiece(opponentPiece, board));
                    }
                }
            }
            // :)

            if (!responses.Any(r => BoardPosition.Equals(r.To, board.GetKingOfColor(GameConstants.OppositeColor(board.colorToMove)).GetPosition()))) {
                legalMoves.Add(pseudoLegal);
            }

            board.UnmakeMoveNew(movingPiece, pseudoLegal);
        }

        return legalMoves;
    }


    /* TODO:
        - is the piece pinned?
        - are we in check?
        - are we in double check? (Has to move king)
        - Can we en pessant?
     */

    private List<Move> GenerateValidMovesForPiece(Piece piece, Board board) {
        Piece.PieceType type = piece.GetPieceType();
        if (type == Piece.PieceType.Pawn)
            return ValidPawnMoves(piece as Pawn, board);
        if (type == Piece.PieceType.Knight)
            return ValidKnightMoves(piece as Knight, board);
        if (type == Piece.PieceType.Rook || type == Piece.PieceType.Bishop || type == Piece.PieceType.Queen)
            return ValidSlidingPieceMoves(piece, board);
        if (type == Piece.PieceType.King)
            return ValidKingMoves(piece as King, board);

        return new List<Move>();
    }

    private List<Move> ValidKingMoves(King king, Board board) {
        List<Move> validMoves = new List<Move>();
        BoardPosition piecePos = king.GetPosition();
        GameConstants.GameColor pieceColor = king.PieceColor();

        foreach (BoardPosition candidateMove in king.CandidateSquares()) {
            Piece piece = board.PieceAt(candidateMove);
            bool emptySquare = piece == null;
            bool sameColorOnSquare = !emptySquare && piece.PieceColor() == pieceColor;
            if (emptySquare || !sameColorOnSquare)
                validMoves.Add(new Move(piecePos, candidateMove));
        }

        // Check for castling
        (bool, bool) castleInformation = CanCastle(king, board);
        Debug.Log("Castle information: " + castleInformation);
        if (castleInformation.Item1)
            validMoves.Add(new Move(piecePos, king.QueenSideCastlePosition(), Move.MoveType.QueenCastle));
        if (castleInformation.Item2)
            validMoves.Add(new Move(piecePos, king.KingSideCastlePosition(), Move.MoveType.KingCastle));

        return validMoves;

    }


    /** (QueenSide, KingSide) */
    private (bool, bool) CanCastle(King king, Board board) {
        bool canCastleQueenSide = false;
        bool canCastleKingSide = false;
        BoardPosition kingPos = king.GetPosition();

        if (king.HasMoved) {
            Debug.Log("King moved");
            return (false, false);
        }

        GameConstants.GameColor kingColor = king.PieceColor();
        if (kingColor == GameConstants.GameColor.White) {
            canCastleKingSide = CanCastleTo(kingPos, king.KingSideCastlePosition(), new BoardPosition(7,0), kingColor, board);
            canCastleQueenSide = CanCastleTo(kingPos, king.QueenSideCastlePosition(), new BoardPosition(0, 0), kingColor, board);
        } else {
            canCastleKingSide = CanCastleTo(kingPos, king.KingSideCastlePosition(), new BoardPosition(7, 7), kingColor, board);
            canCastleQueenSide = CanCastleTo(kingPos, king.QueenSideCastlePosition(), new BoardPosition(0, 7), kingColor, board);
        }

        return (canCastleQueenSide, canCastleKingSide);
    }

    private bool CanCastleTo(BoardPosition kingPos, BoardPosition toSquare, BoardPosition rookPosition, GameConstants.GameColor kingColor, Board board) {

        if (IsSquareVisibleFrom(kingPos, toSquare, board, kingColor)) {

            Piece rookPiece = board.PieceAt(rookPosition);
            if (rookPiece.GetPieceType() == Piece.PieceType.Rook) {
                if (!(rookPiece as Rook).HasMoved && rookPiece.PieceColor() == kingColor) {
                    return true;
                }
            }
        }
        return false;
    
    }

    private List<Move> ValidSlidingPieceMoves(Piece piece, Board board) {
        BoardPosition piecePos = piece.GetPosition();
        List<BoardPosition> candidateSquares = piece.CandidateSquares();
        List<Move> validMoves = new List<Move>();
        foreach (BoardPosition pos in candidateSquares) {
            if (IsSquareVisibleFrom(piecePos, pos, board, piece.PieceColor()))
                validMoves.Add(new Move(piecePos, pos));
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
        bool stillForward = targetSquare.file == piecePosition.file;
        bool stillRight = targetSquare.rank == piecePosition.rank;

        if (stillForward && right) {
            return new BoardPosition(0, 1);
        }
        if (stillForward && !right) {
            return new BoardPosition(0, -1);
        }
        if (forward && stillRight) {
            return new BoardPosition(1, 0);
        }
        if (!forward && stillRight) {
            return new BoardPosition(-1, 0);
        }
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
        List<BoardPosition> visibleSquares = new List<BoardPosition>();
        BoardPosition direction = DirectionOfPiece(start, target);

        // Compute all squares in the direction:
        List<BoardPosition> posInDirection = new List<BoardPosition>();
        for(int i = 0; i < 8; i++) {
            BoardPosition pos = BoardPosition.Add(start, BoardPosition.ScalarMult(direction, i));
            //Debug.Log("A position in the direction: " + pos);
            Piece.AddPositionIfValid(posInDirection, pos);
        }

        // Loop over all squares - if one is occupied, break from loop - else add the square - exlude own square
        for (int i = 1; i < posInDirection.Count; i++) {
            BoardPosition possiblePos = posInDirection[i];
            Piece piece = board.PieceAt(possiblePos);
            // If a piece is there, add it and break.
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
