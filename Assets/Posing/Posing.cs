using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
/// <summary>
/// ��ġ, ȸ������ ����.
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
    /// curTransform�� ���ԵǴ� �ܺΰ��� ���� update�� �����.
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
