﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;

    public Piece[][] table;

    public GameObject AKPrefab;
    public GameObject AMPrefab;
    public GameObject BKPrefab;
    public GameObject BMPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        InitTable();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void InitTable()
    {
        // Creating the empty table
        table = new Piece[5][];

        // Managing the starting pieces
        Piece newPiece = null;
        for (int i = 0; i < 5; i++)
        {
            table[i] = new Piece[5];

            if (i == 2)
            {
                newPiece = Instantiate(AKPrefab, transform).GetComponent<Piece>();
                newPiece.SetPosition(new Vector2Int(i, 0));
                table[i][0] = newPiece;

                newPiece = Instantiate(BKPrefab, transform).GetComponent<Piece>();
                newPiece.SetPosition(new Vector2Int(i, 4));
                table[i][4] = newPiece;
            }
            else
            {
                newPiece = Instantiate(AMPrefab, transform).GetComponent<Piece>();
                newPiece.SetPosition(new Vector2Int(i, 0));
                table[i][0] = newPiece;

                newPiece = Instantiate(BMPrefab, transform).GetComponent<Piece>();
                newPiece.SetPosition(new Vector2Int(i, 4));
                table[i][4] = newPiece;
            }
        }
    }

    private void MovePiece(Vector2Int source, Vector2Int destination)
    {
        // Moving the pieces in the table
        Piece tmp = table[source.x][source.y];
        table[source.x][source.y] = table[destination.x][destination.y];
        table[destination.x][destination.y] = tmp;

        // Physically moving the pieces
        if (table[source.x][source.y] != null)
            table[source.x][source.y].SetPosition(new Vector2Int(source.x, source.y));

        if (table[destination.x][destination.y] != null)
            table[destination.x][destination.y].SetPosition(new Vector2Int(destination.x, destination.y));
    }
    public bool IsTurnValid(TurnResponse turn, Team team)
    {
        // The source and the destination have to be in the bounds of the map
        if (turn.source.x < 0 || turn.source.x >= 5 || turn.source.y < 0 || turn.source.y >= 5)
        {
            Debug.LogWarning("The move source is out of bounds");
            return false;
        }
        if (turn.destination.x < 0 || turn.destination.x >= 5 || turn.destination.y < 0 || turn.destination.y >= 5)
        {
            Debug.LogWarning("The move destination is out of bounds");
            return false;
        }

        // The source must contain a movable Piece
        if (table[turn.source.x][turn.source.y] == null || table[turn.source.x][turn.source.y].team != team)
        {
            Debug.LogWarning("The move source does not contain a movable piece");
            return false;
        }

        // The destination must not contain a movable Piece
        if (table[turn.destination.x][turn.destination.y] != null && table[turn.destination.x][turn.destination.y].team == team)
        {
            Debug.LogWarning("The move destination contains a movable piece");
            return false;
        }

        // The player must own the designated card
        Card playedCard = null;
        if (team == Team.A && GameManager.instance.cardA1.cardName == turn.cardName)
            playedCard = GameManager.instance.cardA1;
        if (team == Team.A && GameManager.instance.cardA2.cardName == turn.cardName)
            playedCard = GameManager.instance.cardA2;
        if (team == Team.B && GameManager.instance.cardB1.cardName == turn.cardName)
            playedCard = GameManager.instance.cardB1;
        if (team == Team.B && GameManager.instance.cardB2.cardName == turn.cardName)
            playedCard = GameManager.instance.cardB2;
        if (playedCard == null)
        {
            Debug.LogWarning("The player does not own the right card");
            return false;
        }

        // The move must be allowed by the designated card
        Vector2Int moveVector = turn.destination - turn.source;
        if (moveVector.x < -2 || moveVector.x >= 3 || moveVector.y < -2 || moveVector.y >= 3)
        {
            Debug.LogWarning("This move exceeds 2 cells in height or width");
            return false;
        }
        if (team == Team.A && playedCard.GetMoves()[moveVector.x + 2][moveVector.y + 2] == 0)
        {
            Debug.LogWarning("The \"" + turn.cardName + "\" card does not allow this move");
            return false;
        }
        if (team == Team.B && playedCard.GetMovesReversed()[moveVector.x + 2][moveVector.y + 2] == 0)
        {
            Debug.LogWarning("The \"" + turn.cardName + "\" card does not allow this move");
            return false;
        }

        return true;
    }

    public void ApplyTurn(TurnResponse turn)
    {
        MovePiece(turn.source, turn.destination);
    }

    public Team HasGameEnded ()
    {
        // One of the players have lost their king

        // One of the player's king is on the other player's throne

        return Team.none;
    }
}
