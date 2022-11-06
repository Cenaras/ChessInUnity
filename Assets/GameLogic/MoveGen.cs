using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveGen {
    // {1, -1, 8, -8} for rook movement. {7, -7, 9, -9} for bishop movements
    private static readonly int[] rookDirections = new int[] { -1, 1, -8, 8 };
    private static readonly int[] bishopDirections = new int[] { 7, -7, 9, -9 };
    private static readonly int[] queenDirections = new int[] { -1, 1, -8, 8, 7, -7, 9, -9 };
    private static readonly int[] knightDirections = new int[] { 17, -17, 15, -15, 10, -10, 6, -6 };
    // TODO: Pawns also?

    // TODO: Maybe make the map from piece to moves into a 2D array for memory efficiency?
    private static readonly Dictionary<int, int[]> directionsForPiece = new Dictionary<int, int[]>() {
        { Piece.Rook, rookDirections},
        { Piece.Bishop, bishopDirections},
        { Piece.Queen, queenDirections},
        { Piece.King, queenDirections},
        { Piece.Knight, knightDirections},
    };


    // TODO: Precompute distance from square to out of board for each square and direction so we can avoid having to check if target square is valid.
    // Could be int[,] with first entry being square and second being direction.

    /* Generate legal moves for a position */
    public List<Move> LegalMoves(Board board) {
        return PseudoLegalMoves(board);
    }


    /* Generate valid moves for a position, ignoring checks and pins. */
    public List<Move> PseudoLegalMoves(Board board) {
        List<Move> pseudoLegal = new List<Move>();

        // Iterate over the entire board to find moves for each piece. 
        // Idea for optimization: Only recompute for moved piece and pieces who could see the from and to pos of moved piece.
        for (int square = 0; square < 64; square++) {
            int piece = board.PieceAt(square);
            if (Piece.IsColor(piece, board.ColorToMove)) {
                pseudoLegal.AddRange(PseudoLegalMovesForPiece(piece, square, board));
            }
        }




        return pseudoLegal;
    }

    /* TODO: Compute the directions here and pass them to the functions. Probably also do the loop stuff here. AND DO THIS BEFORE EXTENDING WITH MORE.
     * The plan is as follows: Implement knight, use this for the player to make moves and THEN after that refactor this to see it works. */
    // Also: Need to ensure the moves are actually on the board and not outside...
    private List<Move> PseudoLegalMovesForPiece(int piece, int startSquare, Board board) {
        int pieceType = Piece.Type(piece);

        if (pieceType == Piece.Pawn) {

        } else if (pieceType == Piece.King) {

        } else if (pieceType == Piece.Knight) {
            return GenerateKnightMoves(piece, startSquare, board);
        } else {
            return GenerateSlidingPieceMoves(piece, startSquare, board);
        }
        return new List<Move>();
    }


    private List<Move> GenerateKnightMoves(int piece, int startSquare, Board board) {
        List<Move> moves = new List<Move>();
        int pieceType = Piece.Type(piece);
        int[] directionsOffset = directionsForPiece[pieceType];
        
        for (int i = 0; i < directionsOffset.Length; i++) {
            // Knights can jump over pieces so we only need to add the valid positions

            int targetSquare = startSquare + directionsOffset[i];
            // For now, ensure position is on board like this. TODO: Generalize this.
            if (targetSquare >= 0 && targetSquare <= 63) {
                int targetPiece = board.PieceAt(targetSquare);

                // If piece is same color, not a valid move, else add it
                if (!Piece.IsColor(piece, Piece.GetColor(targetPiece))) moves.Add(new Move(startSquare, targetSquare));
            }



        }

        return moves;

    }

    private List<Move> GenerateSlidingPieceMoves(int piece, int startSquare, Board board) {
        List<Move> moves = new List<Move>();

        int pieceType = Piece.Type(piece);
        int[] directionOffsets = directionsForPiece[pieceType];




        // Loop over all possible directions for our piece
        for (int i = 0; i < directionOffsets.Length; i++) {
            // Worst case, we have a distance of 7 to the edge of the board for the direction. TODO: Precompute this for each square and direction to safe memory.
            for (int j = 1; j < 7; j++) {

                int targetSquare = startSquare + directionOffsets[i] * j;

                if (targetSquare >= 0 && targetSquare <= 63) {
                    int pieceOnTarget = board.PieceAt(targetSquare);

                    // If there is a friendly piece, stop the search since we're blocked.
                    if (Piece.IsColor(pieceOnTarget, Piece.GetColor(piece))) break;

                    moves.Add(new Move(startSquare, targetSquare));

                    // If enemy piece at target square, blocked again so stop searching
                    if (Piece.IsColor(pieceOnTarget, Piece.OppositeColor(piece))) break;
                }

            }
        }
        return moves;
    }


}
