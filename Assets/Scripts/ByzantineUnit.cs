using UnityEngine;

public class ByzantineUnit : MonoBehaviour
{
    [Header("Настройки на движението")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Текущо положение")]
    public ProvinceTile currentProvince;

    private Vector3 targetPosition;
    private ProvinceTile targetTile;
    private bool isMoving = false;

    void Start()
    {
        // Поставяме византийската армия в началната ѝ провинция
        if (currentProvince != null)
        {
            transform.position = currentProvince.GetCenterPosition();
            currentProvince.SetOwner(ProvinceOwner.Byzantines);
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // Движение към CenterNode
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Завъртане по посока на движението
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Пристигане в CenterNode
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;

                // Византийците завладяват провинцията
                if (targetTile != null)
                {
                    currentProvince = targetTile;
                    targetTile.SetOwner(ProvinceOwner.Byzantines);
                }

                Debug.Log("Византийската армия пристигна в новата си провинция!");
            }
        }
    }

    // Движение към съседно поле
    public void MoveToProvince(ProvinceTile tile)
    {
        if (tile != null && currentProvince != null && currentProvince.IsAdjacentTo(tile))
        {
            targetTile = tile;
            targetPosition = tile.GetCenterPosition();
            isMoving = true;
        }
        else
        {
            Debug.LogWarning("Византийската армия не може да направи този ход!");
        }
    }
}