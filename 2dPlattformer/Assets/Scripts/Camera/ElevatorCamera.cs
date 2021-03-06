﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCamera : MonoBehaviour
{
    public CameraFollow cam;
    private CameraFollow originalCamera;
    //private Vector3 Position;
    public GameObject lazer;
    public Vector3 lazerStartPosition;
    //public GameObject exitLazer;
    //public int killcount=1;
    public int arenaCameraSize = 10;
    public float sizeSpeed = 0.1f;
    public float cameraPositionSpeed = 0.5f;
    public float startPositionY;

    public GameObject startPosition;
    public GameObject endPosition;
    public GameObject lazerEndPosition;
    public Vector3 cameraPosition;

    public bool hasEnterd = false;

    public LevelManager levelManager;

    //private bool lerpFinnished = false;

    private void Start()
    {
        lazerStartPosition = lazer.transform.position;
        lazer.SetActive(false);
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void Update()
    {
        //if (lerpFinnished)
        //{
        //    return;
        //}
        if (cam != null)
        {
            if (cam.enterdArena == true && hasEnterd)
            //if(hasEnterd)
            {

                cameraPosition.z = -10;
                //else
                //{
                //   lerpFinnished = true;
                //}

                //Debug.Log(positionY);
                //cameraPosition = new Vector3(cameraPosition.x,cameraPosition.y+positionY,cameraPosition.z);

                cam._targetPosition = cameraPosition;//startPosition.transform.position;

            }
        }
        if (hasEnterd)
        {
            if (cameraPosition.y < endPosition.transform.position.y)
            {
                //positionY = positionY + cameraPositionSpeed;
                cameraPosition += Vector3.up * cameraPositionSpeed * Time.deltaTime;


            }
            if (lazer.transform.position.y < lazerEndPosition.transform.position.y)
            {
                lazer.transform.Translate(Vector3.up * cameraPositionSpeed * Time.deltaTime);
            }
        }
    }

    public void ResetArena()
    {
        hasEnterd = false;
        lazer.transform.position = new Vector3(lazerStartPosition.x, lazerStartPosition.y, lazerStartPosition.z);
        lazer.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Debug.Log("EnterElevator");
            if (!hasEnterd)
            {
                cameraPosition = startPosition.transform.position;
                cameraPosition.y += startPositionY;

            }
            cameraPosition.z = -10;

            hasEnterd = true;
            if (cam != null)
            {
                cam.enterdArena = true;
                cam.arenaCameraSize = arenaCameraSize;
                cam.sizeSpeed = sizeSpeed;
                //originalCamera = cam;
            }

            lazer.SetActive(true);
            levelManager.ResetEnemies();
        }

    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !cam.enterdArena)
        {
            //cameraPosition = startPosition.transform.position;
            //positionY = cameraPosition.y;
            //cam._targetPosition = startPosition.transform.position;//transform.Find("CameraPosition").position;

            //cam.orthographicSize = cameraSize;

            // Spelaren har triggat cameratrigger
            //cam._targetPosition = new Vector3(cam._targetPosition.x, cam._targetPosition.y, camDepth);
        }
        //if(killcount==0){
           //exitLazer.SetActive(false);
        //}
    }*/

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Debug.Log("exit");
            // Spelaren har triggat cameratrigger
            if (cam != null)
            {
                cam.enterdArena = false;
            }
            //cam._targetPosition = new Vector3(cam._targetPosition.x, cam._targetPosition.y, cam._targetPosition.z);
            //positionY = 0;
        }
    }
}