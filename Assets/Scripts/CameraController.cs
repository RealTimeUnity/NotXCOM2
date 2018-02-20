using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private float cameraMoveSpeed;
    [SerializeField]
    private float cameraRotationSpeed;
    [SerializeField]
    private float cameraZoomSpeed;

    public void FixedUpdate()
    {
        Vector3 forward = Vector3.Cross(Vector3.up, transform.right);

        //if(mousePosition.x > 0.95f)
        //{
        //    transform.position += transform.right * cameraMoveSpeed * Time.deltaTime;
        //}
        //if (mousePosition.x < 0.05f)
        //{
        //    transform.position -= transform.right * cameraMoveSpeed * Time.deltaTime;
        //}
        //if (mousePosition.y > 0.95f)
        //{
        //    transform.position -= forward * cameraMoveSpeed * Time.deltaTime;
        //}
        //if (mousePosition.y < 0.05f)
        //{
        //    transform.position += forward * cameraMoveSpeed * Time.deltaTime;
        //}

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.position += transform.right * cameraMoveSpeed * Time.deltaTime * x;
        transform.position += forward * cameraMoveSpeed * Time.deltaTime * -y;

        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(Vector3.up * -cameraRotationSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(Vector3.up * cameraRotationSpeed * Time.deltaTime, Space.World);
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            transform.position += transform.forward * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * cameraZoomSpeed;
        }
    }
}
