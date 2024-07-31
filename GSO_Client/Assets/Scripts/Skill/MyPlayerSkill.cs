using Google.Protobuf.Protocol;
using Server.Data;
using UnityEngine;

public partial class MyPlayerController
{
    private void UseSkill(int number, int[] targets = null, Vector2? dir = null)
    {
        Skill skill;
        if (DataManager.SkillDict.TryGetValue(number, out skill))
            _coskillCoolTime = StartCoroutine(coInputCoolTime(skill.cooldown));
        else
            Debug.Log($"{number} SkillDict 오류");

        if ((dir == null && skill.projectile != null) || dir == Vector2.zero)
        {
            Debug.LogError("DIr 없음");
            dir = Vector2.right;
        }
        //----------------- 검사 ------------------

       /* var skillpacket = new C_Skill();
        skillpacket.Info = new SkillInfo { SkillId = number };

        if (targets != null) skillpacket.TargetIds.Add(targets);

        if (dir != null)
        {
            skillpacket.Info.DirX = dir.Value.x;
            skillpacket.Info.DirY = dir.Value.y;
        }

        Managers.Network.Send(skillpacket);*/
    }
}