
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.PlayerLoop;

// Magic for records to work
namespace System.Runtime.CompilerServices {
    internal static class IsExternalInit { }
}

public record BoardPosition(int file, int rank) {

    public bool IsValidPosition() {
        return (rank >= 0 && rank <= 7 && file >= 0 && file <= 7);
    }

    public override string ToString() {
        return $"[{rank}, {file}]";
    }

}

public class Board {


    private readonly PieceOld[,] pieces;
    public GameColor colorToMove;


    private void SwapTurn() { 
        if (colorToMove == GameColor.WHITE) {
            colorToMove = GameColor.BLACK;
        } else {
            colorToMove = GameColor.WHITE;
        }
    }

    public Board(PieceOld[,] pieces) {
        this.pieces = pieces;
        colorToMove = GameColor.WHITE;
    }

    public PieceOld[,] getBoardState() {
        return pieces;
    }

    public static Board parseFen(String fen) {
        Dictionary<char, PieceOld.PieceType> pieceFromSymbol = new Dictionary<char, PieceOld.PieceType>()
        {
            {'r', PieceOld.PieceType.Rook},
            {'n', PieceOld.PieceType.Knight},
            {'b', PieceOld.PieceType.Bishop},
            {'q', PieceOld.PieceType.Queen},
            {'k', PieceOld.PieceType.King},
            {'p', PieceOld.PieceType.Pawn},
        };

        PieceOld[,] pieces = new PieceOld[8, 8];
        String[] fenRanks = fen.Split('/');
        for (int i = 0; i < 8; i++) {
            int file = 0;
            String fenRank = fenRanks[i];
            foreach (char c in fenRank) {
                if (Char.IsDigit(c)) file += c;
                else {
                    PieceOld.PieceType pieceType = pieceFromSymbol[char.ToLower(c)];
                    bool isWhite = char.IsUpper(c);
                    //Debug.Log($"Placing piece {pieceType} at position {i}, {file}, white: {isWhite}");
                    pieces[7 - i, file] = new PieceOld(pieceType, isWhite);
                    file++;
                }
            }
        }
        return new Board(pieces);
    }


    /** Returns true if the move was made */
    public bool MakeMove(GameColor playerColor, BoardPosition from, BoardPosition to) {
        /*
        How a move works:
        We get the piece.
        We inspect the valid moves from the piece
        We ensure the target position is valid
        We place the piece
         */

        // For now; brute force piece to position

        // Moving a piece to its own location is not valid.
        //if (from.Equals(to)) return false;


        PieceOld movingPiece = PieceAt(from);
        // Only move the piece if it exists and is from the right player
        if (movingPiece != null && movingPiece.PieceColor() == playerColor) {
            MovePiece(movingPiece, from, to);
            SwapTurn();
            return true;
        }
        Debug.Log("Move illegal");
        return false;

    }


    private void MovePiece(PieceOld piece, BoardPosition from, BoardPosition to) {
        ClearPositionAt(from);
        PlacePieceAt(to, piece);
    }


    public PieceOld PieceAt(BoardPosition position) {
        //Debug.Log("Piece")
        return pieces[position.file, position.rank];
    }

    private void PlacePieceAt(BoardPosition position, PieceOld piece) {
        Debug.Log("Placing piece at " + position);
        pieces[position.file, position.rank] = piece;
    }

    private void ClearPositionAt(BoardPosition position) {
        Debug.Log("Clearing piece at position " + position);
        pieces[position.file, position.rank] = null;
    }

}