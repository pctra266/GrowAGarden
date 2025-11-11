using UnityEngine;

public class AnimalController : MonoBehaviour
{
    private AnimalItem animalData;
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip footstepSounds;

    public AudioClip produceSound;

    private Vector2 targetPosition;
    private float wanderTimer;
    private float productionTimer;
    [Tooltip("Gán prefab 'Generic_Pickup' vào đây")]
    public GameObject pickupItemPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        if (pickupItemPrefab == null)
        {
            Debug.LogError("CHƯA GÁN 'pickupItemPrefab' vào AnimalController!", this.gameObject);
            return;
        }

        if (produceSound != null)
        {
            audioSource.PlayOneShot(produceSound);
        }

        Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * 0.7f;

        GameObject productObj = Instantiate(pickupItemPrefab, spawnPosition, Quaternion.identity);
        productObj.name = animalData.productItem.Name + "_Pickup";

        PickUpItem pickupScript = productObj.GetComponent<PickUpItem>();

        if (pickupScript != null)
        {
            pickupScript.SetItem(animalData.productItem, 1);
        }
        else
        {
            Debug.LogError("Prefab 'Generic_Pickup' thiếu script PickUpItem!", this.gameObject);
        }
    }

    void Wander()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

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

        bool isWalking = direction.sqrMagnitude > 0.01f;

        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
        }

        if (footstepSounds != null)
        {
            if (isWalking)
            {
                if (audioSource.clip != footstepSounds || !audioSource.isPlaying)
                {
                    audioSource.clip = footstepSounds;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.clip == footstepSounds && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
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
        wanderTimer = Random.Range(3f, 7f);
    }


    public float GetProductionTimer()
    {
        return productionTimer;
    }

    public void SetProductionTimer(float savedTime)
    {
        productionTimer = savedTime;
    }

    public AnimalItem GetAnimalData()
    {
        return animalData;
    }
}