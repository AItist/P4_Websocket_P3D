using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;

public class AnimationCode : MonoBehaviour
{
    public UDPReceive udpReceive;
    public GameObject[] Body;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string data = udpReceive.data;
        string[] points = data.Split(',');

        for (int i = 0; i <= 32; i++)
        {
            float x = float.Parse(points[0 + (i * 3)]) / 100;
            float y = float.Parse(points[1 + (i * 3)]) / 100;
            float z = float.Parse(points[2 + (i * 3)]) / 300;
            Body[i].transform.localPosition = new Vector3(x, y, z);
        }


        Thread.Sleep(30);
    }
}   