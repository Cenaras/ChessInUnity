using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour {

    private Player whitePlayer;
    private Player blackPlayer;
    private Player playerToMove;

    private BoardUI boardUI;
    private Board board;
    private static String DEFAULT_STARTING_POSITION = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";


    // Start is called before the first frame update
    void Start() {
        // When the onMovePlaced action is triggered, call the HandleOnMovePlaced function is called. This listens for the GameEvents.onMovePlaced event...
        boardUI = FindObjectOfType<BoardUI>();
        StartNewGame(DEFAULT_STARTING_POSITION);
        boardUI.UpdatePosition(board);
        //board.PrintBoard();

        //TestLegalMoves(3);
        TestSingleDepth(3);
    }


    private void TestLegalMoves(int depth) {
        LegalMovesTest test = new LegalMovesTest(board);
        Debug.Log($"Running {depth} tests");

        for (int i = 1; i <= depth; i++) {
            DateTime before = DateTime.Now;
            int positions = test.MoveGenerationTest(i);
            DateTime after = DateTime.Now;
            Debug.Log($"Depth: {i} ply. \tResult: {positions} positions\tTime: {(after - before).TotalSeconds} seconds");
        }
    }

    private void TestSingleDepth(int depth) {
        Debug.Log($"Running single test of depth {depth}");
        LegalMovesTest test = new LegalMovesTest(board);
        DateTime before = DateTime.Now;
        int positions = test.MoveGenerationTest(depth);
        DateTime after = DateTime.Now;
        Debug.Log($"Depth: {depth} ply. \tResult: {positions} positions\tTime: {(after - before).TotalSeconds} seconds");
    }


    // Update is called once per frame
    void Update() {
        playerToMove.Update();
        UpdatePlayerToMove();
    }


    private void UpdatePlayerToMove() {
        GameConstants.GameColor colorToMove = board.colorToMove;
        if (colorToMove == GameConstants.GameColor.White) {
            //Debug.Log("New player to move is white");
            playerToMove = whitePlayer;
        } else {
            //Debug.Log("New player to move is black");
            playerToMove = blackPlayer;
        }
    }

    private void StartNewGame(String FenStartingPosition) {
        // Maybe make an event for registering a move instead of returning a bool when it does from TryGenerateMove()?
        Piece[,] pieces = Board.ParseFen(FenStartingPosition);
        board = new Board(pieces, boardUI);
        //board = Board.parseFen(FenStartingPosition);
        whitePlayer = new Player("WhitePlayer", GameConstants.GameColor.White, board);
        blackPlayer = new Player("BlackPlayer", GameConstants.GameColor.Black, board);
        playerToMove = whitePlayer;
    }


}
