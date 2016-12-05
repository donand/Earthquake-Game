﻿using UnityEngine;
using System.Collections;

public class CamMovement : MonoBehaviour {

    // Positioning/mouse stuff
    private Vector3 previousMousePosition;
    private bool movedSinceMouseDown = false;
    public float keyMoveSpeed = 1f;

    // Zoom stuff
    public float minScale = 0.1f;
    public float maxScale = 10f;
    public float scrollSpeed = 0.1f;
    // Smooth zoom stuff
    private float zoomTarget;
    public float zoomSmoothness = 0.1f;
    private float zoomVelocity;

    // Use this for initialization
    void Start () {
        previousMousePosition = Input.mousePosition;
        zoomTarget = Camera.main.orthographicSize;
	}

    // Update is called once per frame
    void Update () {

        // Reset mouse position after clicking when coming outside of the screen, else it jumps when panning the first time
        if (Input.GetMouseButtonDown(0) && (previousMousePosition.x < 0 || previousMousePosition.x > Screen.width || previousMousePosition.y < 0 || previousMousePosition.y > Screen.height))
            previousMousePosition = Input.mousePosition;

        // Zoom
        if (Input.mouseScrollDelta.y != 0) {
            float scale = Camera.main.orthographicSize - Input.mouseScrollDelta.y * scrollSpeed;
            zoomTarget = Mathf.Clamp(scale, minScale, maxScale);
        }
        // Camera.main.orthographicSize = Mathf.Lerp(zoomSource, zoomTarget, dZoom);
        Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, zoomTarget, ref zoomVelocity, zoomSmoothness);
        

        // Pan using mouse
        Vector3 pos = transform.position;
        if (Input.GetMouseButton(0)) {
            Vector3 dMouse = previousMousePosition - Input.mousePosition;
            dMouse.x *= Camera.main.orthographicSize * 2 * Camera.main.aspect / Screen.width;
            dMouse.y *= Camera.main.orthographicSize * 2 / Screen.height;
            pos += dMouse;
            
        }
        // Pan using keys
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            pos.x -= Time.deltaTime * keyMoveSpeed;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            pos.x += Time.deltaTime * keyMoveSpeed;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            pos.y -= Time.deltaTime * keyMoveSpeed;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            pos.y += Time.deltaTime * keyMoveSpeed;
        transform.position = pos;


        previousMousePosition = Input.mousePosition;
    }
}