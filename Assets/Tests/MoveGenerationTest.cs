using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerationTest {

    public static string Position2 = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R";
    public static string Position5 = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R";

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


}
