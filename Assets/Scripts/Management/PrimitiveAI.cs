﻿using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Linq;
using UnityEngine;

/// <summary>
/// My little AI.
/// Basically, what it does is spawning random game object 
/// each iteration (which is, for now, random value from 1 to 3 secs) if 
/// 1) it has enough money;
/// 2) it is possible (eg no place for turret).
/// Chances can be set manually, they are normalized automatically btw.
/// But its nice if you will make it valid from start.
/// </summary>
public class PrimitiveAI : PlayerManager
{
    #region Turret stuff.
    [SerializeField]
    private TurretPlacement[] turretPlacements;
    [SerializeField]
    private int numberOfStartTurrets;
    private Turret[] turrets;
    [SerializeField]
    [Range(0, 1)]
    private float laserProbability;
    [SerializeField]
    [Range(0, 1)]
    private float mgProbability;
    #endregion
    #region Unit stuff.
    [SerializeField]
    private Spawn[] spawns;
    [SerializeField]
    [Range(0, 1)]
    private float buggyProbability;
    [SerializeField]
    [Range(0, 1)]
    private float copterProbability;
    #endregion
    #region Buildings stuff.
    [SerializeField]
    private PlantPlacement[] plantPlacements;
    private Plant[] plants;
    [SerializeField]
    private int numberOfStartPlants;
    [SerializeField]
    [Range(0, 1)]
    private float plantProbability;
    #endregion

    private Action currentAction;
    private System.Random sysRand;

    protected override void Awake()
    {
        sysRand = new System.Random();
        turrets = new Turret[turretPlacements.Length];
        plants = new Plant[plantPlacements.Length];
        numberOfStartTurrets = Math.Min(numberOfStartTurrets, turretPlacements.Length);
        numberOfStartPlants = Math.Min(numberOfStartPlants, plantPlacements.Length);

        // Normalizing probabilities (so they give 1 in sum).
        float modifier = 1 / (buggyProbability + copterProbability + laserProbability + mgProbability + plantProbability);
        buggyProbability *= modifier;
        copterProbability *= modifier;
        laserProbability *= modifier;
        mgProbability *= modifier;
        plantProbability *= modifier;

        Energy = 10000f;
        PlayerStats = new Stats();
        GenerateStartTurrets();
        GenerateStartPlants();
        Energy = startingEnergy;
    }
    protected override void Start()
    {
        base.Start();
        LevelManager.Instance.onGameStarted +=
            () => StartCoroutine("RandomAction");
    }
    /// <summary>
    /// Does random action every [1, 2] seconds.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RandomAction()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1f + UnityEngine.Random.value);
            if (currentAction == null)
            {
                currentAction = GetRandomAction();
            }
            currentAction();
        }
    }

    private void GenerateStartTurrets()
    {
        var startTurretPositions = Enumerable.Range(0, turretPlacements.Length).OrderBy(x => sysRand.Next()).ToArray();
        for (int i = 0; i < numberOfStartTurrets; i++)
        {
            int id = startTurretPositions[i];
            if (sysRand.Next(2) == 0)
            {
                turrets[id] = PlaceLaserTurret(turretPlacements[id]);
            }
            else
            {
                turrets[id] = PlaceMGTurret(turretPlacements[id]);
            }
        }
    }
    private void GenerateStartPlants()
    {
        for (int i = 0; i < numberOfStartPlants; i++)
        {
            plants[i] = PlacePlant(plantPlacements[i]);
        }
    }
    private Action GetRandomAction()
    {
        float chance = UnityEngine.Random.value;
        if (chance < buggyProbability)
        {
            return SpawnBuggy;
        }
        chance -= buggyProbability;
        if (chance < copterProbability)
        {
            return SpawnCopter;
        }
        chance -= copterProbability;
        if (chance < laserProbability)
        {
            return PlaceLaser;
        }
        chance -= laserProbability;
        if (chance < mgProbability)
        {
            return PlaceMG;
        }
        chance -= mgProbability;
        if (chance < plantProbability)
        {
            return PlacePlant;
        }
        return null;
    }
    private void SpawnBuggy()
    {
        if (Energy < Cost.Buggy)
        {
            return;
        }
        int id = (int)(UnityEngine.Random.value * spawns.Length);
        if (id == spawns.Length)
        {
            id--;
        }
        SpawnBuggy(spawns[id]);
        currentAction = null;
    }
    private void SpawnCopter()
    {
        if (Energy < Cost.Copter)
        {
            return;
        }
        int id = (int)(UnityEngine.Random.value * spawns.Length);
        if (id == spawns.Length)
        {
            id--;
        }
        SpawnCopter(spawns[id]);
        currentAction = null;
    }
    private void PlaceLaser()
    {
        if (Energy < Cost.LaserTurret)
        {
            return;
        }
        for (int i = 0; i < turrets.Length; i++)
        {
            if (turrets[i] == null || turrets[i].ToString() == "null")
            {
                turrets[i] = PlaceLaserTurret(turretPlacements[i]);
                currentAction = null;
                return;
            }
        }
        currentAction = null;
    }
    private void PlaceMG()
    {
        if (Energy < Cost.MachineGunTurret)
        {
            return;
        }
        for (int i = 0; i < turrets.Length; i++)
        {
            if (turrets[i] == null || turrets[i].ToString() == "null")
            {
                turrets[i] = PlaceMGTurret(turretPlacements[i]);
                currentAction = null;
                return;
            }
        }
        currentAction = null;
    }
    private void PlacePlant()
    {
        if (Energy < Cost.Plant)
        {
            return;
        }
        for (int i = 0; i < plants.Length; i++)
        {
            if (plants[i] == null || plants[i].ToString() == "null")
            {
                plants[i] = PlacePlant(plantPlacements[i]);
                currentAction = null;
                return;
            }
        }
        currentAction = null;
    }
}
