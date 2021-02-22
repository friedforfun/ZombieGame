using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public virtual void OnStateEnter() { }
    
    public virtual void OnStateLeave() { }

    public abstract void UpdateState();
}


public class NPCBaseState: BaseState
{
    protected GameObject npc;

    public NPCBaseState(GameObject npc)
    {
        this.npc = npc;
    }

    public override void UpdateState()
    {

    }
}


/// <summary>
/// Player States are used to determine what modifers affect 
/// gameplay, for example aim accuracy, ability to zoom ect
/// </summary>
public class PlayerBaseState : BaseState
{

    protected GameObject player;
    protected PlayerStateManager playerStateManager;
    protected bool isGrounded;
 
    public PlayerBaseState(GameObject player)
    {
        this.player = player;
        playerStateManager = player.GetComponent<PlayerStateManager>();
        // Player must have state manager script assigned
        if (playerStateManager is null)
            throw new UnassignedReferenceException();
    }

    /// <summary>
    /// Set isGrounded on all state transitions
    /// </summary>
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        isGrounded = playerStateManager.IsGrounded();
    }

    public override void UpdateState()
    {
        
    }


}
