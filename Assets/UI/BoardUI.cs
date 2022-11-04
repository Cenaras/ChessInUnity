using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEditorInternal;
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
        for (int rank = 0; rank < 8; rank++) {
            for (int file = 0; file < 8; file++) {
                int square = BoardUtils.SquareFrom(file, rank);
                int piece = board.PieceAt(square);
                pieceRenderers[file, rank].sprite = BoardUtils.SpriteForPiece(piece);
                pieceRenderers[file, rank].transform.position = new Vector2(file, rank);
            }
        }
    }


    public static BoardPosition PositionFromVector(Vector2 vec) {
        float offset = 0.5f;
        int file = (int) Math.Floor(vec.x + offset);
        int rank = (int) Math.Floor(vec.y + offset);

        if (file >= 0 && file <= 7 && rank >= 0 && rank <= 7) {
            return new BoardPosition(file, rank);
        }

        return null;
    }

    internal void HighLightLegalMoves() {
        
    }

    internal void HighlightSelectedSquare(BoardPosition position) {
        squareRenderers[position.File, position.Rank].material.color = boardTheme.selectedSquareColor;   
    }

    internal void DeselectSquare(BoardPosition position) {
        squareRenderers[position.File, position.Rank].material.color = boardTheme.ColorForSquare(position);
    }


    internal void DragPiece(BoardPosition pieceSquare, Vector2 mousePos) {        
        pieceRenderers[pieceSquare.File, pieceSquare.Rank].transform.position = mousePos;
    }

    internal void ResetPiecePosition(BoardPosition selectedPos) {
        pieceRenderers[selectedPos.File, selectedPos.Rank].transform.position = new Vector2(selectedPos.File, selectedPos.Rank);
    }
}
