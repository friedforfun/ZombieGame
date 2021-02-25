using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;

public class ZombieBehaviour : MonoBehaviour, IKillable, IDamagable<int>, IHaveState
{

    [SerializeField] public float MovementSpeed = 1f;
    [SerializeField] private Rig HeadRig;
    [SerializeField] public GameObject LookTarget;
    [SerializeField] public Transform LineOfSightCheckPoint;
    [SerializeField] public NavMeshAgent agent;

    [SerializeField] public float DetectPlayerRange = 8f;
    [SerializeField] private int HitDamage = 100;

    private int HitPoints = 200;
    private BaseState CurrentState;
    public Animator animator;
    public Rigidbody zombieRigidbody;

    public GameObject player;
    private float stateUpdateTickRate = 0.02f;
    public float attackRange;

    public float AlertTimer = 2f;
    private float rigTransitionSpeed = 1f;
    private bool alertTriggered = false;

    private bool canDamage = true;

    public void Damage(int damageTaken)
    {
        HitPoints = HitPoints - damageTaken;
        //Debug.Log("ZOMBIE TAKES DAMAGE!");
        animator.SetTrigger("TakeDamage");
        //animator.ResetTrigger("TakeDamage");
        if (HitPoints <= 0 && !(CurrentState is ZombieDead))
        {
            Kill();
        }

        if (CurrentState is ZombieUnaware)
        {
            SetState(new ZombieAlerted(gameObject));
        }

    }

    public void Kill()
    {
        SetState(new ZombieDead(gameObject));
    }

    public void SetState(BaseState nextState)
    {
        if (CurrentState != null)
        {
            CurrentState.OnStateLeave();
        }

        CurrentState = nextState;

        if (CurrentState != null)
        {
            CurrentState.OnStateEnter();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        zombieRigidbody = GetComponent<Rigidbody>();
        CurrentState = new ZombieIdle(gameObject);
        HeadRig.weight = 0f;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            player = players[0];
        }

        StartCoroutine(CheckState());
    }

    // Update is called once per frame
    void Update()
    {
        if (alertTriggered && !animator.GetBool("InAttackRange") && !(CurrentState is ZombieDead))
        {
            lookAtPlayer();
            HeadRig.weight = Mathf.MoveTowards(HeadRig.weight, 1f, Time.deltaTime * rigTransitionSpeed);
        }
        else
        {
            HeadRig.weight = Mathf.MoveTowards(HeadRig.weight, 0f, Time.deltaTime * rigTransitionSpeed);
        }
    }

    void FixedUpdate()
    {

    }



    public void ApplyDamage(GameObject player)
    {
        if (IsDead()) return;

        if (canDamage)
        {
            canDamage = false;
            IDamagable<int> damagable = player.GetComponentInParent<IDamagable<int>>();
            damagable.Damage(HitDamage);
            StartCoroutine(resetCanDamage());
        }
    }
 
    public void Attack()
    {
        Debug.Log("Attack Player");
        animator.SetBool("InAttackRange", true);
    }

    public void EndAttack()
    {
        animator.SetBool("InAttackRange", false);
    }


    public void Alert()
    {
        if (!alertTriggered)
            StartCoroutine(beAlerted());
    }

    private void lookAtPlayer()
    {
        LookTarget.transform.position = player.transform.position;
    }


    private IEnumerator beAlerted()
    {
        alertTriggered = true;
        yield return new WaitForSeconds(AlertTimer);
        if (CurrentState is ZombieUnaware)
            SetState(new ZombieAlerted(gameObject));
    }

    private void CleanupListeners()
    {

    }

    private IEnumerator triggerDeath(float delay)
    {
        yield return new WaitForSeconds(delay);
        CleanupListeners();
        Destroy(gameObject, 2f);
    }

    public void DestroyGameobject(float delay)
    {
        IEnumerator death = triggerDeath(delay);
        StartCoroutine(death);
    }

    public BaseState GetState()
    {
        return CurrentState;
    }

    public bool IsDead()
    {
        if (CurrentState is ZombieDead) return true;
        else return false;
    }

    private IEnumerator resetCanDamage()
    {
        yield return new WaitForSeconds(2f);
        canDamage = true;
    }

    private IEnumerator CheckState()
    {
        for (; ; )
        {
            CurrentState.UpdateState();
            yield return new WaitForSeconds(stateUpdateTickRate);
        }
    }

}


public class ZombieBaseState : NPCBaseState
{

    protected ZombieBehaviour zombie;
    protected float enterStateTime;
    protected float initAnimSpeed;
    private float notifyNearbyDistance = 8f;

    public static ZombieDead zombieDead;

