using UnityEngine;

public class AnimalController : MonoBehaviour
{
    public AnimalData animalData;

    public enum AnimalState { Idle, Wandering, Producing }
    public AnimalState currentState;

    private Vector2 targetPosition;
    private Vector2 startingPosition;
    private float stateTimer;
    private bool isFed = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animalData.animatorController;
        startingPosition = transform.position;
        ChangeState(AnimalState.Idle);
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;
        switch (currentState)
        {
            case AnimalState.Idle: UpdateIdleState(); break;
            case AnimalState.Wandering: UpdateWanderingState(); break;
            case AnimalState.Producing: UpdateProducingState(); break;
        }
    }

    void ChangeState(AnimalState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case AnimalState.Idle:
                stateTimer = Random.Range(animalData.minIdleTime, animalData.maxIdleTime);
                animator.SetBool("isWalking", false);
                break;
            case AnimalState.Wandering:
                stateTimer = Random.Range(animalData.minWanderTime, animalData.maxWanderTime);
                SetNewWanderDestination();
                animator.SetBool("isWalking", true);
                break;
            case AnimalState.Producing:
                stateTimer = animalData.productionTime;
                animator.SetBool("isWalking", false);
                break;
        }
    }

    void UpdateIdleState()
    {
        if (stateTimer <= 0) ChangeState(AnimalState.Wandering);
    }

    void UpdateWanderingState()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, animalData.moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f || stateTimer <= 0)
        {
            ChangeState(AnimalState.Idle);
        }
    }

    void UpdateProducingState()
    {
        if (stateTimer <= 0)
        {
            Instantiate(animalData.productPrefab, transform.position, Quaternion.identity);
            isFed = false;
            ChangeState(AnimalState.Idle);
        }
    }

    void SetNewWanderDestination()
    {
        Vector2 randomDirection = Random.insideUnitCircle * animalData.wanderRadius;
        targetPosition = startingPosition + randomDirection;
    }

    public void Feed()
    {
        if (!isFed)
        {
            isFed = true;
            ChangeState(AnimalState.Producing);
            Debug.Log(animalData.animalName + " fed!");
        }
    }

    void OnMouseDown()
    {
        Feed();
    }
}
