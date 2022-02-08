using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnim : MonoBehaviour
{
    public Image Loading_Img;

    public float speed;

    void Update()
    {
        //Loading_Img.GetComponent<RectTransform>().rotation += Quaternion.Euler(0, 0, speed);
        Loading_Img.GetComponent<RectTransform>().RotateAround(transform.position, Vector3.forward, speed * Time.deltaTime);
    }
}
