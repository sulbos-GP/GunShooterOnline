using AuthenticationServer.Service;
using AuthenticationServer.Models;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using AuthenticationServer.Repository;
using System;

namespace AuthenticationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IService   mGPGS;
        private readonly IGameDB    mGameDB;
        private readonly IMemoryDB  mMemoryDB;

        public AuthorizeController(IService gpgs, IGameDB gameDB, IMemoryDB memoryDB)
        {
            mGPGS = gpgs;
            mGameDB = gameDB;
            mMemoryDB = memoryDB;
        }

        /// <summary>
        /// 서버코드를 이용하여 토큰 얻기
        /// </summary>
        [HttpPost]
        public async Task<AccessTokenRes> ExchangeToken([FromBody] AccessTokenReq request)
        {
            Console.WriteLine($"[ExchangeToken] code:{request.server_code}");
            var response = new AccessTokenRes();

            if (request == null || 
                string.IsNullOrEmpty(request.user_id) || 
                string.IsNullOrEmpty(request.service) || 
                string.IsNullOrEmpty(request.server_code))
            {
                response.error = ErrorCode.ServerCodeNotFound;
                return response;
            }

            try
            {

                //서버코드를 이용하여 토큰 가져오기
                var token = await mGPGS.AccessToken(request.user_id, request.server_code);

                //토큰을 이용하여 구글API접근하고 프로필 가져오기
                var profile = await mGPGS.GetPlayer(token.AccessToken);

                //GameDB에서 해당 아이디가 있는지 확인
                var user = await mGameDB.SignIn(profile.GamePlayerId, request.service);

                //새로 회원가입
                if (user.Item1 == ErrorCode.LoginFailUserNotExist)
                {
                    user = await mGameDB.SignUp(profile.GamePlayerId, request.service);

                    if (user.Item1 != ErrorCode.None)
                    {
                        //throw ();
                    }
                }

                //Redis에 프로필에 있는 uid, token를 expires정해서 저장
                if(token.ExpiresInSeconds != null)
                {
                    var result = await mMemoryDB.RegistUserAsync(user.Item2, token.ExpiresInSeconds.Value, token.AccessToken, token.RefreshToken);
                }
                else
                {
                    //throw
                }


                //이미 저장되어 있다면 중복 로그인으로 생각

                response.error          = ErrorCode.None;
                response.uid            = user.Item2;
                response.access_token   = token.AccessToken;
                response.expires_in     = token.ExpiresInSeconds;
                response.scope          = token.Scope;
                response.token_type     = token.TokenType;
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExchangeToken] error:{ex.ToString()}");
                response.error = ErrorCode.IsNotValidateServerCode;
                return response;
            }

        }

    }
}
