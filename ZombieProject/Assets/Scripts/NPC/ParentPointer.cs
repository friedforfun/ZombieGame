using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentPointer : MonoBehaviour
{
    [SerializeField] private GameObject parent;

    public GameObject Parent()
    {
        return parent;
    }
}
