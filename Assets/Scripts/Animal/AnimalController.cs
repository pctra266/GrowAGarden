using UnityEngine;

public class AnimalController : MonoBehaviour
{
    private AnimalItem animalData;
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip footstepSounds;

    // --- ⭐ THÊM BIẾN NÀY CHO ÂM THANH SẢN XUẤT ⭐ ---
    public AudioClip produceSound; // Âm thanh khi sản xuất (bạn gọi là "ăn")

    // AI & Production variables
    private Vector2 targetPosition;
    private float wanderTimer;
    private float productionTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Đã có, rất tốt!
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
        // --- ⭐ LOGIC ÂM THANH SẢN XUẤT (ĂN) ⭐ ---
        // Sử dụng PlayOneShot để âm thanh này không ngắt tiếng bước chân
        if (produceSound != null)
        {
            audioSource.PlayOneShot(produceSound);
        }
        // --- ⭐ KẾT THÚC LOGIC ÂM THANH ⭐ ---

        // 1. Tạo một GameObject trống để chứa sản phẩm
        GameObject productObj = new GameObject(animalData.productItem.Name + "_Pickup");

        // (Phần còn lại của hàm Produce giữ nguyên...)
        // 2. Đặt vị trí cho nó ở gần con vật
        Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * 0.7f;
        productObj.transform.position = spawnPosition;

        // 3. Thêm các component cần thiết...
        SpriteRenderer sr = productObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 1;
        productObj.AddComponent<BoxCollider2D>().isTrigger = true;

        // 4. Thêm và cấu hình script PickUpItem...
        PickUpItem pickupScript = productObj.AddComponent<PickUpItem>();
        pickupScript.SetItem(animalData.productItem, 1);
    }

    void Wander()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // (Logic lật (flip) giữ nguyên...)
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        rb.MovePosition(rb.position + direction * animalData.movementSpeed * Time.fixedDeltaTime);

        // Xác định xem con vật có đang đi không
        bool isWalking = direction.sqrMagnitude > 0.01f;

        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
        }

        // --- ⭐ LOGIC ÂM THANH ĐI LẠI (LOOP) ⭐ ---
        if (footstepSounds != null)
        {
            if (isWalking)
            {
                // Nếu đang đi VÀ audio source đang KHÔNG phát tiếng bước chân
                if (audioSource.clip != footstepSounds || !audioSource.isPlaying)
                {
                    audioSource.clip = footstepSounds;
                    audioSource.loop = true; // Bật lặp lại cho tiếng bước chân
                    audioSource.Play();
                }
            }
            else
            {
                // Nếu dừng LẠI VÀ audio source ĐANG phát tiếng bước chân
                if (audioSource.clip == footstepSounds && audioSource.isPlaying)
                {
                    audioSource.Stop(); // Tắt âm thanh
                }
            }
        }
        // --- ⭐ KẾT THÚC LOGIC ÂM THANH ⭐ ---
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