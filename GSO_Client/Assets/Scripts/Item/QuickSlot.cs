using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    //퀵슬롯 버튼 1, 2, 3에 넣을거임
    public Sprite defaultSprite;

    public ItemData itemData;
    private Image itemImage;
    private TextMeshProUGUI itemAmount;

    private void Awake()
    {
        itemData = null;
        itemImage = transform.GetChild(0).GetComponent<Image>();
        itemAmount = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => UseQuickSlot(itemData));

        ResetSlot();
    }


    /// <summary>
    /// 슬롯의 이미지와 수량 텍스트 설정
    /// </summary>
    public void SetSlot(ItemData item)
    {
        itemData = item;
        GetComponent<Button>().interactable = true;

        Sprite itemSprite = ItemObject.FindItemSprtie(item);
        itemImage.sprite = itemSprite;
        itemAmount.text = item.amount.ToString();
    }

    /// <summary>
    /// 슬롯의 아이템 제거 -> 디폴트 이미지 및 텍스트로 변경
    /// </summary>
    public void ResetSlot()
    {
        itemData = null;
        GetComponent<Button>().interactable = false; 
        itemImage.sprite = defaultSprite;
        itemAmount.text = "x";
    }

    /// <summary>
    /// 등록된 아이템을 사용. 이건 아이템의 기능이 나와야할듯
    /// </summary>
    public void UseQuickSlot(ItemData Item)
    {
        //회복? 이속증가? 공속증가? 방어력증가?
        if(Item == null )
        {
            Debug.Log("아이템이 등록되어있지 않음");
            return;
        }

        //추후 아이템 사용 패킷 생성후 교체
        //패킷을 통해 서버에 해당 아이템이 사용되었다는것을 알려주며 서버에있는 해당 아이템의 개수를 -1 시켜야함

        Item.amount -= 1; //아이템의 개수 감소

        if (Item.amount == 0) //개수가 0이되면 아이템 삭제 및 슬롯 리셋
        {

            ResetSlot();
        }
        else
        {
            itemAmount.text = Item.amount.ToString();
        }


        Debug.Log($"{gameObject.name}칸에 등록된 아이템 사용됨");
    }
}
