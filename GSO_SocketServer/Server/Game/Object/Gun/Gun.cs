using Google.Protobuf.Protocol;
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

namespace Server.Game
{
    public class Gun
    {
        public Player ownerPlayer;

        public GunState gunState { get; private set; }

        public GunData gunData { get; private set; }
        public int _curAmmo { get; private set; } //현재 장탄

        private float _lastFireTime;

        public bool isBulletPrefShoot = false;

        //총알 private LineRenderer bulletLine;
        //private LineRenderer rangeLine;
        private Vector3 _fireStartPos;//궤적 라인 렌더러 (디버깅 라인을 라인렌더러로 표현)
       
        private Vector3 _direction;      // Ray가 향하는 방향

        CoroutineManager coroutineManager = new();

      

        //public void SetGunStat(GunStat gunStat)
        //{
        //    gunData = new GunData();
        //    gunData.range = gunStat.range;
        //    gunData.ammo = gunStat.ammo;
        //    gunData.fireRate = gunStat.fireRate;
        //    gunData.gunType = gunStat.gunType;
        //    gunData.bulletObj = gunStat.bulletObj;
        //    gunData.accuracy = gunStat.accuracy;
        //    gunData.damage = gunStat.damage;
        //    gunData.reloadTime = gunStat.reloadTime;
        //}

        public GunData getGunStat()
        {
            return gunData;
        }

        public void Init(Player p)
        {
            ExtensionMethod.Start();
            GunDataManager.instance.Init();
            gunData = GunDataManager.instance.gunDatas[0];

            //Debug Line
            /*bulletLine = GetComponent<LineRenderer>();
            rangeLine = transform.GetChild(0).GetComponent<LineRenderer>();
            bulletLine.positionCount = 2;
            rangeLine.positionCount = 5;*/

            //게임 시작시 실행할 루틴
            //SetGunStat(_gunStat);
            //_curAmmo = gunData.ammo;
            //gunState = GunState.Shootable;
            //_fireStartPos = transform.GetChild(0);

            ownerPlayer = p;
        }

       /* private void Update()
        {
            SetFireLine();
        }*/

        /*private void SetFireLine()
        {
            //발사범위 선 2개 긋기
            if (_fireStartPos == null) return;

            float halfAngle = gunData.accuracy * 0.5f;

            Vector3 direction1 = ExtensionMethod.Quaternion.Euler(0, 0, halfAngle) * _fireStartPos.up;
            Vector3 endPoint1 = _fireStartPos.position + direction1 * gunData.range;


            Vector3 direction2 = ExtensionMethod.Quaternion.Euler(0, 0, -halfAngle) * _fireStartPos.up;
            Vector3 endPoint2 = _fireStartPos.position + direction2 * gunData.range;


            rangeLine.SetPosition(0, _fireStartPos.position);
            rangeLine.SetPosition(1, endPoint1);
            rangeLine.SetPosition(2, _fireStartPos.position);
            rangeLine.SetPosition(3, endPoint2);
            rangeLine.SetPosition(4, _fireStartPos.position);
        }*/

