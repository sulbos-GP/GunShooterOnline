using SixLabors.ImageSharp.Formats;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private GunData equipSlot1;
    [SerializeField]
    private GunData equipSlot2;

    public Gun usingGun;

    public GunData SetSlot1
    {
        get => equipSlot1;
        set
        {
            equipSlot1 = value;

            if (value == null)  //ÃÑÀ» ÀåÂø ÇØÁ¦ ÇßÀ»¶§ ´Ù¸¥ ÀåÂøÄ­¿¡ ÃÑÀÌ µî·ÏµÇ¾î ÀÖÀ¸¸é ±×ÃÑÀ¸·Î º¯°æ
            {
                slot1btn.interactable = false;
                if(equipSlot2 != null && usingGun.curGunEquipSlot == 1)
                {
                    SetGunSlot2(equipSlot2);
                }
                else
                {
                    usingGun.ResetGun(); //´Ù¸¥ ÀåÂøÄ­¿¡µµ ÃÑÀÌ ¾ø´Ù¸é ÃÑÀ» ÇØÁ¦ÇÔ
                }
            }
            else
            {
                slot1btn.interactable = true;
                if (usingGun.CurGunData == null) //ÃÑÀ» ÀåÂøÇÒ¶§ »ç¿ëÁßÀÎ ÃÑÀÌ ¾ø´Ù¸é Áï½Ã »ç¿ë
                {
                    SetGunSlot1(value);
                }
            }
        }
    }

    public GunData SetSlot2
    {
        get => equipSlot2;
        set
        {
            equipSlot2 = value;

            if (value == null)  //ÃÑÀ» ÀåÂø ÇØÁ¦ ÇßÀ»¶§ ´Ù¸¥ ÀåÂøÄ­¿¡ ÃÑÀÌ µî·ÏµÇ¾î ÀÖÀ¸¸é ±×ÃÑÀ¸·Î º¯°æ
            {
                slot2btn.interactable = false;
                if (equipSlot1 != null && usingGun.curGunEquipSlot == 2)
                {
                    SetGunSlot1(equipSlot1);
                }
                else
                {
                    usingGun.ResetGun(); //´Ù¸¥ ÀåÂøÄ­¿¡µµ ÃÑÀÌ ¾ø´Ù¸é ÃÑÀ» ÇØÁ¦ÇÔ
                }
            }
            else
            {
                slot2btn.interactable = true;
                if (usingGun.CurGunData == null) //ÃÑÀ» ÀåÂøÇÒ¶§ »ç¿ëÁßÀÎ ÃÑÀÌ ¾ø´Ù¸é Áï½Ã »ç¿ë
                {
                    SetGunSlot2(value);
                }
            }
        }
    }

    public Button slot1btn;
    public Button slot2btn;

    public int InstanceID { get;private set; }


    public UnitStat unitStat;
    
    //TO-DO : Awake -> Spawn
    public void Awake()
    {
        Transform wQuickSlots = GameObject.Find("WQuickSlot").transform;
        slot1btn = wQuickSlots.GetChild(0).GetComponent<Button>();
        slot2btn = wQuickSlots.GetChild(1).GetComponent<Button>();

        slot1btn.interactable = false;
        slot2btn.interactable = false;

        slot1btn.onClick.RemoveAllListeners();
        slot2btn.onClick.RemoveAllListeners();
        slot1btn.onClick.AddListener(() => SetGunSlot1(equipSlot1));
        slot2btn.onClick.AddListener(() => SetGunSlot1(equipSlot2));

        usingGun = transform.Find("Pivot/Gun").GetComponent<Gun>();
        equipSlot1 = null;
        equipSlot2 = null;
    }

    public void Spawn()
    {
        Init();
    }

    public void Init()
    {
        InstanceID = gameObject.GetInstanceID();
        unitStat = new UnitStat();
        unitStat.Init();
    }

    private void SetGunSlot1(GunData gunData)
    {
        if(equipSlot1 == null)
        {
            return;
        }
        usingGun.SetGunStat(gunData);
        usingGun.curGunEquipSlot = 1;
    }
    private void SetGunSlot2(GunData gunData)
    {
        if (equipSlot2 == null)
        {
            return;
        }
        usingGun.SetGunStat(gunData);
        usingGun.curGunEquipSlot = 2;
    }
}
