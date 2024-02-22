using System.Collections;
using UltimateCC;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private PlayerMain player;

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
        public ManaSoulSystem manaSoulSystem;
    }

    public NecromancersBladeVariables NecromancersBlade;
    public SoulWalkVariables SoulWalk;

    private void Awake()
    {
        player = GetComponent<PlayerMain>();
        SoulWalk.manaSoulSystem = GetComponent<ManaSoulSystem>();
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
            SoulWalk.manaSoulSystem.UseMana(SoulWalk.ManaDrainPerSecond * Time.deltaTime);
            if (SoulWalk.manaSoulSystem.currentMana <= 0)
            {
                SoulWalk.phase = Phase.End;
            }
        }

        if (SoulWalk.phase == Phase.Start)
        {
            SoulWalk.phase = Phase.Active;

            if (player.CurrentState == PlayerMain.AnimName.CrouchIdle || player.CurrentState == PlayerMain.AnimName.CrouchWalk)
            {
                SoulWalk.IsInvisible = true;
            }
            player._stateMachine.OnStateEnter += CheckCrouchAndDash;
        }
        else if (SoulWalk.phase == Phase.End)
        {
            SoulWalk.phase = Phase.Off;

            player._stateMachine.OnStateEnter -= CheckCrouchAndDash;
        }
    }

    public void CheckCrouchAndDash(PlayerMain.AnimName state)
    {
        if (state == PlayerMain.AnimName.CrouchWalk || state == PlayerMain.AnimName.CrouchIdle)
        {
            SoulWalk.IsInvisible = true;
        }
        else
        {
            if (state == PlayerMain.AnimName.Dash && !SoulWalk.CanDash)
            {
                SoulWalk.phase = Phase.End;
            }
            SoulWalk.IsInvisible = false;
        }
    }
}
