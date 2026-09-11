using UnityEngine;

public class ArmyUnit : MonoBehaviour
{
    [Header("Настройки на движението")]
    public float moveSpeed = 5f;

    private Vector3 targetPosition;
    private ProvinceTile targetTile;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            // Движение към CenterNode
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Пристигане в CenterNode
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;

                // СТЪПКА 1: Завладяване на провинцията при пристигане!
                if (targetTile != null)
                {
                    targetTile.SetOwner(ProvinceOwner.Bulgarians);
                }

                Debug.Log("Армията пристигна и завладя провинцията!");
            }
        }
    }

    public void MoveToProvince(ProvinceTile tile)
    {
        if (tile != null)
        {
            targetTile = tile;
            targetPosition = tile.GetCenterPosition();
            isMoving = true;
        }
    }
}