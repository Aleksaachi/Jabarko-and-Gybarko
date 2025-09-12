using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrderMenu : MonoBehaviour
{
    public GameObject menu;

    public float activationTime;
    float time = 0;
    bool followMouse = true;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            time += Time.deltaTime;

            if (time >= activationTime)
            {
                if (followMouse)
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    menu.transform.position = new(mousePosition.x, mousePosition.y, 0);
                    followMouse = false;
                }

                menu.SetActive(true);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            time = 0;
            menu.SetActive(false);
            followMouse = true;
        }
    }
}
