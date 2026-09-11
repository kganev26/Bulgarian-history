using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int playerFactionId = 1; // Faction ID на играча (1 = България)

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask clickableLayers;

    private List<Unit> selectedUnits = new List<Unit>();

    private void Update()
    {
        HandleSingleSelectionAndOrders();
    }

    private void HandleSingleSelectionAndOrders()
    {
        // Селекция с ляв бутон на мишката
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, clickableLayers))
            {
                Unit clickedUnit = hit.collider.GetComponent<Unit>();

                if (clickedUnit != null && clickedUnit.factionId == playerFactionId)
                {
                    DeselectAll();
                    SelectUnit(clickedUnit);
                }
                else
                {
                    DeselectAll();
                }
            }
            else
            {
                DeselectAll();
            }
        }

        // Команда с десен бутон (Движение или Атака)
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, clickableLayers))
            {
                Unit enemyUnit = hit.collider.GetComponent<Unit>();

                // Ако е кликнато върху противникова единица -> Заповед за Атака
                if (enemyUnit != null && enemyUnit.factionId != playerFactionId)
                {
                    foreach (Unit unit in selectedUnits)
                    {
                        if (unit != null)
                            unit.AttackOrder(enemyUnit);
                    }
                }
                // Ако е кликнато върху земята -> Заповед за Движение
                else
                {
                    foreach (Unit unit in selectedUnits)
                    {
                        if (unit != null)
                            unit.MoveTo(hit.point);
                    }
                }
            }
        }
    }

    private void SelectUnit(Unit unit)
    {
        selectedUnits.Add(unit);
        unit.SetSelected(true);
    }

    private void DeselectAll()
    {
        foreach (Unit unit in selectedUnits)
        {
            if (unit != null)
                unit.SetSelected(false);
        }
        selectedUnits.Clear();
    }
}