using UnityEngine;

public enum TurnState { PlayerTurn, ByzantineTurn }

public class ProvinceManager : MonoBehaviour
{
    [Header("Текуща фаза на играта")]
    public TurnState currentState = TurnState.PlayerTurn;

    [Header("Армии")]
    public ArmyUnit selectedArmy;
    public ByzantineAI byzantineAI;

    [Header("Текуща провинция на играча")]
    public ProvinceTile currentProvince;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (selectedArmy != null && currentProvince != null)
        {
            selectedArmy.transform.position = currentProvince.GetCenterPosition();
            currentProvince.SetOwner(ProvinceOwner.Bulgarians);
        }
    }

    void Update()
    {
        // Играчът може да клика само когато е негов ред
        if (currentState == TurnState.PlayerTurn && Input.GetMouseButtonDown(0))
        {
            HandleTileClick();
        }
    }

    void HandleTileClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            ProvinceTile clickedTile = hit.collider.GetComponent<ProvinceTile>();

            if (clickedTile != null && selectedArmy != null)
            {
                if (clickedTile == currentProvince) return;

                if (currentProvince != null && currentProvince.IsAdjacentTo(clickedTile))
                {
                    selectedArmy.MoveToProvince(clickedTile);
                    currentProvince = clickedTile;
                }
                else
                {
                    Debug.LogWarning($"Ходът до {clickedTile.provinceName} е невалиден!");
                }
            }
        }
    }

    // Метод за бутона "Край на хода"
    public void EndTurn()
    {
        if (currentState != TurnState.PlayerTurn) return;

        // Преминаваме към хода на Византийците
        currentState = TurnState.ByzantineTurn;
        Debug.Log("Ходът на Българите приключи. Ход на Византийците...");

        // Византийският AI прави своя ход
        if (byzantineAI != null)
        {
            byzantineAI.MakeTurn();
        }

        // Връщаме реда на играча
        currentState = TurnState.PlayerTurn;
        Debug.Log("Ваш ред е!");
    }
}