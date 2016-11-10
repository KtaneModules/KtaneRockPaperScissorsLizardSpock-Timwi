using System;
using System.Collections.Generic;
using System.Linq;
using RockPaperScissorsLizardSpock;
using UnityEngine;
using Rnd = UnityEngine.Random;

/// <summary>
/// On the Subject of Rock-Paper-Scissors-Lizard-Spock
/// Created by Timwi
/// </summary>
public class RockPaperScissorsLizardSpockModule : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;
    public KMSelectable[] Selectables;

    void Start()
    {
        Debug.Log("[Rock-Paper-Scissors-Lizard-Spock] Started");
        Module.OnActivate += ActivateModule;
    }

    void ActivateModule()
    {
        Debug.Log("[Rock-Paper-Scissors-Lizard-Spock] Activated");
    }
}
