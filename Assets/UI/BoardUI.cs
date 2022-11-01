using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

// https://github.com/SebLague/Chess-AI
public class BoardUI : MonoBehaviour {

    // Assigned from the Inspector
    public BoardTheme boardTheme;


    SpriteRenderer[,] pieceRenderers = new SpriteRenderer[8, 8];
    MeshRenderer[,] squareRenderers;
    // Start is called before the first frame update
    void Awake() {
        Debug.Log("Generate Board UI");
        GenerateBoardUI();
    }

    /* Note: A bunch of this code is copied from the reference - go back and understand it better... */
    private void GenerateBoardUI() {
        // 8 x 8 array - each square uses a MeshRenderer to display itself
        squareRenderers = new MeshRenderer[8, 8];
        Shader shader = Shader.Find("Unlit/Color");


        for (int rank = 0; rank < 8; rank++) {
            for (int file = 0; file < 8; file++) {
                // Create square
                Transform square = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
                // Set sqaure as child of the BoardUIObject
                square.parent = transform;

                square.position = new Vector2(file, rank);

                // Get the current renderer, and set it to the renderer for the square.
                MeshRenderer squareRenderer = square.GetComponent<MeshRenderer>();

                squareRenderer.material = new Material(shader);
                if ((file + rank) % 2 != 0) {
                    squareRenderer.material.color = boardTheme.whiteSquareColor;
                } else {
                    squareRenderer.material.color = boardTheme.blackSquareColor;
                }

                // Generate a GameObject with a renderer
                SpriteRenderer pieceRenderer = new GameObject("PieceObject").AddComponent<SpriteRenderer>();
                pieceRenderer.transform.parent = transform;
                pieceRenderer.transform.localScale = Vector3.one * 2f;
                pieceRenderer.transform.position = new Vector2(file, rank);


                // Use the generated GameObjectRenderer in the list of renderers
                pieceRenderers[file, rank] = pieceRenderer;
                squareRenderers[file, rank] = squareRenderer;
            }
        }
    }

    public void UpdatePosition(Board board) {
        Debug.Log("Updating position");
        for (int rank = 0; rank < 8; rank++) {
            for (int file = 0; file < 8; file++) {
                Piece piece = board.GetBoardState()[file, rank];

                // If a piece is present, render it otherwise remove textures by setting to null.
                if (piece != null) {
                    pieceRenderers[file, rank].sprite = piece.sprite();
                } else {
                    pieceRenderers[file, rank].sprite = null;
                }
            }
        }
    }


    public void OnMoveMade(Board board) {
        UpdatePosition(board);
        ResetHighlightedSquare();
    }

    public void DragPieceAnim(BoardPosition position, Vector2 mousePos) {
        pieceRenderers[position.file, position.rank].transform.position = mousePos;
    }

    public void ResetPiecePosition(BoardPosition originalPiecePosition) {
        pieceRenderers[originalPiecePosition.file, originalPiecePosition.rank].transform.position =
            new Vector2(
                originalPiecePosition.file,
                originalPiecePosition.rank);
    }


    public void HighlightValidSquares(Piece piece, List<Move> validMoves) {
        foreach (Move move in validMoves) {
            if (piece.GetPosition().Equals(move.From)) {
                squareRenderers[move.To.file, move.To.rank].material.color = boardTheme.highlightSquareColor;
            }
        }
    }

    public void ResetHighlightedSquare() {

        for (int rank = 0; rank < 8; rank++) {
            for (int file = 0; file < 8; file++) {
                Material square = squareRenderers[file, rank].material;
                if ((file + rank) % 2 != 0) {
                    square.color = boardTheme.whiteSquareColor;
                } else {
                    square.color = boardTheme.blackSquareColor;
                }
            }
        }
    }
}
