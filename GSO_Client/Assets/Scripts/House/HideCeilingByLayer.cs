using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCeilingByLayer : MonoBehaviour
{
    // ī�޶��� �⺻ Culling Mask�� ������ ����
    private int defaultCullingMask;

    // ī�޶� ���� ����
    public Camera mainCamera;

    // ���� ���̾��� �̸� (��: "Roof")
    private string roofLayerName;

    private void Start()
    {
        // ī�޶��� �⺻ Culling Mask ����
        defaultCullingMask = mainCamera.cullingMask;
        roofLayerName = "TransparencyWall";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾ �ǹ� ���η� �� ��
        if (collision.CompareTag("Player"))
        {
            Debug.Log("���̾ ���� ����ȭ");
            // Roof ���̾ �����ϵ��� ī�޶��� Culling Mask ����
            int roofLayerMask = 1 << LayerMask.NameToLayer(roofLayerName);
            mainCamera.cullingMask = defaultCullingMask & ~roofLayerMask;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �÷��̾ �ǹ� ������ ���� ��
        if (collision.CompareTag("Player"))
        {
            // ī�޶��� Culling Mask�� ������� ����
            mainCamera.cullingMask = defaultCullingMask;
        }
    }

}
