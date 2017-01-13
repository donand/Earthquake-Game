﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserFeedback : MonoBehaviour {

    private Text text;
    public float visibleTime = 5;

    private bool isVisible = false;

    private float startTime = float.MinValue;

	// Use this for initialization
	void Awake () {
        text = GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if (isVisible) {                
            if (Time.time > startTime + visibleTime) {
                gameObject.SetActive(false);
                isVisible = false;
            }
        }
	}

    public void setText(string text) {
        gameObject.SetActive(true);
        startTime = Time.time;
        this.text.text = text;

        isVisible = true;
    }
}