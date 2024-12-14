using GSO_WebServerLibrary.Reposiotry.Interfaces;
using GSO_WebServerLibrary.Utils;
using GsoWebServer.DTO;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.DTO.Game;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using static Google.Apis.Requests.RequestError;

namespace GsoWebServer.Controllers.Game
{
    [Route("api/Game/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IGameService mGameService;
        private readonly IMasterDB mMasterDB;

        public StorageController(IGameService game, IMasterDB master)
        {
            mGameService = game;
            mMasterDB = master;
        }

        [HttpPost]
        [Route("Load")]
        public async Task<LoadStorageRes> Load([FromHeader] HeaderDTO header, [FromBody] LoadStorageReq request)
        {
            Console.WriteLine($"[Game.Storage.Load] uid:{header.uid}");

            var response = new LoadStorageRes();
            response.error_code = WebErrorCode.None;

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "요청이 올바르지 않습니다.";
                return response;
            }

            var (error, gears) = await mGameService.LoadGear(header.uid);
            if (error != WebErrorCode.None || gears == null)
            {
                response.error_code = error;
                response.error_description = "저장소 아이디가 존재하지 않습니다.";
                return response;
            }

            DB_GearUnit? backpack = gears.FirstOrDefault(gear => gear.attributes.unit_storage_id != null);
            if(backpack == null || backpack.attributes.unit_storage_id.HasValue == false)
            {
                response.gears = gears.ToList();
                return response;
            }

            (error, var items) = await mGameService.LoadInventory(backpack.attributes.unit_storage_id.Value);
            if (error != WebErrorCode.None || items == null)
            {
                response.error_code = error;
                response.error_description = "저장소 아이디가 존재하지 않습니다.";
                return response;
            }

            response.gears = gears.ToList();
            response.items = items.ToList();
            return response;
        }

        [HttpPost]
        [Route("Reset")]
        public async Task<ResetStorageRes> Reset([FromHeader] HeaderDTO header, [FromBody] ResetStorageReq request)
        {
            Console.WriteLine($"[Game.Storage.Reset] uid:{header.uid}");

            var response = new ResetStorageRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "요청이 올바르지 않습니다.";
                return response;
            }

            var (error, gears) = await mGameService.LoadGear(header.uid);
            if (error != WebErrorCode.None || gears == null)
            {
                response.error_code = error;
                response.error_description = "저장소 아이디가 존재하지 않습니다.";
                return response;
            }

            DB_GearUnit? backpack = gears.FirstOrDefault(gear => gear.attributes.unit_storage_id != null);
            if (backpack != null && backpack.attributes.unit_storage_id.HasValue != false)
            {
                await mGameService.ClearInventory(backpack.attributes.unit_storage_id.Value);
            }
            await mGameService.ClearGear(header.uid);

            //임시 스타터팩
            {
                //권총
                DB_GearUnit pistol = new DB_GearUnit()
                {
                    gear = new DB_Gear()
                    {
                        part = "sub_weapon",
                        unit_attributes_id = 0
                    },

                    attributes = new DB_UnitAttributes()
                    {
                        item_id = mMasterDB.Context.MasterItemBase.First(item => item.Value.name == "Colt45").Value.item_id,
                        amount = 1,
                        loaded_ammo = 0,
                        durability = 0,
                        unit_storage_id = null,
                    }
                };
                await mGameService.InsertGear(header.uid, pistol);

                //기본 가방
                DB_GearUnit defaultBackpack = new DB_GearUnit()
                {
                    gear = new DB_Gear()
                    {
                        part = "backpack",
                        unit_attributes_id = 0
                    },

                    attributes = new DB_UnitAttributes()
                    {
                        item_id = mMasterDB.Context.MasterItemBase.First(item => item.Value.name == "기본가방").Value.item_id,
                        amount = 1,
                        loaded_ammo = 0,
                        durability = 0,
                        unit_storage_id = null,
                    }
                };
                int storage_id = await mGameService.InsertGearBackpackItem(header.uid, defaultBackpack);
       
                //붕대
                DB_GearUnit band = new DB_GearUnit()
                {
                    gear = new DB_Gear()
                    {
                        part = "pocket_first",
                        unit_attributes_id = 0
                    },

                    attributes = new DB_UnitAttributes()
                    {
                        item_id = mMasterDB.Context.MasterItemBase.First(item => item.Value.name == "밴드").Value.item_id,
                        amount = 3,
                        loaded_ammo = 0,
                        durability = 0,
                        unit_storage_id = null,
                    }
                };
                await mGameService.InsertGear(header.uid, band);

                //권총 총알
                DB_ItemUnit bullet = new DB_ItemUnit()
                {
                    storage = new DB_StorageUnit()
                    {
                        grid_x = 0,
                        grid_y = 0,
                        rotation = 0,
                        unit_attributes_id = 0
                    },

                    attributes = new DB_UnitAttributes()
                    {
                        item_id = mMasterDB.Context.MasterItemBase.First(item => item.Value.name == "5.59mm").Value.item_id,
                        amount = 12,
                        loaded_ammo = 0,
                        durability = 0,
                        unit_storage_id = null,
                    }
                };

                await mGameService.InsertInventory(storage_id, bullet);
            }

            (error, response.gears) = await mGameService.LoadGear(header.uid);
            if (error != WebErrorCode.None)
            {
                response.error_code = error;
                response.error_description = "장비 로드에 실패하였습니다.";
                return response;
            }

            if (response.gears != null)
            {
                var newBackpack = response.gears.FirstOrDefault(gear => gear.gear.part == "backpack");
                if (newBackpack != null && newBackpack.attributes.unit_storage_id != null)
                {
                    (error, response.items) = await mGameService.LoadInventory(newBackpack.attributes.unit_storage_id.Value);
                    if (error != WebErrorCode.None)
                    {
                        response.error_code = error;
                        response.error_description = "인벤토리 로드에 실패하였습니다.";
                        return response;
                    }
                }
            }

            response.error_code = WebErrorCode.None;
            response.error_description = "";
            return response;
        }
    }
}
