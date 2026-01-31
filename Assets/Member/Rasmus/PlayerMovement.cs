using UnityEngine;
using UnityEngine.InputSystem;

public enum Lane
{
    Left = 0,
    Right = 1
}
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float laneDistance = 25.0f;

    // Movement
    public float laneSwitchSpeed = 10f;
    public float bonusSpeedAmplitude = 2;
    public AnimationCurve speedCurve;

    Vector3 targetPosition;
    PlayerActions playerActions;
    InputAction moveAction;

    Lane currentLane;
    private float movementDelta = 0;

    private void Awake()
    {
        playerActions = new PlayerActions();
        playerActions.Enable();
        // "performed" happens once on button down, not repeating
        playerActions.gameplay.move.performed += ctx => ReadMovement(ctx);
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
            targetPosition = new Vector3(-laneDistance, 0);
        else
            targetPosition = new Vector3(laneDistance, 0);
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
            float totalDistanceBetweenLanes = laneDistance * 2;
            float totalDistanceRemaning = totalDistanceBetweenLanes - distanceToGoal;
            float percent = totalDistanceRemaning / totalDistanceBetweenLanes;

            float bonusSpeed = speedCurve.Evaluate(percent);

            if (currentLane == Lane.Left)
            {
                movementDelta -= (laneSwitchSpeed + (bonusSpeed * bonusSpeedAmplitude)) * Time.deltaTime;
            }
            else
            {
                movementDelta += (laneSwitchSpeed + (bonusSpeed * bonusSpeedAmplitude)) * Time.deltaTime;
            }

            Vector3 leftPos = new Vector3(-laneDistance,0);
            Vector3 rightPos = new Vector3(laneDistance,0);
            transform.position = Vector3.Lerp(leftPos, rightPos, movementDelta);
        }
    }
}
