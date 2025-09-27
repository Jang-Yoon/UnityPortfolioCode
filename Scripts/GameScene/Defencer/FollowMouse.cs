using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{


    private void Update()
    {
        // 게임오브젝트가 마우스의 포지션을 (스크린 -> 월드 좌표로 변환) 따라다니도록 한다.
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePosition.z = 0;

        transform.position = mousePosition;

    }
}
