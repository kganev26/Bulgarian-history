using System.Collections.Generic;
using UnityEngine;

public enum TerrainType { Fortress, Plains, Swamp, Coastal }
public enum ProvinceOwner { Bulgarians, Byzantines, Neutral }

public class ProvinceTile : MonoBehaviour
{
    [Header("Данни за провинцията")]
    public string provinceName = "Нова Провинция";
    public TerrainType terrainType = TerrainType.Plains;
    public ProvinceOwner owner = ProvinceOwner.Neutral;

    [Header("Точка за позициониране")]
    public Transform centerNode;

    [Header("Съседи (Законни ходове)")]
    public List<ProvinceTile> adjacentTiles = new List<ProvinceTile>();

    [Header("Цветове / Материали за Собственик")]
    public Color bulgarianColor = Color.green;    // Цвят при българско владение
    public Color byzantineColor = Color.magenta; // Цвят при византийско владение
    public Color neutralColor = Color.gray;       // Цвят за неутрални полета

    private Renderer tileRenderer;

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        UpdateTileColor(); // Задаваме началния цвят според собственика
    }

    // Връща позицията за армията
    public Vector3 GetCenterPosition()
    {
        if (centerNode != null)
            return centerNode.position;
        return transform.position + new Vector3(0, 0.5f, 0);
    }

    // Проверка дали дадено поле е съседно на това
    public bool IsAdjacentTo(ProvinceTile targetTile)
    {
        return adjacentTiles.Contains(targetTile);
    }

    // Промяна на собственика при завладяване
    public void SetOwner(ProvinceOwner newOwner)
    {
        owner = newOwner;
        UpdateTileColor();
        Debug.Log($"{provinceName} вече е под контрола на {owner}!");
    }

    // Визуално обновяване на цвета
    public void UpdateTileColor()
    {
        if (tileRenderer == null) return;

        switch (owner)
        {
            case ProvinceOwner.Bulgarians:
                tileRenderer.material.color = bulgarianColor;
                break;
            case ProvinceOwner.Byzantines:
                tileRenderer.material.color = byzantineColor;
                break;
            case ProvinceOwner.Neutral:
                tileRenderer.material.color = neutralColor;
                break;
        }
    }
}