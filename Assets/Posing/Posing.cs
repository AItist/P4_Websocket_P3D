using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
/// <summary>
/// 위치, 회전값을 변경.
/// </summary>
public class Posing : MonoBehaviour
{
    public Transform curTransform;
    public int index;

    // Start is called before the first frame update
    private void Awake()
    {
        SetTransform();
    }

    private void Update()
    {
      //  SetCurrentTransform();
    }

    public void SetTransform()
    {
        curTransform = GetComponent<Transform>();

    }
    /// <summary>
    /// curTransform에 대입되는 외부값에 따라 update로 적용됨.
    /// </summary>
    public void SetCurrentTransform()
    {
        SetTransform();
        gameObject.transform.localPosition = curTransform.localPosition;
        gameObject.transform.rotation = curTransform.rotation;
    }
   
    public void SetCurTransform()
    {
        gameObject.transform.position = curTransform.position;
        gameObject.transform.rotation = curTransform.rotation;
    }
  
}
