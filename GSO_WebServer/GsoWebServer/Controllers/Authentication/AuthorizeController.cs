using Microsoft.AspNetCore.Mvc;
using GsoWebServer.Servicies.Interfaces;
using GsoWebServer.Reposiotry.Interfaces;
using GSO_WebServerLibrary;
using GsoWebServer.DTO.Authentication;
using Google.Apis.Games.v1;
using static Google.Apis.Requests.RequestError;
using GsoWebServer.Servicies.Game;


namespace AuthenticationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IAuthenticationService mAuthenticationService;
        private readonly IGameService mGameService;
        private readonly IDataLoadService mDataLoadService;

        public AuthorizeController(IAuthenticationService auth, IGameService game, IDataLoadService dataload)
        {
            mAuthenticationService = auth;
            mGameService = game;
            mDataLoadService = dataload;
        }

        /// <summary>
        /// 로그인
        /// </summary>
        [HttpPost]
        [Route("SignIn")]
        public async Task<SignInRes> SignIn([FromBody] SignInReq request)
        {

            Console.WriteLine($"[SignIn] user:{request.user_id} code:{request.server_code} service:{request.service}");

            var response = new SignInRes();
            WebErrorCode errorCode = WebErrorCode.None;

            if (!WebUtils.IsValidModelState(request))
            {
                response.error = WebErrorCode.IsNotValidModelState;
                return response;
            }

            try
            {

                //서버코드를 이용하여 토큰 가져오기
                (errorCode, var token) = await mAuthenticationService.ExchangeToken(request.user_id, request.server_code);
                if (token is null)
                {
                    response.error = errorCode;
                    return response;
                }

                //토큰을 이용하여 구글API접근하고 프로필 가져오기
                (errorCode, var player) = await mAuthenticationService.GetMyPlayer(request.user_id, token.AccessToken);
                if (player is null)
                {
                    response.error = errorCode;
                    return response;
                }

                //해당 유저(플레이어)가 있는지 확인
                (errorCode, var uid) = await mAuthenticationService.VerifyUser(player.GamePlayerId, request.service);
                if (errorCode == WebErrorCode.SignInFailUserNotExist)
                {
                    (errorCode, uid) = await mGameService.SingUpWithNewUserGameData(player.GamePlayerId, request.service);
                }
                
                if (errorCode != WebErrorCode.None || uid == 0)
                {
                    response.error = errorCode;
                    return response;
                }

                if (token.ExpiresInSeconds == null)
                {
                    response.error = errorCode;
                    return response;
                }

                //Redis에 토큰 저장
                errorCode = await mAuthenticationService.RegisterToken(uid, token.ExpiresInSeconds.Value, token.AccessToken, token.RefreshToken);

                //이미 저장되어 있다면 중복 로그인으로 생각
                //TODO : 나중에 처리

                if (errorCode != WebErrorCode.None)
                {
                    response.error = errorCode;
                    return response;
                }

                //최근 로그인 시간 변경
                errorCode = await mAuthenticationService.UpdateLastSignInTime(uid);
                if (errorCode != WebErrorCode.None)
                {
                    response.error = errorCode;
                    return response;
                }

                //유저 데이터 로드
                (errorCode, response.userData) = await mDataLoadService.LoadUserData(uid);
                if (errorCode != WebErrorCode.None)
                {
                    response.error = errorCode;
                    return response;
                }

                response.error          = WebErrorCode.None;
                response.uid            = uid;
                response.access_token   = token.AccessToken;
                response.expires_in     = token.ExpiresInSeconds.Value;
                response.scope          = token.Scope;
                response.token_type     = token.TokenType;
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExchangeToken] error:{ex.ToString()}");
                response.error = WebErrorCode.IsNotValidateServerCode;
                return response;
            }

        }

        //[HttpPost]
        //[Route("RefreshToken")]
        //public async Task<RefreshTokenRes> RefreshToken([FromBody] RefreshTokenReq request)
        //{

        //    Console.WriteLine($"[RefreshToken] uid:{request.uid} access:{request.access_token} refesh:{request.refresh_token}");

        //    var response = new RefreshTokenRes();
        //    if (!WebUtils.IsValidModelState(request))
        //    {
        //        response.error = WebErrorCode.IsNotValidModelState;
        //        return response;
        //    }

        //    try
        //    {
        //        //유저아이디를 통해 UID가져오기
        //        var uid = await mAuthenticationService.(request.user_id);
        //        if (uid.Item1 != WebErrorCode.None || uid.Item2 == 0)
        //        {
        //            return response;
        //        }

        //        //토큰이 만료 되었는지 확인 및 유저의 정보 가져오기
        //        var user = await mMemoryDB.GetRegistUserAsync(uid.Item2);
        //        if (user.Item1 != WebErrorCode.None || user.Item2.user == null)
        //        {
        //            return response;
        //        }

        //        if (string.IsNullOrEmpty(user.Item2.user.refresh_token))
        //        {
        //            return response;
        //        }

        //        //유저의 새로운 토큰 생성
        //        var token = await mAuthenticationService.RefreshToken(request.user_id, user.Item2.user.refresh_token);
        //        if (token == null)
        //        {
        //            return response;
        //        }

        //        //재생성된 새로운 토큰 저장
        //        if (token.ExpiresInSeconds != null)
        //        {
        //            var result = await mMemoryDB.RegistUserAsync(uid.Item2, token.ExpiresInSeconds.Value, token.AccessToken, token.RefreshToken);
        //            if (result != WebErrorCode.None)
        //            {
        //                return response;
        //            }
        //        }
        //        else
        //        {
        //            return response;
        //        }

        //        response.error = WebErrorCode.None;
        //        response.new_access_token = token.AccessToken;
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[RefreshToken] error:{ex.ToString()}");
        //        response.error = WebErrorCode.IsNotValidateServerCode;
        //        return response;
        //    }

        //}
    }
}
