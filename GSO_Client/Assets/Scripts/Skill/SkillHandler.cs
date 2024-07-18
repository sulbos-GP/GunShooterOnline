using System;
using UnityEngine;

public class SkillHandler
{
    /* //�������� �Ǵ�
        Color orignalColor = obj.transform.GetComponent<SpriteRenderer>().color;// obj.transform.GetComponent<SpriteRenderer>().color;
        obj.transform.GetComponent<SpriteRenderer>().color = Color.blue;


        Action action = () => { obj.transform.GetComponent<SpriteRenderer>().color = orignalColor; Debug.Log(orignalColor); return; };
        Managers.Instance.Add(new Tuple<Action, short>(action, 100));
        //Debug.Log("Skill100");*/
    internal static void Skill10(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill100(CreatureController obj)
    {
        Debug.Log("Skill100");

        //����Ʈ ����
    }

    internal static void Skill101(CreatureController obj)
    {
        Debug.Log("Skill101");
    }

    internal static void Skill102(CreatureController obj)
    {
        Debug.Log("Skill102");
    }

    internal static void Skill103(CreatureController obj)
    {
        Debug.Log("Skill103");
    }

    internal static void Skill104(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill110(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill500(CreatureController obj)
    {
        Debug.Log("Skill500");
    }

    internal static void Skill200(CreatureController obj)
    {
        Debug.Log("Skill200");
    }

    internal static void Skill201(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill202(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill203(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill501(CreatureController obj)
    {
        // Debug.Log("Skill501");
    }


    internal static void Skill204(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill205(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill11(CreatureController obj)
    {
        Debug.Log("Skill11");

        var bullet = Managers.Resource.Instantiate("Objects/Bullet");
    }

    internal static void Skill300(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill301(CreatureController obj)
    {
        Debug.Log("��ȯ");
    }

    internal static void Skill302(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill303(CreatureController obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill304(CreatureController obj)
    {
        throw new NotImplementedException();
    }
}