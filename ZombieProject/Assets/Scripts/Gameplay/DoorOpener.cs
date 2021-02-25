using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private string EventIdentifier;

    void OnEnable()
    {
        EventManager.StartListening(EventIdentifier, OpenDoor);
    }

    void OnDisable()
    {
        EventManager.StopListening(EventIdentifier, OpenDoor);
    }

    private void OpenDoor()
    {
        LeanTween.moveLocalY(gameObject, 31f, 25f).setEaseOutQuad();
    }

}
