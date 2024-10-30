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

        public int Damage
        {
            get
            {
                return UsingGunData.damage;
            }
        }

        public GunState UsingGunState { get; private set; } //현재 총의 상태 : shootable, empty, reload
        public FMasterItemWeapon UsingGunData { get; private set; } //현재 사용중인 총의 스텟 데이터
        public ItemObject gunItemData;
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
        //public bool isBulletPrefShoot = false;

        public FMasterItemWeapon GetCurrentGunData()
        {
            return UsingGunData;
        }

        public void Init(Player p)
        {
            ownerPlayer = p;
            ResetGun();
        }

        public void SetGunData(int gunItemId) //여기의 id는 총의 오브젝트 ID
        {
            gunItemData = ObjectManager.Instance.Find<ItemObject>(gunItemId);
            UsingGunData = DatabaseHandler.Context.MasterItemWeapon.Find(gunItemData.ItemId); 
            UsingGunState = CurAmmo == 0 ? GunState.Empty : GunState.Shootable; 
        }

        public void ResetGun()
        {
            UsingGunData = null;
            gunItemData = null;
            CurAmmo = 0;
            UsingGunState = GunState.Empty;
            //총 제거 패킷 전송
        }

        //발사버튼 누를시
        public async void Fire(Player attacker, Vector2 pos, Vector2 dir)
        {   // TODO : 20240903 주석제거 
            //플레이어가 착용한 총의 정보
            if(UsingGunState != GunState.Shootable)
            {
                //발사 실패
                Console.WriteLine("Gun is not Shootable State");
                return;
            }

            DecreaseAmmo();

            Console.WriteLine($"attacker : {attacker}\n" +
                $"pos : {pos.X},{pos.Y}\n" +
                $"dir : {dir.X},{dir.Y}");
            //ItemObject mainWeapon = ownerPlayer.gear.GetPartItem(EGearPart.MainWeapon);
            //FMasterItemWeapon mainWeaponInfo = DatabaseHandler.Context.MasterItemWeapon.Find(mainWeapon.ItemId);

            //정규분포를 사용한 발사
            float halfAccuracyRange = UsingGunData.attack_range / 2f;
            float meanAngle = 0f;  // 발사 각도의 평균 (중앙)
            float standardDeviation = halfAccuracyRange / 3f;  // 발사 각도의 표준편차 (정확도 기반)
            float randomAngle = GetRandomNormalDistribution(meanAngle, standardDeviation);
            Vector2 direction = RotateVector(dir, randomAngle); //발사할 각도

            Vector2 endPos = Vector2.Zero;
            float length = MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            if (length != 0)
            {
                endPos = pos + (direction / length) * UsingGunData.distance;
            }
            else
            {
                endPos = pos;
            }

            RaycastHit2D hit2D = RaycastManager.Raycast(pos, direction, UsingGunData.distance, new List<GameObject>() { ownerPlayer }); //충돌객체 체크
            if (hit2D.Collider == null)
            {
                //충돌된 객체 없음
                S_RaycastShoot noHit = new S_RaycastShoot();
                noHit.HitObjectId = -1;
                noHit.ShootPlayerId = attacker.Id;
                noHit.HitPointX = endPos.X;
                noHit.HitPointY = endPos.Y;
                noHit.StartPosX = pos.X;
                noHit.StartPosY = pos.Y;
                ownerPlayer.gameRoom.BroadCast(noHit);
                Console.WriteLine("hit is null");
                await WaitForReadyToFire();
                return;
            }

            GameObject hitObject = hit2D.Collider.Parent;
            if (hitObject == null)
            {
                S_RaycastShoot noHit = new S_RaycastShoot();
                noHit.HitObjectId = -1;
                noHit.ShootPlayerId = attacker.Id;
                noHit.HitPointX = endPos.X;
                noHit.HitPointY = endPos.Y;
                noHit.StartPosX = pos.X;
                noHit.StartPosY = pos.Y;
                ownerPlayer.gameRoom.BroadCast(noHit);
                //충돌은 했는데 충돌한 객체에는 오브젝트가 없음
                Console.WriteLine("hit is null");
                await WaitForReadyToFire();
                return;
            }

            if (hitObject.ObjectType == GameObjectType.Player || hitObject.ObjectType == GameObjectType.Monster)
            {
                CreatureObj creatureObj = hitObject as CreatureObj;

                //TODO : 피격자의 hp 변화  attacker 밑에 넣기 240814지승현
                creatureObj.OnDamaged(attacker, attacker.gun.Damage);

                S_ChangeHp ChangeHpPacket = new S_ChangeHp();
                ChangeHpPacket.ObjectId = creatureObj.Id;
                ChangeHpPacket.Hp = creatureObj.Hp;

                Console.WriteLine("attacker Id :" + attacker.Id + ", " + "HIT ID " + creatureObj.Id + "HIT Hp : " + creatureObj.Hp);
                ownerPlayer.gameRoom.BroadCast(ChangeHpPacket);
            }

            S_RaycastShoot hit = new S_RaycastShoot();
            hit.HitObjectId = hitObject.Id;
            hit.ShootPlayerId = attacker.Id;
            hit.HitPointX = hit2D.hitPoint.Value.X;
            hit.HitPointY = hit2D.hitPoint.Value.Y;
            hit.StartPosX = pos.X;
            hit.StartPosY = pos.Y;
            ownerPlayer.gameRoom.BroadCast(hit);

            await WaitForReadyToFire();
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


        //재장전 버튼 누를시
        public async Task Reload()
        {
            if (CurAmmo < UsingGunData.reload_round || UsingGunState != GunState.Reloading)
            {

                await Reloading();
                
            }
        }

        //실질적인 재장전
        private async Task Reloading()
        {
            UsingGunState = GunState.Reloading;
            await Task.Delay(((int)UsingGunData.reload_time) * 1000);
            Console.WriteLine("Reload done");
            CurAmmo = UsingGunData.reload_round;
            UsingGunState = GunState.Shootable;
        }

        public async Task<bool> WaitForReadyToFire()
        {
            UsingGunState = GunState.Reloading;
            await Task.Delay((int)(UsingGunData.attack_speed * 1000));
            Console.WriteLine("ReadyToFire done");
            UsingGunState = GunState.Shootable;
            return true;
        }

        //FovPlayer의 코루틴에서 사용
        public float GetFireRate()
        {
            return (float)UsingGunData.attack_speed; // GunStat 클래스에서 설정한 발사 속도를 반환
        }
    }

    
}
