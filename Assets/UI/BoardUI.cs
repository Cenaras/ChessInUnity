using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://github.com/SebLague/Chess-AI
public class BoardUI : MonoBehaviour {

    private static Color whiteColor = new Color(0.961f, 0.784f, 0.784f, 1);
    private static Color blackColor = new Color(0.961f, 0.519f, 0.519f, 1);

    SpriteRenderer[,] pieceRenderers;

    // Start is called before the first frame update
    void Start() {
        generateBoardUI();
    }

    /* Note: A bunch of this code is copied from the reference - go back and understand it better... */
    private void generateBoardUI() {
        // 8 x 8 array - each square uses a MeshRenderer to display itself
        MeshRenderer[,] squareRenderers = new MeshRenderer[8, 8];
        Shader shader = Shader.Find("Unlit/Color");

        pieceRenderers = new SpriteRenderer[8, 8];


        for (int rank = 0; rank < 8; rank++) {
            for (int file = 0; file < 8; file++) {
                // Create square
                Transform square = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
                // Set sqaure as child of the BoardUIObject
                square.parent = transform;

                // Testing stuff
                square.position = new Vector2(rank, file);


                // Get the current renderer, and set it to the renderer for the square.
                MeshRenderer squareRenderer = squareRenderers[file, rank];
                squareRenderer = square.GetComponent<MeshRenderer>();
                // Create new material for square

                squareRenderer.material = new Material(shader);
                if ((file + rank) % 2 == 0) {
                    squareRenderer.material.color = whiteColor;
                } else {
                    squareRenderer.material.color = blackColor;
                }

                // Generate a GameObject with a renderer
                SpriteRenderer pieceRenderer = new GameObject("PieceObject").AddComponent<SpriteRenderer>();
                pieceRenderer.transform.parent = transform;
                pieceRenderer.transform.localScale = Vector3.one * 2f;
                pieceRenderer.transform.position = new Vector2(rank, file);

                // Use the generated GameObjectRenderer in the list of renderers
                pieceRenderers[file, rank] = pieceRenderer;
            }
        }
    }


    public void UpdatePosition(Board board) {
        // Idea: In here we get the sprite renderer for each position and query the board for piece type
        //Piece piece = board.getBoardState()[0, 0];
        //pieceRenderers[0, 0].sprite = piece.sprite();

        for (int rank = 0; rank < 8; rank++) {
            for (int file = 0; file < 8; file++) {
                Piece piece = board.getBoardState()[file, rank];

                // If a piece is present, render it otherwise remove textures by setting to null.
                if (piece != null) {
                    pieceRenderers[file, rank].sprite = piece.sprite();
                }
                else {
                    pieceRenderers[file, rank].sprite = null;
                }
            }
        }

    }
}
