using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiritSystem : MonoBehaviour
{
    PlayerActions playerActions;
    void Awake()
    {
        playerActions = new PlayerActions();
            playerActions.Enable();
            playerActions.gameplay.attack.performed += ctx => Attack(ctx);
    }

    private void Attack(InputAction.CallbackContext ctx)
    {
        Debug.Log("Don!");
        const float maxRange = 3;
        // TODO MASK FOR SPIRITS!
        const int spiritLayerMask = 6;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, maxRange, transform.forward, maxRange, spiritLayerMask);
        if (hits.Length > 0)
        {
            RaycastHit nearestHit = hits[0];
            foreach (RaycastHit hit in hits)
            {
                if (hit.distance < nearestHit.distance)
                    nearestHit = hit;
            }

            // Set to default layer. This object is no longer registered as spirit
            nearestHit.transform.gameObject.layer = 0;
            // TODO: Add effect on the spirit. BOOM!

            if (nearestHit.distance < 1)
            {
                // GOOD!
                Debug.Log("GOOD");
            }
            else if (nearestHit.distance < 2)
            {
                Debug.Log("OK");
            }
            else if (nearestHit.distance < 3)
            {
                // Ehh..
                Debug.Log("BAD");
            }
        }
    }

}
