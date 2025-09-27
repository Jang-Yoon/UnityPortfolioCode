using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 타일에 타워가 건설되어 있는지 여부를 알아야 한다.
public class TileGround : MonoBehaviour
{
    //[SerializeField]private bool isBuilted;

    //public bool IsBuilted{ get { return isBuilted; } set { isBuilted = value; } }

    // 자동 구현 프로퍼티 : 프로퍼티 get, set기본 형을 때 코드의 길이를 줄이기 위해 사용.
    // 변수를 선언할 필요 없다.
    public bool IsBuilted { get; set; }
}