    public ZombieBaseState(GameObject npc): base(npc)
    {
        zombie = npc.GetComponent<ZombieBehaviour>();
        enterStateTime = Time.time;
        initAnimSpeed = zombie.animator.GetFloat("Speed");
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Lerp between initial Speed and lerpTo over duration
    /// </summary>
    /// <param name="lerpTo"></param>
    /// <param name="duration"></param>
    /// <returns>Current lerp value</returns>
    protected float lerpAnimSpeed(float lerpTo, float duration)
    {
        //Debug.Log($"Time now: {Time.time}\nStart time: {enterStateTime}\nDuration: {duration}");
        float transitionRatio = (Time.time - enterStateTime) / duration;
        //Debug.Log($"TransitionRatio = {transitionRatio}");
        if (transitionRatio >= 1f)
        {
            return lerpTo;
        }
        return Mathf.Lerp(initAnimSpeed, lerpTo, transitionRatio);
    }

    protected Vector3 directionToPlayer()
    {
        return zombie.transform.position - new Vector3(zombie.player.transform.position.x, 0, zombie.player.transform.position.z);
    }

    protected void notifyNearbyZombies()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Hostile");
        foreach (GameObject otherZombie in zombies)
        {
            // Check the otherZombie is in range
            if ((zombie.transform.position - otherZombie.transform.position).magnitude < notifyNearbyDistance)
            {
                // Make sure its a different zombie
                ZombieBehaviour zb = otherZombie.GetComponent<ZombieBehaviour>();
                if (zb != null && zb != zombie)
                {
                    zb.Alert();
                }
            }
        }
    }

    protected bool LineOfSightCheck(GameObject other)
    {
        Vector3 directionToOther = other.transform.position - zombie.LineOfSightCheckPoint.position;
        Debug.DrawRay(zombie.LineOfSightCheckPoint.position, directionToOther, Color.cyan);
        RaycastHit hit;
        Ray los = new Ray(zombie.LineOfSightCheckPoint.position, directionToOther);
        if (Physics.Raycast(los, out hit))
        {
            //Debug.Log($"Hit name: {hit.transform.name}");
            //Debug.Log($"Other name: {other.transform.name}");
            if (hit.transform.name == other.transform.name) 
                return true;
        }
        return false;
    }

}


public class ZombieUnaware : ZombieBaseState
{
    private float detectRange;
    public ZombieUnaware(GameObject npc) : base(npc)
    {
        detectRange = zombie.DetectPlayerRange;
    }


    // Check for player every update, if detected transition to alerted state
    protected void CheckForPlayer()
    {
        Vector3 directionToTarget = directionToPlayer();
        float angle = Vector3.Angle(zombie.transform.forward, directionToTarget);
        float distance = directionToTarget.magnitude;
        //Debug.Log($"Distance: {distance}");
        if (distance < detectRange) // player in range
        {
            //Debug.Log("Player in range");
            if (Mathf.Abs(angle) > 80 && LineOfSightCheck(zombie.player))
            {
                //Debug.Log("Zombie should be alerted");
                zombie.Alert();
            }
        }
        //throw new System.NotImplementedException();
    }

    protected ZombieUnaware nextState()
    {
        float coin = Random.value;

        if (coin < 0.5f)
        {
            return new ZombieIdle(npc);
        }
        else
        {
            return new ZombieWander(npc);
        }
    }


    public override void UpdateState()
    {
        zombie.SetState(nextState());
    }
}

public class ZombieAttack : ZombieBaseState
{
    public ZombieAttack(GameObject npc) : base(npc)
    {

    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        zombie.agent.ResetPath();
        zombie.Attack();
    }

    public override void UpdateState()
    {
        // Orient towards player
        npc.transform.LookAt(zombie.player.transform);

        zombie.animator.SetFloat("Speed", lerpAnimSpeed(0f, 0.15f));


        float playerDistance = directionToPlayer().magnitude;
        if (playerDistance > zombie.attackRange)
        {
            zombie.EndAttack();
            zombie.SetState(new ZombiePursue(npc));
        }
    }

    private Quaternion pickDirection()
    {
        float orient = Random.Range(-360, 360);
        Quaternion direction = Quaternion.Euler(0, orient, 0);
        return direction;
    }

}

public class ZombiePursue : ZombieBaseState
{
    private float xOffset;
    private float zOffset;
    public ZombiePursue(GameObject npc) : base(npc)
    {
        xOffset = Random.Range(-(zombie.attackRange - 0.05f), (zombie.attackRange - 0.05f));
        zOffset = Random.Range(-(zombie.attackRange - 0.05f), (zombie.attackRange - 0.05f));
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        zombie.agent.ResetPath();
        zombie.agent.speed = 4f;
    }

    protected void MoveTowardPlayer()
    {
        Vector3 nearPlayer = zombie.player.transform.position;
        nearPlayer.x += xOffset;
        nearPlayer.z += zOffset;
        zombie.agent.SetDestination(nearPlayer);
    }

