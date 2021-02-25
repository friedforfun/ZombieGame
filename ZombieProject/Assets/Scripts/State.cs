using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base state abstract class for FSM
/// </summary>
public abstract class BaseState
{
    public virtual void OnStateEnter() { }
    
    public virtual void OnStateLeave() { }

    public abstract void UpdateState();
}

/// <summary>
/// Parent state of all npc states
/// Ensures we have a reference to the game object, since states do not inherit Monobehaviour
/// </summary>
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
 
    public PlayerBaseState(GameObject player)
    {
        this.player = player;
        playerStateManager = player.GetComponent<PlayerStateManager>();
        // Player must have state manager script assigned
        if (playerStateManager is null)
            throw new UnassignedReferenceException();
    }


    public override void OnStateEnter()
    {
        base.OnStateEnter();
 
    }

    public override void UpdateState()
    {
        
    }


}
