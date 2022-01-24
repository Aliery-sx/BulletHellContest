using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public GameObject player;
    
    [Header("���������")]
    public float easing = 0.01f;
    public Vector2 minY = Vector2.zero;
    
    [Header("������������ ��������")]
    public float camX;
    public float camZ;
    public Vector3 offset;
    
    void Awake()
    {
        camX = this.transform.position.x;
        camZ = this.transform.position.z;
        offset = transform.position - player.transform.position;
    }

    
    void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 destination = player.transform.position;
            destination = Vector3.Lerp(transform.position, destination + offset, easing);
            transform.position = destination;
        }
    }
}
