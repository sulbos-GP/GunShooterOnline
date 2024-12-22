using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Enum;

namespace Server.Game
{

    public class WeaponInventory
    {


        private enum currentWeapon
        {
            None = 0,
            MAIN = 1,
            SUB = 2,
        }


        public Player ownerPlayer;



        currentWeapon current = currentWeapon.None;


        public Gun MainGun;
        public Gun SubGun;



        internal void Init(Player player)
        {
            current = currentWeapon.None;

            ownerPlayer = player;

            MainGun = new Gun();
            ItemObject mainGunItem = ownerPlayer.gear.GetPartItem(WebCommonLibrary.Enum.EGearPart.MainWeapon);
            if (mainGunItem != null)
            {
                MainGun.SetGunData(mainGunItem.Id);
                MainGun.Init(this);
            }

            SubGun = new Gun();
            ItemObject subGunItem = ownerPlayer.gear.GetPartItem(WebCommonLibrary.Enum.EGearPart.SubWeapon);
            if (subGunItem != null)
            {
                SubGun.SetGunData(subGunItem.Id);
                SubGun.Init(this);
            }
        }

     


        public PE_GearPart GetCurrentWeaponGearPart()
        {
            switch (current)
            {
                case currentWeapon.MAIN:
                    return PE_GearPart.MainWeapon;
                case currentWeapon.SUB:
                    return PE_GearPart.SubWeapon;
                default:
                    Console.WriteLine("Error : currentType is Zero");
                    return PE_GearPart.None;
            }
        }
    
        public Gun GetCurrentWeapon()
        {
            switch (current)
            {
                case currentWeapon.MAIN:
                    return MainGun;
                case currentWeapon.SUB:
                    return SubGun;
                default:
                    Console.WriteLine("Error : currentType is Zero");
                    return null;
            }
        }
        public void Fire(Player attacker, Vector2 pos, Vector2 dir)
        {
            if (GetCurrentWeapon().UsingGunState != GunState.Shootable)
            {
                return;
            }
            GetCurrentWeapon().Fire(attacker, pos, dir);
        }

        internal void ResetGun()
        {
            GetCurrentWeapon().ResetGun();
            current = currentWeapon.None;
        }

        internal void SetGunData(PS_GearInfo info)
        {
            int gunId = 0;

            switch (info.Part)
            {
                case PE_GearPart.None:
                    Console.WriteLine("WeaponInventory info.Part is None");
                    break;
                case PE_GearPart.MainWeapon:
                    current = currentWeapon.MAIN;
                    gunId = ownerPlayer.gear.GetPartItem(WebCommonLibrary.Enum.EGearPart.MainWeapon).Id;
                    break;
                case PE_GearPart.SubWeapon:
                    current = currentWeapon.SUB;
                    gunId = ownerPlayer.gear.GetPartItem(WebCommonLibrary.Enum.EGearPart.SubWeapon).Id;

                    break;
              
            }


            

            GetCurrentWeapon().SetGunData(gunId);

        }

        public void CancelReload()
        {
            GetCurrentWeapon().CancelReload();
        }

        internal void Reload()
        {
            GetCurrentWeapon().Reload();

        }
    }

}