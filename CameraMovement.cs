using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour {
    public float speed;
    public FixedJoystick fixedJoystick;
    public float maxX = 10f;
    public float minX = -10f;
    public float maxZ = 10f;
    public float minZ = -10f;

    public Slider heightSlider;
    public float minHeight = 2f;
    public float maxHeight = 10f;

    public bool isCameraMoving;
    public Camera mainCamera;

    private void Start () {
        heightSlider.onValueChanged.AddListener(OnHeightSliderValueChanged);
        mainCamera = Camera.main;
    }

    public void FixedUpdate () {
        Vector3 direction = Vector3.forward * fixedJoystick.Vertical + Vector3.right * fixedJoystick.Horizontal;
        Vector3 velocity = direction * speed * Time.fixedDeltaTime;
        Vector3 newPosition = transform.position + velocity;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        transform.position = newPosition;
    }

    private void OnHeightSliderValueChanged (float value) {
        float newHeight = Mathf.Lerp(minHeight, maxHeight, value);

        Vector3 newPosition = new Vector3(mainCamera.transform.position.x, value, mainCamera.transform.position.z);
        mainCamera.transform.position = newPosition;
    }
}
