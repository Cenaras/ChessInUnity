using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        // When the onMovePlaced action is triggered, call the HandleOnMovePlaced function is called. This listens for the GameEvents.onMovePlaced event...
        boardUI = FindObjectOfType<BoardUI>();
        StartNewGame(DEFAULT_STARTING_POSITION);
    }

    // Update is called once per frame
    void Update()
    {
        // Maybe rename to Update if we do more. 
        // 
        bool moveMade = playerToMove.TryGenerateMove();
        boardUI.UpdatePosition(board);
        if (moveMade) {
            // TODO: Only update when move is made - but for some reason our render doesn't load then...
            playerToMove = (playerToMove.Equals(whitePlayer)) ? blackPlayer : whitePlayer;
        }
        
    }

    private void StartNewGame(String FenStartingPosition) {
        // Maybe make an event for registering a move instead of returning a bool when it does from TryGenerateMove()?
        board = Board.parseFen(FenStartingPosition);
        whitePlayer = new Player("WhitePlayer", GameColor.WHITE, board);
        blackPlayer = new Player("BlackPlayer", GameColor.BLACK, board);
        playerToMove = whitePlayer;
    }


}