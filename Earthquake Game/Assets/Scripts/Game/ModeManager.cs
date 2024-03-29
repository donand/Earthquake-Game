﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModeManager : MonoBehaviour {

    public enum GameMode { Building, Upgrade, Simulation, Pass, Fail };
    public List<GameObject> buildingObjects, upgradeObjects, simulationObjects, passObjects, failObjects;

    GameObject buildingContainer;

    public TargetController targetController;
    public EarthquakeSimulator earthquakeSimulator;
    public BuildingList buildingList;
    public UpgradeList upgradeList;

    public UserFeedback userFeedback;

    public UnityEvent onPass, onFail;

    private Solutions solutions;

    public int finishWaitTime = 15;

    private GameMode mode;
    public GameMode Mode {
        get { return mode; }
        set {
            setGameMode(value);
            mode = value;
        }
    }

    void Start() {
        Mode = GameMode.Building;
        buildingContainer = GameObject.Find("BuildingContainer");
        solutions = GetComponent<Solutions>();

        targetController.gameObject.SetActive(false);
    }

    // The idea is to put behaviours specific to a mode in a separate object, e.g. the wave simulator,
    // so that we don't need exceptions for every mode. The script starts in its Start method
    public void setGameMode(GameMode newMode) {
        Debug.Log(mode + " -> " + newMode);
        // Exceptions
        if (mode == GameMode.Building && newMode == GameMode.Upgrade) {
            if (!buildingList.finishedPlacing()) {
                userFeedback.setText("You should place all buildings before continuing");
                return;
            }
        } else if ((mode == GameMode.Building || mode == GameMode.Upgrade) && newMode == GameMode.Simulation) {
            // Todo: Check if all buildings have been set
            if (!buildingList.finishedPlacing()) {
                userFeedback.setText("You should place all buildings before continuing");
                return;
            } else if (!upgradeList.finishedPlacing()) {
                userFeedback.setText("You should place all upgrades before continuing");
                return;
            }

            // Disable all undo buttons
            GameObject[] platforms = GameObject.FindGameObjectsWithTag("BuildingPlatform");
            foreach (GameObject go in platforms) {
                Button b = go.GetComponentInChildren<Button>();
                if (b != null)
                    b.gameObject.SetActive(false);
            }
                

            earthquakeSimulator.simulateEarthquake();

            // Check if passed
            bool passed = true;
            foreach (GameObject go in platforms) {
                BuildingPlatformController bpc = go.GetComponent<BuildingPlatformController>();
                if (bpc.building != null && bpc.building.type == Building.BuildingType.Thematic)
                    continue;
                else if (!bpc.isCorrect()) {
                    Debug.Log(bpc.building);
                    passed = false;
                    break;
                }
            }

            if (passed) {
                StartCoroutine(setGameMode(GameMode.Pass, finishWaitTime));
            } else {
                StartCoroutine(setGameMode(GameMode.Fail, finishWaitTime));
            }
        } else if (mode == GameMode.Upgrade && newMode == GameMode.Building) {
            // Todo: Remove/reset upgrades when going back?

        } else if (mode == GameMode.Upgrade && newMode == GameMode.Simulation) {
            // Check if all foundations have been set
            if (!upgradeList.finishedPlacing()) {
                userFeedback.setText("Place all upgrades before continuing");
                return;
            }

        } else if (newMode == GameMode.Pass) {
            onPass.Invoke();
        } else if (newMode == GameMode.Fail) {
             onFail.Invoke();
        }

        // In every case, switch all objects
        List<GameObject> oldObjs = getModeObjects(mode);
        List<GameObject> newObjs = getModeObjects(newMode);

        foreach (GameObject go in oldObjs) {
            go.SetActive(false);
        }
        foreach (GameObject go in newObjs) {
            go.SetActive(true);
        }

        mode = newMode;

        // Stop wave propogation when modes change
        targetController.stopWaves();
    }

    IEnumerator setGameMode(GameMode newMode, int seconds) {
        yield return new WaitForSeconds(seconds);
        setGameMode(newMode);
    }

    // Function with string so you can do it via button click
    public void setGameMode(string newMode) {
        bool match = false;
        foreach (GameMode val in GameMode.GetValues(typeof(GameMode))) {
            if (newMode.Equals(val.ToString())) {
                setGameMode(val);
                match = true;
                break;
            }
        }
        if (!match)
            Debug.Log("Mode not recognized: " + newMode);
    }

    private List<GameObject> getModeObjects(GameMode gm) {
        switch (gm) {
            case GameMode.Building:
                return buildingObjects;
            case GameMode.Upgrade:
                return upgradeObjects;
            case GameMode.Simulation:
                return simulationObjects;
            case GameMode.Pass:
                return passObjects;
            case GameMode.Fail:
                return failObjects;
        }
        return null;
    }
}
