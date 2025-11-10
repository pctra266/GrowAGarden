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
        if (produceSound != null)
        {
            audioSource.PlayOneShot(produceSound);
        }

        GameObject productObj = new GameObject(animalData.productItem.Name + "_Pickup");

        Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * 0.7f;
        productObj.transform.position = spawnPosition;

        SpriteRenderer sr = productObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 1;
        productObj.AddComponent<BoxCollider2D>().isTrigger = true;

        PickUpItem pickupScript = productObj.AddComponent<PickUpItem>();
        pickupScript.SetItem(animalData.productItem, 1);
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
}