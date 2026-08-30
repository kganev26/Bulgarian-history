using System.Collections.Generic;
using UnityEngine;

public class ByzantineAI : MonoBehaviour
{
    public ByzantineUnit byzantineUnit;

    // Метод за извършване на автоматичен ход
    public void MakeTurn()
    {
        if (byzantineUnit == null || byzantineUnit.currentProvince == null) return;

        List<ProvinceTile> possibleMoves = byzantineUnit.currentProvince.adjacentTiles;

        if (possibleMoves.Count == 0) return;

        // Първо търси да превземе българска или неутрална провинция
        ProvinceTile targetTile = null;

        foreach (ProvinceTile tile in possibleMoves)
        {
            if (tile.owner == ProvinceOwner.Bulgarians || tile.owner == ProvinceOwner.Neutral)
            {
                targetTile = tile;
                break;
            }
        }

        // Ако всички съседи са вече византийски, избира случаен съсед
        if (targetTile == null)
        {
            int randomIndex = Random.Range(0, possibleMoves.Count);
            targetTile = possibleMoves[randomIndex];
        }

        // Извършва движението
        byzantineUnit.MoveToProvince(targetTile);
    }
}