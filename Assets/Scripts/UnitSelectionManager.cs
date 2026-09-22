using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int playerFactionId = 1; // 1 = България

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask clickableLayers;

    private List<Unit> selectedUnits = new List<Unit>();
    private Vector3 startMousePos;
    private bool isDragging = false;

    private void Update()
    {
        HandleSelection();
        HandleOrders();
    }

    private void HandleSelection()
    {
        // 1. Начало на клик с ляв бутон
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Input.mousePosition;
            isDragging = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, clickableLayers))
            {
                Unit clickedUnit = hit.collider.GetComponentInParent<Unit>();

                if (clickedUnit != null && clickedUnit.factionId == playerFactionId)
                {
                    // Ако НЕ е задържан Shift, изчистваме предишната селекция
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        DeselectAll();
                    }
                    SelectUnit(clickedUnit);
                }
                else if (!Input.GetKey(KeyCode.LeftShift))
                {
                    DeselectAll();
                }
            }
            else if (!Input.GetKey(KeyCode.LeftShift))
            {
                DeselectAll();
            }
        }

        // 2. Отпускане на мишката след влачене (Drag Box Selection)
        if (Input.GetMouseButtonUp(0))
        {
            // Ако мишката е преместена на повече от 10 пиксела, правим селекция с рамка
            if (isDragging && Vector3.Distance(startMousePos, Input.mousePosition) > 10f)
            {
                SelectUnitsInBox();
            }
            isDragging = false;
        }
    }

    // Намира всички войници на играча, които попадат в начертания правоъгълник
    private void SelectUnitsInBox()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            DeselectAll();
        }

        Rect rect = GetScreenRect(startMousePos, Input.mousePosition);

        // Вземаме списъка с български единици от ProvinceManager
        if (ProvinceManager.Instance != null)
        {
            foreach (Unit unit in ProvinceManager.Instance.playerUnits)
            {
                if (unit != null)
                {
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
                    if (rect.Contains(screenPos))
                    {
                        SelectUnit(unit);
                    }
                }
            }
        }
    }

    private Rect GetScreenRect(Vector3 screenPos1, Vector3 screenPos2)
    {
        screenPos1.y = Screen.height - screenPos1.y;
        screenPos2.y = Screen.height - screenPos2.y;
        Vector3 topLeft = Vector3.Min(screenPos1, screenPos2);
        Vector3 bottomRight = Vector3.Max(screenPos1, screenPos2);
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    private void HandleOrders()
    {
        // Команда с десен бутон (Движение или Атака)
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, clickableLayers))
            {
                Unit enemyUnit = hit.collider.GetComponentInParent<Unit>();

                // Атака срещу враг
                if (enemyUnit != null && enemyUnit.factionId != playerFactionId)
                {
                    foreach (Unit unit in selectedUnits)
                    {
                        if (unit != null) unit.AttackOrder(enemyUnit);
                    }
                }
                // Движение с леко разпределение (Формация)
                else
                {
                    int count = selectedUnits.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (selectedUnits[i] != null)
                        {
                            // Офсет, за да не вървят всички точно в една и съща точка
                            Vector3 offset = new Vector3((i % 4) * 1.5f, 0, (i / 4) * 1.5f);
                            selectedUnits[i].MoveTo(hit.point + offset);
                        }
                    }
                }
            }
        }
    }

    private void SelectUnit(Unit unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
            unit.SetSelected(true);
        }
    }

    private void DeselectAll()
    {
        foreach (Unit unit in selectedUnits)
        {
            if (unit != null)
            {
                unit.SetSelected(false);
            }
        }
        selectedUnits.Clear();
    }
}