using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KillCounter : MonoBehaviour
{
    [SerializeField] private Text killCounter;

    private Manager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<Manager>();
        if (gameManager is null)
            throw new UnassignedReferenceException();
    }

    private void Update()
    {
        SetText(gameManager.GetKills());
    }


    private void SetText(int kills)
    {
        killCounter.text = $"Kills: {kills}";
    }


}
