using Microsoft.AspNetCore.Mvc;
using GsoWebServer.Servicies.Interfaces;
using GSO_WebServerLibrary.Utils;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.Error;
using GsoWebServer.DTO;
using WebCommonLibrary.Models.MemoryDB;
using WebCommonLibrary.Models.GameDB;


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
        /// 기존 유저인지 인증
        /// </summary>
        [HttpPost]
        [Route("Authentication")]
        public async Task<AuthenticationRes> Authentication([FromBody] AuthenticationReq request)
        {
            Console.WriteLine($"[Authentication] user:{request.user_id} service:{request.service}");

            var response = new AuthenticationRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            //해당 유저(플레이어)가 있는지 확인
            var (errorCode, uid) = await mAuthenticationService.VerifyUser(request.user_id, request.service);
            if (errorCode == WebErrorCode.SignInFailUserNotExist)
            {
                response.error_code = WebErrorCode.TEMP_ERROR;
                response.error_description = "";
                return response;
            }

            //유저의 정보 얻어오기
            (errorCode, var user) = await mGameService.GetUserInfo(uid);
            if (errorCode != WebErrorCode.None || user == null)
            {
                response.error_code = WebErrorCode.TEMP_ERROR;
                response.error_description = "";
                return response;
            }

            //마지막 로그인한 날짜로부터 6개월 이상이 지났다면 다시 갱신토큰 받을 수 있도록 하기
            var now = DateTime.Now;
            int monthsDifference = (now.Year - user.recent_login_dt.Year) * 12 + now.Month - user.recent_login_dt.Month;
            if(monthsDifference > 6)
            {
                response.error_code = WebErrorCode.TEMP_ERROR;
                response.error_description = "";
                return response;
            }

            response.error_code = WebErrorCode.None;
            return response;
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
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            try
            {

                //서버코드를 이용하여 토큰 가져오기
                (errorCode, var token) = await mAuthenticationService.ExchangeToken(request.user_id, request.server_code);
                if (token is null)
                {
                    response.error_code = errorCode;
                    response.error_description = "";
                    return response;
                }

                //토큰을 이용하여 구글API접근하고 프로필 가져오기
                (errorCode, var player) = await mAuthenticationService.GetMyPlayer(request.user_id, token.AccessToken);
                if (player is null)
                {
                    response.error_code = errorCode;
                    response.error_description = "";
                    return response;
                }

                //해당 유저(플레이어)가 있는지 확인
                (errorCode, var uid) = await mAuthenticationService.VerifyUser(request.user_id, request.service);
                if (errorCode == WebErrorCode.SignInFailUserNotExist)
                {
                    (errorCode, uid) = await mGameService.SingUpWithNewUserGameData(request.user_id, request.service, token.RefreshToken);
                }
                
                if (errorCode != WebErrorCode.None || uid == 0)
                {
                    response.error_code = errorCode;
                    response.error_description = "";
                    return response;
                }

                //갱신 토큰 유지하기
                if (token.RefreshToken is null)
                {
                    (errorCode, var user) = await mGameService.GetUserInfo(uid);
                    if (errorCode != WebErrorCode.None || user == null)
                    {
                        response.error_code = WebErrorCode.TEMP_ERROR;
                        response.error_description = "";
                        return response;
                    }
                    token.RefreshToken = user.refresh_token;
                }

                if (token.ExpiresInSeconds is null)
                {
                    response.error_code = errorCode;
                    response.error_description = "";
                    return response;
                }

                //Redis에 토큰 저장
                errorCode = await mAuthenticationService.RegisterToken(uid, player.GamePlayerId, token.AccessToken, token.RefreshToken, token.ExpiresInSeconds.Value);
                if (errorCode != WebErrorCode.None)
                {
                    response.error_code = errorCode;
                    response.error_description = "";
                    return response;
                }

                //최근 로그인 시간 변경
                errorCode = await mAuthenticationService.UpdateLastSignInTime(uid);
                if (errorCode != WebErrorCode.None)
                {
                    response.error_code = errorCode;
                    response.error_description = "";
                    return response;
                }

                //유저 데이터 로드
                (errorCode, response.userData) = await mDataLoadService.LoadUserData(uid);
                if (errorCode != WebErrorCode.None)
                {
                    response.error_code = errorCode;
                    response.error_description = "";
                    return response;
                }

                response.error_code     = WebErrorCode.None;
                response.credential = new ClientCredential()
                {
                    uid = uid,
                    access_token = token.AccessToken,
                    expires_in = token.ExpiresInSeconds.Value,
                    scope = token.Scope,
                    token_type = token.TokenType,
                };

                return response;
            }
            catch (Exception ex)
            {
                string message = ($"[SignIn] error:{ex.ToString()}");
                Console.WriteLine(message);
                response.error_code = WebErrorCode.IsNotValidateServerCode;
                response.error_description = message;
                return response;
            }

        }

        /// <summary>
        /// 로그아웃
        /// </summary>
        [HttpPost]
        [Route("SignOut")]
        public async Task<SignOutRes> SignOut([FromHeader] HeaderDTO header, [FromBody] SignOutReq request)
        {
            Console.WriteLine($"[SignOut] uid:{header.uid} access:{header.access_token} cause:{request.cause}");

            var response = new SignOutRes();
            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            try
            {


                this.HttpContext.Items.TryGetValue(nameof(AuthUserDataInfo), out var userDataInfo);
                var user = userDataInfo as AuthUserDataInfo;
                if (user == null)
                {
                    response.error_code = WebErrorCode.TEMP_ERROR;
                    response.error_description = "";
                    return response;
                }

                //본 상태의 uid 얻어주기
                int uid = KeyUtils.GetUID(user.uid);

                //Redis의 Access Token과 Refresh Token 삭제
                var errorCode = await mAuthenticationService.RemoveToken(uid);
                if (errorCode != WebErrorCode.None)
                {
                    response.error_code = errorCode;
                    response.error_description = "";
                    return response;
                }

                //구글에서 토큰 Revoke하여 삭제하기
                errorCode = await mAuthenticationService.RevokeToken(user.user_id, user.access_token);
                if (errorCode != WebErrorCode.None)
                {
                    response.error_code = errorCode;
                    response.error_description = "";
                    return response;
                }

                response.error_code = WebErrorCode.None;
                return response;
            }
            catch (Exception ex)
            {
                string message = ($"[SignOut] error:{ex.ToString()}");
                Console.WriteLine(message);
                response.error_code = WebErrorCode.IsNotValidateServerCode;
                response.error_description = message;
                return response;
            }
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<RefreshTokenRes> RefreshToken([FromBody] RefreshTokenReq request)
        {

            Console.WriteLine($"[RefreshToken] uid:{request.uid}");

            var response = new RefreshTokenRes();
            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                return response;
            }

            try
            {

                //유저의 정보 가져오기
                var (error, user) = await mGameService.GetUserInfo(request.uid);
                if (error != WebErrorCode.None || user == null)
                {
                    response.error_code = WebErrorCode.TEMP_ERROR;
                    response.error_description = "";
                    return response;
                }


                //유저의 새로운 토큰 생성
                (error, var token) = await mAuthenticationService.RefreshToken(user.player_id, user.refresh_token);
                if (error != WebErrorCode.None || token == null || token.ExpiresInSeconds == null)
                {
                    return response;
                }

                response.error_code     = WebErrorCode.None;
                response.uid            = response.uid;
                response.access_token   = token.AccessToken;
                response.expires_in     = token.ExpiresInSeconds.Value;
                response.scope          = token.Scope;
                response.token_type     = token.TokenType;
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RefreshToken] error:{ex.ToString()}");
                response.error_code = WebErrorCode.IsNotValidateServerCode;
                response.error_description = ex.Message;
                return response;
            }

        }
    }
}
