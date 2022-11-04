using Packages.Rider.Editor.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class LegalMovesTest
{

    Board board;

    public LegalMovesTest(Board board) {
        this.board = board;
    }


    public int MoveGenerationTest(int depth) {

        if (depth == 0) return 1;

        List<Move> moves = new List<Move>();

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                Piece piece = board.PieceAt(new BoardPosition(i, j));
                if (piece != null && piece.PieceColor() == board.colorToMove) {
                    moves.AddRange(board.moveGen.GenerateValidMoves(piece, board));
                }
            }
        }

        int numPositions = 0;

        foreach (Move m in moves) {
            Piece movingPiece = board.PieceAt(m.From);
            Piece capturedPiece = board.PieceAt(m.To);
            if (m.From.file == 0 && m.From.rank == 2 && m.To.file == 0 && m.To.rank == 4 && m.Type == Move.MoveType.PawnDoubleMove)
                Debug.Log("Impossible");


            board.MakeSilentMove(movingPiece, m);
            numPositions += MoveGenerationTest(depth - 1);
            board.UnmakeMove(movingPiece, m, capturedPiece);
        }
        return numPositions;
    }


}
