using GSO_WebServerLibrary.Utils;
using GsoWebServer.DTO;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.DTO.Game;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Error;

namespace GsoWebServer.Controllers.Game
{
    [Route("api/Game/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IGameService mGameService;

        public StorageController(IGameService game)
        {
            mGameService = game;
        }

        [HttpPost]
        [Route("Load")]
        public async Task<LoadStorageRes> Load([FromHeader] HeaderDTO header, [FromBody] LoadStorageReq request)
        {
            Console.WriteLine($"[Game.Storage.Load] uid:{header.uid} stoarge_id:{request.storage_id}");


            var response = new LoadStorageRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "요청이 올바르지 않습니다.";
                return response;
            }

            var (error, items) = await mGameService.LoadInventory(request.storage_id);
            if(error != WebErrorCode.None || items == null)
            {
                response.error_code = error;
                response.error_description = "저장소 아이디가 존재하지 않습니다.";
                return response;
            }

            response.error_code = WebErrorCode.None;
            response.error_description = "";
            response.items = items.ToList();
            return response;
        }
    }
}
