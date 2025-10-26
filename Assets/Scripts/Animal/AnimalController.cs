using UnityEngine;

public class AnimalController : MonoBehaviour
{
    private AnimalItem animalData;
    private Rigidbody2D rb;
    private Animator animator;

    // AI & Production variables
    private Vector2 targetPosition;
    private float wanderTimer;
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
            productionTimer = animalData.productionTimeInSeconds; // Reset timer
        }
    }

    void Produce()
    {
        // 1. Tạo một GameObject trống để chứa sản phẩm
        GameObject productObj = new GameObject(animalData.productItem.Name + "_Pickup");

        // 2. Đặt vị trí cho nó ở gần con vật
        Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * 0.7f;
        productObj.transform.position = spawnPosition;

        // 3. Thêm các component cần thiết để nó hoạt động như một vật phẩm nhặt được
        SpriteRenderer sr = productObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 1;             // ✅ set order cố định

        productObj.AddComponent<BoxCollider2D>().isTrigger = true;

        // 4. Thêm và cấu hình script PickUpItem của bạn
        PickUpItem pickupScript = productObj.AddComponent<PickUpItem>();
        pickupScript.SetItem(animalData.productItem, 1);
    }

    void Wander()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * animalData.movementSpeed * Time.fixedDeltaTime);

        if (animator != null)
        {
            animator.SetBool("isWalking", direction.sqrMagnitude > 0.01f);
        }
    }

    void UpdateWanderTimer()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0 || Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
            PickNewWanderTarget();
        }
    }

    void PickNewWanderTarget()
    {
        targetPosition = (Vector2)transform.position + Random.insideUnitCircle * 5f;
        wanderTimer = Random.Range(3f, 7f); // Wander for 3 to 7 seconds
    }
}
