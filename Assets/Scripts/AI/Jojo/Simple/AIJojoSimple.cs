﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIJojoSimple : AI
{
    private Team team;

    public override void Init(Team team)
    {
        this.team = team;
        return;
    }

    // Makes the list of all the possible turns, then picks up a random one
    public override TurnResponse PlayTurn()
    {
        return DeepAnalysis();
    }

    // Does a width-first traversal of all the possibilities to get the best path
    private TurnResponse DeepAnalysis()
    {
        BoardState board = InfoGiver.board;
        List<TurnResponse> possibleTurns = GetAllTurns();

        TurnResponse bestTurn = possibleTurns[0];
        float bestPositivity = 0;

        foreach (TurnResponse turn in possibleTurns)
        {
            BoardState newBoard = InfoGiver.ApplyTurn(board, turn);
            float positivity = LightAnalysis(newBoard.table);

            if (bestPositivity < positivity)
            {
                bestTurn = turn;
                bestPositivity = positivity;
            }
        }

        Debug.Log(bestPositivity);

        return bestTurn;
    }

    // Analyses a given situation and returns its "positivity"
    // Game lost -> 0
    // Game won -> 1
    // Other cases -> Proportion of allied pieces in all the pieces in game
    private float LightAnalysis(PieceState[][] table)
    {
        Team winner = InfoGiver.HasGameEnded(table);
        if (winner == team)
            return 1;

        if (winner == Team.none)
        {
            int allies = 0;
            int total = 0;

            // Counting the allied pieces and all the pieces
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (table[i][j] != null)
                    {
                        total += 1;
                        if (table[i][j].team == team)
                            allies += 1;
                    }
                }
            }

            return (float) allies / (float) total;
        }

        return 0;
    }

    private List<TurnResponse> GetAllTurns()
    {
        List<TurnResponse> possibleTurns = new List<TurnResponse>();

        PieceState[][] table = InfoGiver.table;

        // Iterates over all the table twice to find all the possible turns (yes this is disgusting, but done in 1 second)
        TurnResponse tr = null;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                for (int k = 0; k < 5; k++)
                {
                    for (int l = 0; l < 5; l++)
                    {
                        if (team == Team.A)
                        {
                            tr = new TurnResponse(InfoGiver.cardA1.cardName, new Vector2Int(i, j), new Vector2Int(k, l));
                            if (InfoGiver.IsTurnValid(table, InfoGiver.cardA1, InfoGiver.cardA2, Team.A, tr))
                                possibleTurns.Add(tr);

                            tr = new TurnResponse(InfoGiver.cardA2.cardName, new Vector2Int(i, j), new Vector2Int(k, l));
                            if (InfoGiver.IsTurnValid(table, InfoGiver.cardA1, InfoGiver.cardA2, Team.A, tr))
                                possibleTurns.Add(tr);
                        }
                        if (team == Team.B)
                        {
                            tr = new TurnResponse(InfoGiver.cardB1.cardName, new Vector2Int(i, j), new Vector2Int(k, l));
                            if (InfoGiver.IsTurnValid(table, InfoGiver.cardB1, InfoGiver.cardB2, Team.B, tr))
                                possibleTurns.Add(tr);

                            tr = new TurnResponse(InfoGiver.cardB2.cardName, new Vector2Int(i, j), new Vector2Int(k, l));
                            if (InfoGiver.IsTurnValid(table, InfoGiver.cardB1, InfoGiver.cardB2, Team.B, tr))
                                possibleTurns.Add(tr);
                        }
                    }
                }
            }
        }

        return possibleTurns;
    }
}
