using UnityEngine;

public class ParryMechanic : MonoBehaviour
{
    [SerializeField] private float parryWindow = 0.2f;
    private float parryTimer = 0f;
    public bool IsParrying => parryTimer > 0f;

    private void Update()
    {
        if (parryTimer > 0)
        {
            parryTimer -= Time.deltaTime;
        }

        // ponytail: Input checking here directly instead of abstracting into a massive input state machine
        if (Input.GetKeyDown(KeyCode.Q)) // Example input
        {
            AttemptParry();
        }
    }

    public void AttemptParry()
    {
        if (parryTimer <= 0)
        {
            parryTimer = parryWindow;
            // TODO: Play parry animation/VFX
        }
    }

    public bool TryDeflectAttack()
    {
        if (IsParrying)
        {
            // Successfully parried
            return true;
        }
        return false;
    }
}
