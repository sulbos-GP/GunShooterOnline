using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_Item : BaseData<Data_Item>
{
    public string code;
    public string name;
    public float weight;
    public eITEM_TYPE type;
    public int description;
    public int scale_x;
    public int scale_y;
    public int purchase_price;
    public float inquiry_time;
    public int sell_price;
    public int stack_count;
    //TO-DO : 추후에 아래 2개는 다른 방식으로 대체 가능할 것 같음 이야기 할 예정.
    public string prefab;
    public string icon;
}
