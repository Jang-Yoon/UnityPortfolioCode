using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class Detector : MonoBehaviourPunCallbacks
{
    // 타워 건설을 담당하는 TowerBuilder 필요.
    private TowerBuilder towerBuilder;

    // 광선과 충돌한 오브젝트의 정보를 저장할 변수
    private RaycastHit hit;
    private Vector3 direction;

    // 마우스 클릭으로 선택된 트랜스폼
    private Transform hitTransform;
    private TowerTooltip towerTooltip;
    private Transform towerGroup;

    // Start is called before the first frame update
    void Start()
    {
        towerTooltip = GetComponent<TowerTooltip>();
        // 게임매니저에 Detector를 할당하려는 설계구나...
        towerBuilder = GetComponent<TowerBuilder>();
    }

    // Update is called once per frame
    void Update()
    {
        // UI를 클릭했을 때 동작하지 않도록 한다.
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // 0 좌클릭, 1 우클릭
        if (Input.GetMouseButtonDown(0) && !Config.isAttacker)
        {
            // 레이캐스트에 사용할 광선을 정의
            // 메인카메라로부터 마우스 클릭한 곳을 향하는 광선
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Physics.Raycast(광선, out 정보 저장 변수, 사거리)
            // 리턴 타입 : bool -> if문으로 사용 가능
            // 충돌 성공하면 true
            // 3D광선
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hitTransform = hit.transform;

                direction = (hit.transform.position - Camera.main.transform.position);
                DebugRay();
                //Debug.Log("광선과 충돌한 곳 : " + hit.transform.name);

                // 여러 충돌체 있을 수 있으므로 태그 검사
                if (hit.collider.CompareTag("TileGround"))
                {
                    // 타워 건설
                    photonView.RPC("BuildTower", RpcTarget.All, hit.transform.position);
                    //towerBuilder.BuildTower(hit.transform);
                }
                if(hit.collider.CompareTag("Tower"))
                {
                    Tower tower = hit.transform.GetComponent<Tower>();
                    photonView.RPC("TooltipOn", RpcTarget.All, tower.Index);
                    //towerTooltip.TooltipOn(hit.transform);
                }


            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (hitTransform == null || !hitTransform.CompareTag("Tower"))
            {
                towerTooltip.TooltipOff();
            }
            hitTransform = null;
        }
    }

    // 레이캐스트 디버깅 용 함수
    private void DebugRay()
    {
        // Debug.DrawRay(Camera.main.transform.position, direction, Color.red, 1f);

        Debug.DrawLine(Camera.main.transform.position, hit.transform.position, Color.red, 1f);
    }
}


// RayCast : 광선 쏘아서 [충돌]한 오브젝트를 감지하는 것
// 눈에 안 보인다.
// 물리기반이다. => 반드시 충돌하려는 오브젝트에 collider가 있어야 한다.
// Rigidbody보다 효율이 좋아서 자주 쓰기를 권장.


