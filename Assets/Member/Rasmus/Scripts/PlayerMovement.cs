// For switching lanes
using UnityEngine;
using UnityEngine.InputSystem;

public enum Lane
{
    Left = 0,
    Right = 1
}
public class PlayerMovement : MonoBehaviour
{
    // Movement
    public float laneSwitchSpeed = 10f;
    public float bonusSpeedAmplitude = 2;
    public AnimationCurve speedCurve;

    Vector3 targetPosition;
    PlayerActions playerActions;

    StageMover stageMover;

    Lane currentLane;
    private float movementDelta = 0;
    Vector3 leftPos;
    Vector3 rightPos;
    float distanceBetweenLanes = 0;

    private void Awake()
    {
        playerActions = new PlayerActions();
        playerActions.Enable();
        // "performed" happens once on button down, not repeating
        playerActions.gameplay.move.performed += ctx => ReadMovement(ctx);

        stageMover = FindAnyObjectByType<StageMover>();
        if (!stageMover)
        {
            Debug.LogError("No stageMover found! Movement won't work. Destroying player...");
            Destroy(this);
            return;
        }

        // Player depth but spawnpoint off-set
        leftPos = transform.position;
        leftPos.x = stageMover.lanePoints[0]. transform. position.x;
        rightPos = transform.position;
        rightPos.x = stageMover.lanePoints[1]. transform. position.x;

        Vector3 lengthVector = leftPos - rightPos;
        distanceBetweenLanes = lengthVector.magnitude;

        SpiritSystem spirit = GetComponent<SpiritSystem>();
        if (spirit)
        {
            spirit.InitShadows(leftPos, rightPos);
        }
        else
        {
            Debug.LogError("PlayerMovenet could not get component SpiritSystem!");
        }
    }

    private void Start()
    {
        SwitchToLane(Lane.Left);
    } 

    // Read player input only
    private void ReadMovement(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        if (value < 0)
        {
            // Player pressed 'Left'. Player wants to go left
            if (currentLane == Lane.Left)
                SwitchToLane(Lane.Right);
            else
                SwitchToLane(Lane.Left);
        }
        else
        {
            // Player pressed 'Right'. Player wants to go right
            if (currentLane == Lane.Right)
                SwitchToLane(Lane.Left);
            else
                SwitchToLane(Lane.Right);
        }
    }

    public void SwitchToLane(Lane newLane)
    {
        if (newLane == Lane.Left) 
            targetPosition = leftPos;
        else
            targetPosition = rightPos;
        currentLane = newLane;
    }

    void Update()
    {
        float distanceToGoal = 0;
        if (currentLane == Lane.Left)
        {
            // Moving to the left
            distanceToGoal = transform.position.x - targetPosition.x;
        }
        else
        {
            // Moving to the right
            distanceToGoal = targetPosition.x - transform.position.x;
        }

        if (transform.position.x != targetPosition.x)
        {
            // Calculate how far (what %) of the total distance we are at
            float totalDistanceRemaning = distanceBetweenLanes - distanceToGoal;
            float percent = totalDistanceRemaning / distanceBetweenLanes;

            float bonusSpeed = speedCurve.Evaluate(percent);

            if (currentLane == Lane.Left)
            {
                movementDelta -= (laneSwitchSpeed + (bonusSpeed * bonusSpeedAmplitude)) * Time.deltaTime;
            }
            else
            {
                movementDelta += (laneSwitchSpeed + (bonusSpeed * bonusSpeedAmplitude)) * Time.deltaTime;
            }

            transform.position = Vector3.Lerp(leftPos, rightPos, movementDelta);
        }
    }
}
