using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float border = 0.25f;

    Camera camera;

    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        Vector2 topLeft = camera.ViewportToWorldPoint(new Vector3(border, border, 1f));
        Vector2 bottomRight = camera.ViewportToWorldPoint(new Vector3(1f - border, 1f - border, 1f));
        Vector2 pos = transform.position;
        Vector2 playerPos = PlayerController.Instance.transform.position;
        if (playerPos.x < topLeft.x)
            transform.position += new Vector3(playerPos.x - topLeft.x, 0f, 0f);
        else if (playerPos.x > bottomRight.x)
            transform.position += new Vector3(playerPos.x - bottomRight.x, 0f, 0f);
        if (playerPos.y < topLeft.y)
            transform.position += new Vector3(0f, playerPos.y - topLeft.y, 0f);
        else if (playerPos.y > bottomRight.y)
            transform.position += new Vector3(0f, playerPos.y - bottomRight.y, 0f);
    }
}
