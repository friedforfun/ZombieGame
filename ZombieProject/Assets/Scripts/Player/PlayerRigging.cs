using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerRigging : MonoBehaviour
{
    [SerializeField] private Rig HeadRig;
    [SerializeField] private GameObject LookTarget;

    [SerializeField] private Rig LArmRig;
    [SerializeField] private Rig RArmRig;
    [SerializeField] private GameObject LeftArmTarget;
    [SerializeField] private GameObject RightArmTarget;

    private float rigTransitionSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHeadRigWeight(float weight)
    {
        HeadRig.weight = weight;
    }

    public void SetLeftArmRigWeight(float weight)
    {
        LArmRig.weight = weight;
    }

    public void SetRightArmRigWeight(float weight)
    {
        RArmRig.weight = weight;
    }

    public void lookAtTarget(Vector3 target)
    {
        LookTarget.transform.position = target;
    }

    public void SetLeftHandTarget(Vector3 target)
    {
        LeftArmTarget.transform.position = target;
    }
    public void SetRightHandTarget(Vector3 target)
    {
        RightArmTarget.transform.position = target;
    }

}
