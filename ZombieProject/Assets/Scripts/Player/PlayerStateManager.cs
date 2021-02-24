using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStateManager : MonoBehaviour, IHaveState, IDamagable<int>, IHealable<int>, IKillable
{
    [SerializeField] private HealthBar healthBar;


    private BaseState CurrentState;
    private PlayerCombat playerCombat;
    private Manager gameManager;
    public int HitPoints = 100;
    public float WeaponSpreadMultiplier = 1.0f;

    public bool sprinting = false;
    public bool jumpBlocked = false;
    public bool aimHeld = false;

    public void Damage(int damageTaken)
    {
        HitPoints -= damageTaken;
        healthBar.SetHealth(HitPoints);
        if (HitPoints <= 0)
        {
            Kill();
        }
    }

    public BaseState GetState()
    {
        return CurrentState;
    }

    public void Heal(int healAmount)
    {
        throw new System.NotImplementedException();
    }

    public bool IsDead()
    {
        throw new System.NotImplementedException();
    }

    public void Kill()
    {
        SetState(new PlayerDead(gameObject));
        FindObjectOfType<Manager>().EndGame();


    }

    public void SetState(BaseState state)
    {
        if (CurrentState != null)
        {
            CurrentState.OnStateLeave();
        }

        CurrentState = state;

        if (CurrentState != null)
        {
            CurrentState.OnStateEnter();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentState = new PlayerStanding(gameObject);

        playerCombat = GetComponent<PlayerCombat>();
        if (playerCombat is null)
            throw new UnassignedReferenceException();

        gameManager = FindObjectOfType<Manager>();
        if (gameManager is null)
            throw new UnassignedReferenceException();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootDown()
    {
        playerCombat.ShootDown();
    }

    public void ShootUp()
    {
        playerCombat.ShootUp();
    }

}

public class PlayerStanding : PlayerBaseState
{
    public PlayerStanding(GameObject player) : base(player)
    {

    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        playerStateManager.WeaponSpreadMultiplier = 1.0f;
    }
}

public class PlayerCrouching : PlayerBaseState
{
    public PlayerCrouching(GameObject player) : base(player)
    {
        
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        playerStateManager.WeaponSpreadMultiplier = 0.6f;
    }
}

public class PlayerJumping : PlayerBaseState
{
    public PlayerJumping(GameObject player) : base(player)
    {

    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        playerStateManager.WeaponSpreadMultiplier = 1.5f;

    }
}

public class PlayerDead : PlayerBaseState
{
    public PlayerDead(GameObject player) : base(player)
    {

    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        // Play death animation
        Debug.Log("Player Should play death animation");
    }
}