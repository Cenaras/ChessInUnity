using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerationTest {

    public static string Position2 = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R";
    public static string Position5 = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R";

    // TODO: Should be able to specify who to move in Fen...


    public static int RecursiveMoveGenerationTest(Board board, int depth) {
        if (depth == 0) return 1;

        int positions = 0;

        List<Move> moves = board.MoveGen.LegalMoves(board);
        foreach (Move move in moves) {
            board.MakeMove(move);
            positions +=RecursiveMoveGenerationTest(board, depth - 1);
            board.UnmakeMove(move);
        }

        return positions;

    }


    public static int MoveGenTestStuff(Board board, int depth) {

        // Total depth is 5, meaning argument is 5- how many moves we make by hand here

        // Comparisions with Stockfish - Correct results: a2a3, b2b3, c2c3, d2d3, e2e3, f2f3, g2g3, h2h3
        // Breaks down with a2a4 - 

        // Inspecting a2a4 responses - Correct results: a7a6, b7b6, c7c6, d7d6, e7e6, f7f6, g7g6, h7h6, a7a5
        // Breaks for b7b5

        // a2a4
        board.MakeMove(new Move(8, 24));
        // b7b5
        board.MakeMove(new Move(49, 33));

        // Inspecting a2a4, b7b5 responses - Correct results are: b2b3, c2c3, d2d3, e2e3, f2f3, g2g3, h2h3, a4a5, b2b4, c2c4, d2d4, e2e4, f2f4, g2g4, h2h4, 
        // Breaks for a4b5
        board.MakeMove(new Move(24,33));

        /* Our testing moves below here */

        // Inspecting a2a4 b7b5 a4b5 responses - Everything works it seems... But it doesn't. Maybe unmake move doesn't work for EP?

        board.MakeMove(new Move(48, 32, Move.Type.PawnDouble));
        //board.UnmakeMove(new Move(48, 32, Move.Type.PawnDouble));

        //return RecursiveMoveGenerationTest(board, depth);
        Debug.Log(board.currentState);
        return 0;

    }

}
