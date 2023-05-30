using UltimateCC;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    PlayerData playerData;

    public enum Phase { Off, Enter, Exit, On }
    [System.Serializable]
    public class NecromancersBladeVariables
    {
        public float ManaCost;
        [Range(1, 3)] public float AttackMultiplier;
        public float MaxSoul;
        public float MaxUpgradedAttacks;
        [NonEditable] public Phase phase;
        [NonEditable] public int StoredSoul;
        [NonEditable] public float CurrentUpgradedAttack;
        NecromancersBladeVariables()
        {
            phase = Phase.Off;
        }
    }
    // Soul Walk Variables

    public NecromancersBladeVariables NecromancersBlade;

    private void Awake()
    {
        playerData = GetComponent<PlayerMain>().PlayerData;
    }

    private void Update()
    {
        HandleNecromancersBlade();
        //Soul Walk Handler Function
    }

    public void HandleNecromancersBlade()
    {
        if (NecromancersBlade.phase == Phase.Off)
        {
            return;
        }
        else if (NecromancersBlade.phase == Phase.On)
        {
            if (NecromancersBlade.CurrentUpgradedAttack == 0)
            {
                NecromancersBlade.phase = Phase.Exit;
            }
        }

        if (NecromancersBlade.phase == Phase.Enter)
        {
            // increase attack multiplier (at playerData) by NecromancersBlade.AttackMultiplier
            NecromancersBlade.CurrentUpgradedAttack = NecromancersBlade.MaxUpgradedAttacks;
            NecromancersBlade.phase = Phase.On;
        }
        else if (NecromancersBlade.phase == Phase.Exit)
        {
            // decrease attack multiplier (at playerData) by NecromancersBlade.AttackMultiplier
            NecromancersBlade.CurrentUpgradedAttack = 0;
            if (NecromancersBlade.StoredSoul > 0)
            {
                SpawnCreature(NecromancersBlade.StoredSoul);
            }
            NecromancersBlade.phase = Phase.Off;
        }
    }

    private void SpawnCreature(int number)
    {
        // instantiate of creature number times
    }

    // Soul walk Handler Function Declaration

}
