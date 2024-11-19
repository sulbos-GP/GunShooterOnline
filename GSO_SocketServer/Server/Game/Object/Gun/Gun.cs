using Google.Protobuf.Protocol;
using Server.Database.Handler;
using Server.Game.Utils;
using ServerCore;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Models.MasterDatabase;

namespace Server.Game
{
    public class Gun
    {
        public Player ownerPlayer;

       
        public GunState UsingGunState { get; private set; } //현재 총의 상태 : shootable, empty, reload
        public FMasterItemWeapon GunData { get; private set; } //현재 사용중인 총의 스텟 데이터
        public ItemObject gunItemData; //해당 총에 장전된 총알이 있을경우 불러오기 위함


        #region Props

        public int Damage
        {
            get
            {
                return GunData.damage;
            }
        }
        public int CurAmmo
        {
            get => gunItemData.Loaded_ammo;
            set
            {
                if (gunItemData == null)
                {
                    return;
                }
                gunItemData.Loaded_ammo = value;
            }
        }

        #endregion


        //public bool isBulletPrefShoot = false;

        public FMasterItemWeapon GetCurrentGunData()
        {
            return GunData;
        }

        public void Init(Player p)
        {
            ownerPlayer = p;
            ResetGun();
        }

        //클라로부터 사용할 총을 지정하여 ChangeAppearence 패킷을 받은 경우 => 총의 번호가 유효할 경우
        public void SetGunData(int gunItemId) //여기의 id는 총의 오브젝트 ID
        {
            gunItemData = ObjectManager.Instance.Find<ItemObject>(gunItemId);
            GunData = DatabaseHandler.Context.MasterItemWeapon.Find(gunItemData.ItemId); 
            UsingGunState = CurAmmo == 0 ? GunState.Empty : GunState.Shootable; 
        }
        //=> 총의 번호가 0일 경우 (들고 있는 총이 없음)
        public void ResetGun()
        {
            GunData = null;
            gunItemData = null;
            CurAmmo = 0;
            UsingGunState = GunState.Empty;
        }

        //발사버튼 누를시
        public async void Fire(Player attacker, Vector2 pos, Vector2 dir)
        {   // TODO : 20240903 주석제거 
            //플레이어가 착용한 총의 정보
            if (UsingGunState != GunState.Shootable)
            {
                //발사 실패
                Console.WriteLine("Gun is not Shootable State");
                return;
            }

            DecreaseAmmo();

            Console.WriteLine($"attacker : {attacker}\n" +
                $"pos : {pos.X},{pos.Y}\n" +
                $"dir : {dir.X},{dir.Y}");

            //정규분포를 사용한 발사
            Vector2 direction = CalculateNormal(dir);
            Vector2 endPos = GetEndPos(pos, direction);

            RaycastHit2D hit2D = RaycastManager.Raycast(pos, direction, GunData.distance, new List<GameObject>() { ownerPlayer }); //충돌객체 체크
            //충돌된 객체 없음
            if (hit2D.Collider == null)
            {
                
                BroadcastHitPacket(-1, attacker, pos, endPos);
                Console.WriteLine("hit is null");
                await WaitForReadyToFire();
                return;
            }

            GameObject hitObject = hit2D.Collider.Parent;

            //충돌은 했는데 충돌한 객체에는 오브젝트가 없음
            if (hitObject == null)
            {
                BroadcastHitPacket(-1, attacker, pos, endPos);
                Console.WriteLine("hit is null");
                await WaitForReadyToFire();
                return;
            }

            if (CheckHitObjectType(hitObject))
            {
                CreatureObj creatureObj = hitObject as CreatureObj;

                //TODO : 피격자의 hp 변화  attacker 밑에 넣기 240814지승현
                creatureObj.OnDamaged(attacker, attacker.gun.Damage);

                BroadcastChangeHpPacket(attacker, creatureObj);

                Console.WriteLine("attacker Id :" + attacker.Id + ", " + "HIT ID " + creatureObj.Id + "HIT Hp : " + creatureObj.Hp);

            }

            BroadcastHitPacket(hitObject.Id, attacker, pos, hit2D.hitPoint.Value);


            await WaitForReadyToFire();
        }

        

        //히트 판정을 내릴 객체 타입을 정의
        private static bool CheckHitObjectType(GameObject hitObject)
        {
            return hitObject.ObjectType == GameObjectType.Player || hitObject.ObjectType == GameObjectType.Monster;
        }
        

        private Vector2 GetEndPos(Vector2 pos, Vector2 direction)
        {
            Vector2 normalizedDir = Vector2.Normalize(direction);
            Vector2 endPos = pos + normalizedDir * GunData.distance;

            float distance = Vector2.Distance(pos, endPos);
            Console.WriteLine($"이동거리 = {distance}");
            return endPos;
        }

        private void DecreaseAmmo()
        {
            CurAmmo = Math.Max(--CurAmmo, 0);
            Console.WriteLine($"CurAmmo = {CurAmmo}");
            if (CurAmmo == 0)
            {
                UsingGunState = GunState.Empty;
            }
        }

