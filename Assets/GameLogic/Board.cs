
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


    private readonly Piece[,] pieces;


    public Board(Piece[,] pieces) {
        this.pieces = pieces;
    }

    public Piece[,] getBoardState() {
        return pieces;
    }

    public static Board parseFen(String fen) {
        Dictionary<char, Piece.PieceType> pieceFromSymbol = new Dictionary<char, Piece.PieceType>()
        {
            {'r', Piece.PieceType.Rook},
            {'n', Piece.PieceType.Knight},
            {'b', Piece.PieceType.Bishop},
            {'q', Piece.PieceType.Queen},
            {'k', Piece.PieceType.King},
            {'p', Piece.PieceType.Pawn},
        };
        
        Piece[,] pieces = new Piece[8, 8];
        String[] fenRanks = fen.Split('/');
        for (int i = 0; i < 8; i++) {
            int file = 0;
            String fenRank = fenRanks[i];
            foreach (char c in fenRank) {
                if (Char.IsDigit(c)) file += c;
                else {
                    Piece.PieceType pieceType = pieceFromSymbol[char.ToLower(c)];
                    bool isWhite = char.IsUpper(c);
                    //Debug.Log($"Placing piece {pieceType} at position {i}, {file}, white: {isWhite}");
                    pieces[7 - i, file] = new Piece(pieceType, isWhite);
                    file++;
                }
            }
        }
        return new Board(pieces);
    }


    public void MakeMove(BoardPosition from, BoardPosition to) {
        /*
        How a move works:
        We get the piece.
        We inspect the valid moves from the piece
        We ensure the target position is valid
        We place the piece
         */

        // For now; brute force piece to position

        Piece movingPiece = PieceAt(from);
        MovePiece(movingPiece, from, to);
    }


    private void MovePiece(Piece piece, BoardPosition from, BoardPosition to) {
        if (piece != null) {
            ClearPositionAt(from);
            PlacePieceAt(to, piece);
        } 
    }


    public Piece PieceAt(BoardPosition position) {
        //Debug.Log("Piece")
        return pieces[position.file, position.rank];
    }

    private void PlacePieceAt(BoardPosition position, Piece piece) {
        Debug.Log("Placing piece at " + position);
        pieces[position.file, position.rank] = piece;
    }

    private void ClearPositionAt(BoardPosition position) {
        Debug.Log("Clearing piece at position " + position);
        pieces[position.file, position.rank] = null;
    }

}
