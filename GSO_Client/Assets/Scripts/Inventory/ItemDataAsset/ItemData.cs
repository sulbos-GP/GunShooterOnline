using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ItemObjData", menuName = "Inventory/ItemObjData")]
public class ItemObjData : ScriptableObject
{
    /*
     * 스크립터블 오브젝트로 아이템의 데이터를 정의합니다.
     * itemCode(아이템 종류에 따른 코드) , 아이템의 이름, 아이템을 검색하는 시간, 크기, 이미지
     * 를 설정하여 아이템 프리팹에 부착됩니다.
     * 
     * 실제로는 컨트롤러의 리스트에 넣어 컨트롤러에서 생성할때 리스트 안의 데이터중 하나를 
     * 부착시킵니다.
     */

    public int itemCode; //아이템의 종류
    public string itemName;
    public float itemSearchTime;

    [Header("아이템의 크기")]
    public int width = 1;
    public int height = 1;

    [Header("아이템의 이미지")]
    public Sprite itemSprite;
}