        //발사버튼 누를시
        public bool Fire(Player attacker, Vector2 pos, Vector2 dir, float length)
        {
            if (gunState == GunState.Shootable && ExtensionMethod.time >= _lastFireTime + 1 / gunData.fireRate)
            {
                /*
                 발사 코드 작성.
                 총알을 발사하든 레이케스트로 충돌감지를 하든
                 적중시 패킷을 서버에게 전달
                 */

                //정규분포를 사용한 발사
                float halfAccuracyRange = gunData.accuracy / 2f;

                float meanAngle = 0f;  // 발사 각도의 평균 (중앙)
                float standardDeviation = halfAccuracyRange / 3f;  // 발사 각도의 표준편차 (정확도 기반)
                float randomAngle = GetRandomNormalDistribution(meanAngle, standardDeviation);
                //Vector3 direction = ExtensionMethod.Quaternion.Euler(0, 0, randomAngle) *_fireStartPos.up;

                //레이캐스트를 사용한 방법
               // RaycastHit2D hit = Physics2D.Raycast(_fireStartPos.position, direction, gunData.range);


                RaycastHit2D hit2D = RaycastManager.Raycast(pos + pos * dir * 0.5f, dir, length);



                if (hit2D.Collider == null)
                {
                    return false;
                }

                GameObject hitObject = hit2D.Collider.Parent;
                if (hitObject == null)
                {
                    Console.WriteLine("hit is null");
                    return false;
                }

                if (hitObject.ObjectType == GameObjectType.Player || hitObject.ObjectType == GameObjectType.Monster)
                {
                    CreatureObj creatureObj = hitObject as CreatureObj;


                    //TODO : 공격력  attacker 밑에 넣기 240814지승현
                    creatureObj.OnDamaged(attacker, 3);

                    S_ChangeHp ChangeHpPacket = new S_ChangeHp();
                    ChangeHpPacket.ObjectId = creatureObj.Id;
                    ChangeHpPacket.Hp = creatureObj.Hp;

                    Console.WriteLine("attacker Id :" + attacker.Id + ", " + "HIT ID " + creatureObj.Id + "HIT Hp : " + creatureObj.Hp);

                    ownerPlayer.gameRoom.BroadCast(ChangeHpPacket);
                }

                S_RaycastHit packet = new S_RaycastHit();
                packet.HitObjectId = hitObject.Id;
                packet.RayId = hit2D.rayID;
                packet.Distance = hit2D.distance;
                packet.HitPointX = hit2D.hitPoint.Value.X;
                packet.HitPointY = hit2D.hitPoint.Value.Y;
                packet.StartPosX = pos.X;
                packet.StartPosY = pos.Y;


                ownerPlayer.gameRoom.BroadCast(packet);

                

                if (isBulletPrefShoot)
                {
                    //총알을 사용한 방법
                    //Bullet bullet = gunData.bulletObj;
                    //bullet._damage = gunData.damage;
                    //bullet._range = gunData.range;
                    //bullet._dir = direction;
                    ObjectManager.Instance.Add((gunData.bulletObj));
                    //Instantiate(bullet, _fireStartPos.position, _fireStartPos.rotation);
                }
                _lastFireTime = ExtensionMethod.time;//마지막 사격 시간 업데이트

                _curAmmo--; //현재 총알감소
                _curAmmo = Math.Max(_curAmmo, 0);
                if (_curAmmo == 0)
                {
                    gunState = GunState.Empty;
                }
                return true;
            }
            return false; //발사 성공 여부
        }

        public float GetRandomNormalDistribution(float mean, float standard)
        {
            // 정규 분포로 부터 랜덤값을 가져오는 함수
            float x1 = ExtensionMethod.Range(0f, 1f);
            float x2 = ExtensionMethod.Range(0f, 1f);
            float randStdNormal = Convert.ToSingle(Math.Sqrt(-2.0f * Math.Log(x1)) * Math.Sin(2.0f * Math.PI * x2));
            float randNormal = mean + standard * randStdNormal; //평균 + 표준편차* 랜덤정규분포
            return randNormal;
        }


        //재장전 버튼 누를시
        public async Task Reload()
        {
            if (_curAmmo < gunData.ammo || gunState != GunState.Reloading)
            {
                await Reloading();
                
            }
        }

        //실질적인 재장전
        private async Task Reloading()
        {
            gunState = GunState.Reloading;
            await Task.Delay(gunData.reloadTime);
            
            _curAmmo = gunData.ammo;
            gunState = GunState.Shootable;
        }

        //FovPlayer의 코루틴에서 사용
        public float GetFireRate()
        {
            return gunData.fireRate; // GunStat 클래스에서 설정한 발사 속도를 반환
        }
    }

    [System.Serializable]
    public class GunData
    {
        public float range;       // 사거리.
        public float fireRate;    // 연사 속도. 값과 속도가 반비례
        public int ammo;          // 장탄수(보유장탄)
        public float accuracy;    // 발사 각도(정확도)
        public int damage;        // 데미지
        public GameObject bulletObj;
        public GunType gunType;
        public int reloadTime;

        public GunData(float range, float fireRate, int ammo, float accuracy, int damage, GameObject bulletObj, GunType gunType, int reloadTime)
        {
            this.range = range;
            this.fireRate = fireRate;
            this.ammo = ammo;
            this.accuracy = accuracy;
            this.damage = damage;
            this.bulletObj = bulletObj;
            this.gunType = gunType;
            this.reloadTime = reloadTime;
        }
    }
}
