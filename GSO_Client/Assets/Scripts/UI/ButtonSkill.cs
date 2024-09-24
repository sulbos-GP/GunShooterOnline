using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Server.Data;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSkill : MonoBehaviour
{
    public int ButtonCount;

    public List<GameObject> skillBtns;
    public GameObject MainBtn;


    private MyPlayerController myPlayerController;


    List<Skill> playerSkills = new List<Skill>();

    public void Init(StatInfo stat)
    {
        var _class = stat.Class;
        //var _level = stat.Level;


        for (var i = 0; i < 10; i++) //기본적인 스킬 범위
        {
            Skill _skill;

            if (DataManager.SkillDict.TryGetValue(_class * 100 + i, out _skill))
                playerSkills.Add(_skill);
        }


        var _order = 0;
        foreach (var _skill in playerSkills) // 스킬 이름에 따른 스프라이트 만들기
        {
            var i = _skill.id;
            
            if (_order == 0)
            {
                var mainImg = Util.FindChild<Image>(MainBtn, "Icon", true);
                mainImg.sprite = Resources.Load<Sprite>($"Sprite/Skill/{i}");
            }
            else
            {
                var subImg = Util.FindChild<Image>(skillBtns[_order - 1], "Icon", true);
                subImg.sprite = Resources.Load<Sprite>($"Sprite/Skill/{i}");
            }

            _order++;
            //Todo : 개수에 맞게 추가
        }

        //UpdateBtn(_level);

    }

    public void UpdateBtn(int _level)
    {
        for (var i = 0; i < playerSkills.Count; i++) // 버튼 레벨에 따라 활성화
        {
            if (i <= _level)
            {
                var success = ActivateBtn(i, true);
                Debug.Log(i + "번 활성화" + success);
            }
            else
            {
                var success = ActivateBtn(i, false);
                Debug.Log(i + "번 비활성화" + success);
            }
        }
    }
    

    private bool ActivateBtn(int num, bool active)
    {
        if (num == 0)
        {
            //MainBtn.GetComponent<UIButton>().interactable = active;
            return true;
        }

        num--;
        if (skillBtns.Count > num)
        {
            //skillBtns[num].GetComponent<UIButton>().interactable = active;
            return true;
        }

        return false;
    }


    public void OnPressedBtn(int i) //ui에서 버튼 누르면
    {
        //0번 ~ 4번
        //100번 클래스인면 
        if (myPlayerController == null)
        {
            myPlayerController = FindObjectOfType<MyPlayerController>();

            if (myPlayerController == null)
            {
                Debug.Log("Player not found");
                return;
            }
        }

        //플레이어가 있으면
        myPlayerController.UseSkill_Requst(i);
    }
}