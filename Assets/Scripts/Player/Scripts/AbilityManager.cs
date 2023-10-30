using System.Collections;
using UltimateCC;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private PlayerData playerData;

    public enum Phase { Off, Start, End, Active }
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
        public GameObject creaturePrefab;
        NecromancersBladeVariables()
        {
            phase = Phase.Off;
        }
    }

    [System.Serializable]
    public class SoulWalkVariables
    {
        public float ManaDrainPerSecond;
        public bool CanDash;
        [NonEditable] public bool IsInvisible;
        public float walkMultiplier;
        [NonEditable] public Phase phase;
    }

    public NecromancersBladeVariables NecromancersBlade;
    public SoulWalkVariables SoulWalk;

    private void Awake()
    {
        playerData = GetComponent<PlayerMain>().PlayerData;
    }

    private void Start()
    {
        PlayerAttackCollider.OnEnemyKilled += () =>
        {
            if (NecromancersBlade.phase == Phase.Active)
            {
                NecromancersBlade.StoredSoul++;
            }
        };
    }

    private void Update()
    {
        HandleNecromancersBlade();

        HandleSoulWalk();
    }

    public void HandleNecromancersBlade()
    {
        if (NecromancersBlade.phase == Phase.Off)
        {
            return;
        }
        else if (NecromancersBlade.phase == Phase.Active)
        {
            if (NecromancersBlade.CurrentUpgradedAttack == 0)
            {
                NecromancersBlade.phase = Phase.End;
            }
        }

        if (NecromancersBlade.phase == Phase.Start)
        {
            // increase attack multiplier (at playerData) by NecromancersBlade.AttackMultiplier
            NecromancersBlade.CurrentUpgradedAttack = NecromancersBlade.MaxUpgradedAttacks;
            NecromancersBlade.StoredSoul = 0;
            NecromancersBlade.phase = Phase.Active;
        }
        else if (NecromancersBlade.phase == Phase.End)
        {
            // decrease attack multiplier (at playerData) by NecromancersBlade.AttackMultiplier
            if (NecromancersBlade.StoredSoul > 0)
            {
                SpawnCreature(NecromancersBlade.StoredSoul);
                NecromancersBlade.StoredSoul = 0;
            }
            NecromancersBlade.phase = Phase.Off;
        }
    }

    private void SpawnCreature(int number)
    {
        StartCoroutine(HandleCreatureSpawn(number));
    }

    IEnumerator HandleCreatureSpawn(int number)
    {
        for (int i = 1; i <= number; i++)
        {
            Instantiate(NecromancersBlade.creaturePrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.25f);
        }
        yield return null;
    }

    public void HandleSoulWalk()
    {
        if (SoulWalk.phase == Phase.Off)
        {
            return;
        }
        else if (SoulWalk.phase == Phase.Active)
        {
            // Handle Mana Drain
            // if mana 0, phase = Phase.End
        }

        if (SoulWalk.phase == Phase.Start)
        {
            SoulWalk.phase = Phase.Active;
        }
        else if (SoulWalk.phase == Phase.End)
        {
            SoulWalk.phase = Phase.Off;
        }
    }
}
