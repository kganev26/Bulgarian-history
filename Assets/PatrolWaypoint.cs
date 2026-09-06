using UnityEngine;

public class PatrolWaypoint : MonoBehaviour
{
    public bool isOccupied { get; private set; } = false;
    public GameObject currentOccupant { get; private set; }

    public bool Claim(GameObject unit)
    {
        if (isOccupied) return false;
        
        isOccupied = true;
        currentOccupant = unit;
        return true;
    }

    public void Release(GameObject unit)
    {
        if (currentOccupant == unit)
        {
            isOccupied = false;
            currentOccupant = null;
        }
    }
}