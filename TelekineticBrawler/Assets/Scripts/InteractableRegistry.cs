using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableRegistry : MonoBehaviour
{

    public static InteractableRegistry Instance { get; private set; }

    private readonly List<Interactable> activeInteractables = new();

    void Awake()
    {
        Instance = this;
    }

    public void Register(Interactable Interactable)
    {

        activeInteractables.Add(Interactable);
        for (int i = 0; i < activeInteractables.Count; i++)
        {

            Debug.Log($"index: {i}, {activeInteractables[i]}");

        }
            
    }

    public void Unregister(Interactable Interactable)
    {
        activeInteractables.Remove(Interactable);
    }


    public IReadOnlyList<Interactable> ActiveInteractables => activeInteractables;
}