using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    private Player whitePlayer;
    private Player blackPlayer;
    private Player playerToMove;

    private BoardUI boardUI;
    private Board board;
    private static String DEFAULT_STARTING_POSITION = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    // Start is called before the first frame update
    void Start()
    {
        boardUI = FindObjectOfType<BoardUI>();
        StartNewGame(DEFAULT_STARTING_POSITION);
        print("Starting game... Player to move is: " + playerToMove.Username());
        
    }

    // Update is called once per frame
    void Update()
    {
        // Maybe call Update if we do more. 
        // 
        playerToMove.TryGenerateMove();
        boardUI.UpdatePosition(board);
    }



    private void StartNewGame(String FenStartingPosition) {
        board = Board.parseFen(FenStartingPosition);
        whitePlayer = new Player("WhitePlayer", board);
        blackPlayer = new Player("BlackPlayer", board);
        playerToMove = whitePlayer;
    }

}
