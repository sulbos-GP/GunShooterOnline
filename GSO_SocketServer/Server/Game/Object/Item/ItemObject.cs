using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ItemObject
    {
        //clientSession.MyPlayer = ObjectManager.Instance.Add<Player>(); 

        public int itemId; // 해당 아이템의 고유한 아이디
        public int itemCode; //아이템의 종류(해당 아이템을 DB에서 조회하기 위한 코드)
        public Vector2Int itemPos; // 아이템의 그리드 안 좌표상의 위치
        public int itemRotate; // 아이템의 회전코드(rotate * 90)
        public int itemAmount = 1; // 아이템의 개수(소모품만 64개까지)
        public List<int> searchedPlayerId; // 이 아이템을 조회한 플레이어의 아이디

        //임시변수(아이템 코드를 통해 데이터베이스에서 불러오기가 가능할때까지)
        public string item_name;
        public float item_weight; //아이템의 무게
        public int item_type;
        public int item_string_value;
        public int item_purchase_price;
        public int item_sell_price;
        public float item_searchTime;
        public int width = 1;
        public int height = 1;
        public bool isItemConsumeable; //임시(아이템 타입으로 유추가능, 아이템 머지에 소모품인지 판단함. 이후 코드를 통해 조회로 변경)

    }
}
