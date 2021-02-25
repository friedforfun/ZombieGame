using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private Manager gameManager;
    private LayerMask playerLayer = 9;
    private bool hasFired = false;

    private void Start()
    {
        gameManager = FindObjectOfType<Manager>();
        if (gameManager is null)
            throw new UnassignedReferenceException();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            if (!hasFired)
            {
                gameManager.WinGame();
            }

            hasFired = true;
        }
    }
}
