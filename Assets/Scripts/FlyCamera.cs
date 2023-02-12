using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float speed = 10.0f;
    public float mouseSensitivity = 100.0f;

    private float rotX = 0.0f;
    private float rotY = 0.0f;

    private Vector3 initPos;
    private Quaternion initRot;

    private void Start()
    {
        initPos = transform.position;
        initRot = transform.rotation;

        rotX = transform.eulerAngles.y;
        rotY = transform.eulerAngles.x;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.position += horizontal * speed * Time.deltaTime * transform.right;
        transform.position += speed * Time.deltaTime * vertical * transform.forward;

        if (Input.GetKey(KeyCode.E))
        {
            transform.position += speed * Time.deltaTime * transform.up;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= speed * Time.deltaTime * transform.up;
        }

        if (Input.GetMouseButton(1))
        {
            rotX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            rotY -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            rotY = Mathf.Clamp(rotY, -90.0f, 90.0f);

            transform.localRotation = Quaternion.Euler(rotY, rotX, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCamera();
        }
    }

    void ResetCamera()
    {
        transform.position = initPos;
        transform.rotation = initRot;
        rotX = transform.eulerAngles.y;
        rotY = transform.eulerAngles.x;
    }
}
