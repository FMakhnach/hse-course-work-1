﻿using UnityEngine;

public class Spawn : MonoBehaviour
{
    private Tile[] path;
    /// <summary>
    /// For now it just contains the path.
    /// </summary>
    public Tile[] Path
    {
        get => path;
        set
        {
            if (path != null)
            {
                Debug.LogError("Trying to rewrite path wtf");
            }
            else
            {
                path = value;
            }
        }
    }
}
