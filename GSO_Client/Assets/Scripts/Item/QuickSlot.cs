using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    //������ ��ư 1, 2, 3�� ��������
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
    /// ������ �̹����� ���� �ؽ�Ʈ ����
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
    /// ������ ������ ���� -> ����Ʈ �̹��� �� �ؽ�Ʈ�� ����
    /// </summary>
    public void ResetSlot()
    {
        itemData = null;
        GetComponent<Button>().interactable = false; 
        itemImage.sprite = defaultSprite;
        itemAmount.text = "x";
    }

    /// <summary>
    /// ��ϵ� �������� ���. �̰� �������� ����� ���;��ҵ�
    /// </summary>
    public void UseQuickSlot(ItemData Item)
    {
        //ȸ��? �̼�����? ��������? ��������?
        if(Item == null )
        {
            Debug.Log("�������� ��ϵǾ����� ����");
            return;
        }

        //���� ������ ��� ��Ŷ ������ ��ü
        //��Ŷ�� ���� ������ �ش� �������� ���Ǿ��ٴ°��� �˷��ָ� �������ִ� �ش� �������� ������ -1 ���Ѿ���

        Item.amount -= 1; //�������� ���� ����

        if (Item.amount == 0) //������ 0�̵Ǹ� ������ ���� �� ���� ����
        {

            ResetSlot();
        }
        else
        {
            itemAmount.text = Item.amount.ToString();
        }


        Debug.Log($"{gameObject.name}ĭ�� ��ϵ� ������ ����");
    }
}
