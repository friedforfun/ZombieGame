using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStateManager : MonoBehaviour, IHaveState, IDamagable<int>, IHealable<int>, IKillable
{
    private PlayerBaseState CurrentState;


    public void Damage(int damageTaken)
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }

    public void SetState(BaseState state)
    {
        throw new System.NotImplementedException();
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
}

public class PlayerCrouching : PlayerBaseState
{
    public PlayerCrouching(GameObject player) : base(player)
    {

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
        // check if jump was pressed 
        //  -> trigger jump animation or go straight to mid jump
    }
}