        #region 정규분포
        private Vector2 CalculateNormal(Vector2 dir)
        {
            float halfAccuracyRange = GunData.attack_range / 2f;
            float meanAngle = 0f;  // 발사 각도의 평균 (중앙)
            float standardDeviation = halfAccuracyRange / 3f;  // 발사 각도의 표준편차 (정확도 기반)
            float randomAngle = GetRandomNormalDistribution(meanAngle, standardDeviation);
            Vector2 direction = RotateVector(dir, randomAngle); //발사할 각도
            return direction;
        }

        public float GetRandomNormalDistribution(float mean, float standard)
        {
            //todo-> 여기서 값이 이상해짐
            // 정규 분포로 부터 랜덤값을 가져오는 함수
            float x1 = ExtensionMethod.Range(0f, 1f);
            float x2 = ExtensionMethod.Range(0f, 1f);
            float randStdNormal = Convert.ToSingle(Math.Sqrt(-2.0f * Math.Log(x1)) * Math.Sin(2.0f * Math.PI * x2));
            float randNormal = mean + standard * randStdNormal; //평균 + 표준편차* 랜덤정규분포
            return randNormal;
        }

        Vector2 RotateVector(Vector2 dir, float angleInDegrees)
        {
            // 각도를 라디안으로 변환
            float angleInRadians = (float)(angleInDegrees * (Math.PI / 180.0));
            float cos = (float)Math.Cos(angleInRadians);
            float sin = (float)Math.Sin(angleInRadians);

            // 회전된 벡터 계산
            float rotatedX = dir.X * cos - dir.Y * sin;
            float rotatedY = dir.X * sin + dir.Y * cos;

            return new Vector2(rotatedX, rotatedY);
        }
        #endregion

        //재장전 버튼 누를시
        public void Reload()
        {
            //part추가

            //나중에 수정
            var ammo = ownerPlayer.inventory.FindItem();
            
            if(ammo != null)
            {
                int target = GunData.reload_round - CurAmmo;

          
                if(ammo.Amount >= target)       //넉넉함
                {
                    int next = ammo.Amount - target;
                    //ownerPlayer.inventory.UpdateItem(ammo);
                    ownerPlayer.gameRoom.DeleteItemHandler(ownerPlayer, 0, ammo.Id);

                    ammo.Amount = next;
                }
                else                            //인벤에 있는 총알이 부족함
                {
                    int next = ammo.Amount;

                    //ownerPlayer.gameRoom.DeleteItemHandler(ownerPlayer, 0, ammo.Id);

                }







            }
            else                        //총알이 없음
            {

            }
            


            if (CurAmmo < GunData.reload_round && UsingGunState != GunState.Reloading )
            {
                UsingGunState = GunState.Reloading;
                ownerPlayer.gameRoom.PushAfter(GunData.reload_time * 1000 ,HandleReload);

            }


            S_GundataUpdate s_GundataUpdate = new S_GundataUpdate();


            s_GundataUpdate.GunData = new PS_GearInfo();
            s_GundataUpdate.GunData.Part = PE_GearPart.MainWeapon;
            s_GundataUpdate.GunData.Item = new PS_ItemInfo();
            s_GundataUpdate.GunData.Item.ObjectId = gunItemData.Id;
            s_GundataUpdate.GunData.Item.ItemId = GunData.item_id;
            s_GundataUpdate.GunData.Item.Attributes = new PS_ItemAttributes();
            s_GundataUpdate.GunData.Item.Attributes.LoadedAmmo = GunData.reload_round;
            //s_GundataUpdate.GunData.Item.Attributes.LoadedAmmo = CurAmmo;



            ownerPlayer.Session.Send(s_GundataUpdate);
        }

        //실질적인 재장전
        private void HandleReload()
        {
            CurAmmo = GunData.reload_round;
            UsingGunState = GunState.Shootable;


        }

        //다음 발사까지 대기
        public async Task<bool> WaitForReadyToFire()
        {
            UsingGunState = GunState.Reloading;
            await Task.Delay((int)(GunData.attack_speed * 1000));
            Console.WriteLine("ReadyToFire done");
            UsingGunState = GunState.Shootable;
            return true;
        }

        public static void BroadcastChangeHpPacket(Player attacker, CreatureObj creatureObj)
        {
            S_ChangeHp ChangeHpPacket = new S_ChangeHp();
            ChangeHpPacket.ObjectId = creatureObj.Id;
            ChangeHpPacket.Hp = creatureObj.Hp;
            attacker.gameRoom.BroadCast(ChangeHpPacket);
        }

        public static void BroadcastHitPacket(int hitObjectId, Player attacker, Vector2 startPos, Vector2 hitPoint)
        {
            S_RaycastShoot hit = new S_RaycastShoot
            {
                HitObjectId = hitObjectId,
                ShootPlayerId = attacker.Id,
                HitPointX = hitPoint.X,
                HitPointY = hitPoint.Y,
                StartPosX = startPos.X,
                StartPosY = startPos.Y
            };
            attacker.gameRoom.BroadCast(hit);
        }
    }
}
