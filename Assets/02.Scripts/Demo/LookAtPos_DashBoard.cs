using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LookAtPos_DashBoard : MonoBehaviour
{
    public Text txt;
    public Transform rot1;
    public Transform rot2;
    public Transform rot3;
    public Transform rot4;


    // Update is called once per frame
    void Update()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"pos1: {rot1.position}");
        sb.AppendLine($"rot1: {rot1.rotation.eulerAngles}\n");
        sb.AppendLine($"pos2: {rot2.position}");
        sb.AppendLine($"rot2: {rot2.rotation.eulerAngles}\n");
        sb.AppendLine($"pos3: {rot3.position}");
        sb.AppendLine($"rot3: {rot3.rotation.eulerAngles}\n");
        sb.AppendLine($"pos4: {rot4.position}");
        sb.AppendLine($"rot4: {rot4.rotation.eulerAngles}\n");
        txt.text = sb.ToString();
    }
}
