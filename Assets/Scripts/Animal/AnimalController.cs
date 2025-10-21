using UnityEngine;

public class AnimalController : MonoBehaviour
{
    private AnimalItem animalData;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 targetPosition;
    private float wanderTimer;
    public float wanderTimeMin = 3f;
    public float wanderTimeMax = 7f;

    private float productionTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Initialize(AnimalItem data)
    {
        animalData = data;
        gameObject.name = animalData.Name + "_Instance";

        productionTimer = animalData.productionTimeInSeconds;
        PickNewWanderTarget();
    }

    private void Update()
    {
        UpdateProduction();
        UpdateWanderTimer();
    }

    private void FixedUpdate()
    {
        Wander();
    }

    void UpdateProduction()
    {
        productionTimer -= Time.deltaTime;
        if (productionTimer <= 0)
        {
            Produce();
            productionTimer = animalData.productionTimeInSeconds;
        }
    }

    void Produce()
    {
        if (AnimalManager.Instance == null || AnimalManager.Instance.productPickupPrefab == null || animalData.productItem == null) { Debug.LogError("Thi?u tham chi?u c?n thi?t ?? t?o s?n ph?m!"); return; }

        Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * 0.7f;

        GameObject productObj = Instantiate(AnimalManager.Instance.productPickupPrefab, spawnPosition, Quaternion.identity);

        PickUpItem pickupScript = productObj.GetComponent<PickUpItem>();
        if (pickupScript != null)
        {
            pickupScript.SetItem(animalData.productItem, 1);
        }

        Debug.Log(animalData.Name + " ?ã t?o ra s?n ph?m!");
    }

    void Wander()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * animalData.movementSpeed * Time.fixedDeltaTime);

        if (animator != null)
        {
            animator.SetBool("isWalking", direction.sqrMagnitude > 0);
        }
    }

    void UpdateWanderTimer()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0 || Vector2.Distance(transform.position, targetPosition) < 0.2f) { PickNewWanderTarget(); }
    }

    void PickNewWanderTarget()
    {
        targetPosition = (Vector2)transform.position + Random.insideUnitCircle * 5f;
        wanderTimer = Random.Range(wanderTimeMin, wanderTimeMax);
    }
}