    public override void UpdateState()
    {
        zombie.animator.SetFloat("Speed", lerpAnimSpeed(1f, 0.25f));
        Debug.Log(directionToPlayer().magnitude);
        if (directionToPlayer().magnitude < zombie.attackRange)
        {
            zombie.SetState(new ZombieAttack(npc));
        }

        MoveTowardPlayer();

    }

}

public class ZombieAlerted : ZombieBaseState
{

    private float pursueDelay = 2.3f;

    public ZombieAlerted(GameObject npc) : base(npc)
    {

    }


    public override void OnStateEnter()
    {
        base.OnStateEnter();
        zombie.agent.ResetPath();
        zombie.animator.SetTrigger("Alerted");
        notifyNearbyZombies();
        //Debug.Log("ZOMBIE ALERTED!");
    }

    public override void OnStateLeave()
    {
        base.OnStateLeave();
        // Display red glowing eyes
        zombie.animator.ResetTrigger("Alerted");
    }





    public override void UpdateState()
    {
        zombie.animator.SetFloat("Speed", lerpAnimSpeed(0f, 0.25f));
        if (Time.time - enterStateTime > pursueDelay)
        {
            zombie.SetState(new ZombiePursue(npc));
        }
    }
}

public class ZombieIdle : ZombieUnaware
{

    private float idleDuration;


    public ZombieIdle(GameObject npc) : base(npc)
    {
        idleDuration = Random.Range(2f, 6f);
    }

    public override void UpdateState()
    {
        zombie.animator.SetFloat("Speed", lerpAnimSpeed(0f, 0.25f));
        if (Time.time - enterStateTime > idleDuration)
        {
            zombie.SetState(nextState());
        }
        CheckForPlayer();
    }


}

public class ZombieWander : ZombieUnaware
{
    // Wander aimlessly in a random direction for 2-6 seconds
    // Either repeat this state after timer with a new direction 
    // or go to idle

    private float wanderDistance;
    private Quaternion wanderDirection;
    private Vector3 walkModifier;

    private Vector3 destination;
    private Vector3 rawDestination;

    public ZombieWander(GameObject npc) : base(npc)
    {
        wanderDistance = Random.Range(2.0f, 8.0f);
        //wanderDirection = pickDirection();
        //walkModifier = new Vector3(zombie.MovementSpeed * 0.5f, zombie.MovementSpeed * 0.5f, zombie.MovementSpeed * 0.5f);
        rawDestination = Random.insideUnitSphere * wanderDistance;
        destination = new Vector3(npc.transform.position.x + rawDestination.x , npc.transform.position.y + rawDestination.y, npc.transform.position.z + rawDestination.z);
    }

    public override void UpdateState()
    {

        zombie.animator.SetFloat("Speed", lerpAnimSpeed(0.5f, 0.25f));

        // Wander time exceeded -> new wander or idle state
        if (zombie.agent.remainingDistance <= 0.05)
        {
            zombie.SetState(nextState());
        }

        // Check for player after calling nextSate() to prevent overwriting alerted state
        CheckForPlayer();
    }

    private void ApplyMove()
    {

        //float turnRatio = (Time.time - enterStateTime) / wanderDuration;
        //zombie.transform.rotation = Quaternion.Slerp(zombie.transform.rotation, wanderDirection, turnRatio);
        // check if about to collide with wall -> if so zombie.SetState(nextState())

        // apply force to zombie to move in wander direction
        //Vector3 movement = zombie.transform.forward;
        //movement.Scale(walkModifier);

        //zombie.zombieRigidbody.AddForce(movement);
  

    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        zombie.agent.speed = 1f;
        zombie.agent.SetDestination(destination);
    }

    private Quaternion pickDirection()
    {
        float orient = Random.Range(-360, 360);
        Quaternion direction = Quaternion.Euler(0, orient, 0);
        return direction;
    }

}

public class ZombieDead : ZombieBaseState
{
    float deathTime;
    float sinkTime = 3.5f;
    public ZombieDead(GameObject npc) : base(npc)
    {
        deathTime = Time.time;
    }


    public override void UpdateState()
    {
        float transitionRatio = Time.time - deathTime / sinkTime;

        // Sink zombie into floor
        //Vector3 nextPosition = Vector3.Lerp(zombie.transform.position, Vector3.down, transitionRatio);
        //zombie.transform.position = nextPosition;

        if (Time.time - deathTime >= 3f)
        {
            zombie.DestroyGameobject(1f);
        }
    }


    public override void OnStateEnter()
    {
        base.OnStateEnter();
        zombie.agent.ResetPath();
        zombie.animator.SetTrigger("Death");
        zombie.animator.SetBool("IsDead", true);
        //Debug.Log("Enter death state");
        //zombie.animator.ResetTrigger("IsDead");
        zombie.animator.SetFloat("Speed", 0);
    }



}




