using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStateManager : MonoBehaviour, IHaveState, IDamagable<int>, IHealable<int>, IKillable
{
    private BaseState CurrentState;

    public int HitPoints = 100;
    public float WeaponSpreadMultiplier = 1.0f;

    public void Damage(int damageTaken)
    {
        HitPoints -= damageTaken;

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

    public bool IsGrounded()
    {
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class PlayerStanding : PlayerBaseState
{
    private bool Sprinting = false;
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
        // check if jump was pressed 
        //  -> trigger jump animation or go straight to mid jump
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