using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCeilingByLayer : MonoBehaviour
{
    // 카메라의 기본 Culling Mask를 저장할 변수
    private int defaultCullingMask;

    // 카메라에 대한 참조
    public Camera mainCamera;

    // 지붕 레이어의 이름 (예: "Roof")
    private string roofLayerName;

    private void Start()
    {
        // 카메라의 기본 Culling Mask 저장
        defaultCullingMask = mainCamera.cullingMask;
        roofLayerName = "TransparencyWall";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 건물 내부로 들어갈 때
        if (collision.CompareTag("Player"))
        {
            Debug.Log("레이어를 통한 투명화");
            // Roof 레이어를 무시하도록 카메라의 Culling Mask 설정
            int roofLayerMask = 1 << LayerMask.NameToLayer(roofLayerName);
            mainCamera.cullingMask = defaultCullingMask & ~roofLayerMask;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어가 건물 밖으로 나갈 때
        if (collision.CompareTag("Player"))
        {
            // 카메라의 Culling Mask를 원래대로 복구
            mainCamera.cullingMask = defaultCullingMask;
        }
    }

}
