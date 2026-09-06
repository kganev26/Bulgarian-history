using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ByzantineAI : MonoBehaviour
{
    public enum AIState { Idle, Patrol, Chasing, Attacking, Retreating }

    [Header("AI Settings")]
    public AIState currentState = AIState.Idle;
    public float detectionRadius = 15f;
    [Range(0, 360)] public float viewAngle = 90f;
    public float attackRange = 2.2f;
    public float attackRate = 1.5f;
    public float attackDamage = 20f;

    [Header("Vision Cone Settings")]
    public bool showVisionCone = true;
    public int visionConeSegments = 30; // Брой сегменти за заобляне на дъгата
    public Color visionConeColor = new Color(0.5f, 0.5f, 0.5f, 0.3f); // Сив полупрозрачен цвят

    [Header("Patrol Settings")]
    public PatrolWaypoint[] patrolPoints;
    public float waitTimeAtWaypoint = 3.0f;

    private NavMeshAgent agent;
    private Unit selfUnit;
    private Unit currentTargetUnit;
    private float nextAttackTime;

    private int currentPatrolIndex = -1;
    private PatrolWaypoint currentWaypoint;
    private float waitTimer;
    private bool isWaiting;

    // Компоненти за визуализация на конуса
    private GameObject visionConeObj;
    private MeshFilter visionMeshFilter;
    private MeshRenderer visionMeshRenderer;
    private Mesh visionMesh;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        selfUnit = GetComponent<Unit>();
        SetupVisionConeMesh();
    }

    void Start()
    {
        if (selfUnit != null && selfUnit.data != null)
        {
            attackDamage = selfUnit.data.attackDamage;
            attackRange = selfUnit.data.attackRange;
            attackRate = selfUnit.data.attackSpeed;
        }

        if (patrolPoints.Length > 0)
        {
            MoveToNextAvailableWaypoint();
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Idle:
                FindTarget();
                if (currentTargetUnit == null && patrolPoints.Length > 0)
                {
                    MoveToNextAvailableWaypoint();
                }
                break;

            case AIState.Patrol:
                PatrolBehavior();
                FindTarget();
                break;

            case AIState.Chasing:
                ChaseBehavior();
                break;

            case AIState.Attacking:
                AttackBehavior();
                break;

            case AIState.Retreating:
                break;
        }

        UpdateVisionConeVisibility();
    }

    void LateUpdate()
    {
        if (showVisionCone && visionConeObj.activeSelf)
        {
            DrawVisionCone();
        }
    }

    void SetupVisionConeMesh()
    {
        visionConeObj = new GameObject("VisionCone");
        visionConeObj.transform.SetParent(transform);
        visionConeObj.transform.localPosition = Vector3.up * 0.1f; // Малко над земята
        visionConeObj.transform.localRotation = Quaternion.identity;

        visionMeshFilter = visionConeObj.AddComponent<MeshFilter>();
        visionMeshRenderer = visionConeObj.AddComponent<MeshRenderer>();

        visionMesh = new Mesh();
        visionMesh.name = "Vision Cone Mesh";
        visionMeshFilter.mesh = visionMesh;

        // Създаване на полупрозрачен сив материал
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = visionConeColor;
        visionMeshRenderer.material = mat;
    }

    void UpdateVisionConeVisibility()
    {
        // Скриване при откриване/атакуване на цел или ако е изключен от настройките
        bool shouldBeVisible = showVisionCone && (currentState == AIState.Idle || currentState == AIState.Patrol);
        
        if (visionConeObj.activeSelf != shouldBeVisible)
        {
            visionConeObj.SetActive(shouldBeVisible);
        }
    }

    void DrawVisionCone()
    {
        int numVertices = visionConeSegments + 2;
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[visionConeSegments * 3];

        vertices[0] = Vector3.zero; // Начална точка (единицата)

        float currentAngle = -viewAngle / 2f;
        float angleStep = viewAngle / visionConeSegments;

        for (int i = 0; i <= visionConeSegments; i++)
        {
            Vector3 vertexDir = DirFromAngle(currentAngle, false);
            vertices[i + 1] = vertexDir * detectionRadius;

            if (i < visionConeSegments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            currentAngle += angleStep;
        }

        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        float closestDistance = Mathf.Infinity;
        Unit bestTarget = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Bulgarian"))
            {
                Unit targetUnit = hit.GetComponent<Unit>();
                if (targetUnit == null)
                {
                    targetUnit = hit.GetComponentInParent<Unit>();
                }

                if (targetUnit != null && targetUnit.CanBeTargeted())
                {
                    Vector3 directionToTarget = (targetUnit.transform.position - transform.position).normalized;

                    if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2f)
                    {
                        float distance = Vector3.Distance(transform.position, targetUnit.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            bestTarget = targetUnit;
                        }
                    }
                }
            }
        }

        if (bestTarget != null)
        {
            ClearCurrentTarget();
            ReleaseCurrentWaypoint();

            currentTargetUnit = bestTarget;
            currentTargetUnit.RegisterAttacker(this);

            isWaiting = false;
            waitTimer = 0f;
            currentState = AIState.Chasing;
        }
    }

    void PatrolBehavior()
    {
        if (patrolPoints.Length == 0) return;

        if (currentWaypoint == null)
        {
            MoveToNextAvailableWaypoint();
            return;
        }

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                MoveToNextAvailableWaypoint();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaiting = true;
            waitTimer = 0f;
        }
    }

    void MoveToNextAvailableWaypoint()
    {
        if (patrolPoints.Length == 0) return;

        ReleaseCurrentWaypoint();

        int startIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        int checkIndex = startIndex;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[checkIndex] != null && patrolPoints[checkIndex].Claim(gameObject))
            {
                currentPatrolIndex = checkIndex;
                currentWaypoint = patrolPoints[checkIndex];
                agent.destination = currentWaypoint.transform.position;
                currentState = AIState.Patrol;
                return;
            }

            checkIndex = (checkIndex + 1) % patrolPoints.Length;
        }

        currentState = AIState.Idle;
    }

    void ReleaseCurrentWaypoint()
    {
        if (currentWaypoint != null)
        {
            currentWaypoint.Release(gameObject);
            currentWaypoint = null;
        }
    }

    void ClearCurrentTarget()
    {
        if (currentTargetUnit != null)
        {
            currentTargetUnit.UnregisterAttacker(this);
            currentTargetUnit = null;
        }
    }

    void ChaseBehavior()
    {
        if (currentTargetUnit == null)
        {
            ResetToPatrolOrIdle();
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTargetUnit.transform.position);

        if (distance <= attackRange)
        {
            agent.ResetPath();
            currentState = AIState.Attacking;
        }
        else if (distance > detectionRadius * 1.5f)
        {
            ClearCurrentTarget();
            ResetToPatrolOrIdle();
        }
        else
        {
            agent.SetDestination(currentTargetUnit.transform.position);
        }
    }

    void AttackBehavior()
    {
        if (currentTargetUnit == null)
        {
            ResetToPatrolOrIdle();
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTargetUnit.transform.position);
        if (distance > attackRange)
        {
            currentState = AIState.Chasing;
            return;
        }

        Vector3 direction = (currentTargetUnit.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackRate;
            currentTargetUnit.TakeDamage(attackDamage);
        }
    }

    void ResetToPatrolOrIdle()
    {
        ClearCurrentTarget();
        isWaiting = false;
        waitTimer = 0f;
        MoveToNextAvailableWaypoint();
    }

    void OnDisable()
    {
        ReleaseCurrentWaypoint();
        ClearCurrentTarget();
    }

    void OnDestroy()
    {
        ReleaseCurrentWaypoint();
        ClearCurrentTarget();
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}