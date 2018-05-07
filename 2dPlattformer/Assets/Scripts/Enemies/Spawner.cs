﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject ReferensEnemy;

    public GameObject spawnArea;
    //public GameObject patrolPoint;

    public Transform[] patrolPoints;

    //public DronePatrolState dronePatrol;

    private bool isInArena;
    public float timer;
    public float startTimer;

    public LevelManager _levelManager;

    // Use this for initialization
    void Start () {
        timer = startTimer;

        _levelManager = FindObjectOfType<LevelManager>();
        //patrolPoints[0] = transform.Find("Patrol Point 7");//funkar också
    }
	
	// Update is called once per frame
	void Update () {

        if (isInArena)
        {
            timer -= Time.deltaTime;

            Spawn();
        }
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.gameObject.tag == "Player")
        {
            isInArena = true;         

        }

    }

    private void Spawn()
    {
        if(timer <= 0){ 
            //dronePatrol = ReferensEnemy.GetComponent<DronePatrolState>();
            //ReferensEnemy.GetComponent<DronePatrolState>().patrolPoints[0] = patrolPoints[0];
            //ReferensEnemy.GetComponent<DronePatrolState>().patrolPoints[1] = patrolPoints[1];
            ReferensEnemy.GetComponent<DronePatrolState>().patrolPoints = patrolPoints;
            ReferensEnemy.transform.position = spawnArea.transform.position;
            //ReferensEnemy.pat
            GameObject enemy = Instantiate(ReferensEnemy);
            AddToList(enemy);
            timer = startTimer;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInArena = false;

        }
    }

    private void AddToList(GameObject referensEnemy)
    {
        _levelManager._enemies.Add(referensEnemy);
    }

    public void DestroySpawnedEnemies (GameObject enemy)
    {
        Destroy(enemy);
    }
}
