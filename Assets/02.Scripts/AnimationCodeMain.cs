using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using UnityEngine.UIElements;

public class AnimationCodeMain : MonoBehaviour
{
    public int camIndex;
    public Management.MainManager manager;
    //public UDPReceive udpReceive;

    public Transform originCamera;
    public List<Vector3> dest_points;
    
    public GameObject[] Body;

    // Start is called before the first frame update
    void Start()
    {
        dest_points = new List<Vector3>();
        dest_points.AddRange(new Vector3[33]);
    }

    // Update is called once per frame
    void Update()
    {
        //string data = udpReceive.data;
        if (!manager.ImgDics.ContainsKey(camIndex))
        {
            return;
        }

        string data = manager.ImgDics[camIndex].poseframe.ToString();
        string[] _points = data.Split(',');

        for (int i = 0; i <= 32; i++)
        {
            float x = float.Parse(_points[0 + (i * 3)]) / 100;
            float y = float.Parse(_points[1 + (i * 3)]) / 100;
            float z = float.Parse(_points[2 + (i * 3)]) / 300;
            //Body[i].transform.localPosition = new Vector3(x, y, z);
            dest_points[i] = new Vector3(x, y, z);
        }


        Thread.Sleep(30);
    }
}   