namespace WebCommonLibrary.Error
{
    public enum WebErrorCode
    {
        None = 0,

        //TEMP ERROR
        TEMP_ERROR = 1,
        TEMP_Exception = 2,

        //Middleware
        IncorrectHeaderContext = 500,
        InvalidVersion = 501,
        DiscrepancyAppVersion = 502,
        DiscrepancyDataVersion = 503,
        AccessTokenIsExpries = 504,
        FailedRefreshToken = 505,
        InValidAppVersion = 506,


        // Common 1000 ~
        UnhandleException = 1001,
        RedisFailException = 1002,
        InValidRequestHttpBody = 1003,
        AuthTokenFailWrongAuthToken = 1006,
        IsNotValidModelState = 1500,

        // Account 2000 ~
        CreateAccountFailException = 2001,
        LoginFailException = 2002,
        LoginFailUserNotExist = 2003,
        LoginFailServiceNotMatch = 2004,
        LoginFailSetAuthToken = 2005,
        AuthTokenMismatch = 2006,
        AuthTokenNotFound = 2007,
        AuthTokenFailWrongKeyword = 2008,
        AuthTokenFailSetNx = 2009,
        AccountIdMismatch = 2010,
        DuplicatedLogin = 2011,
        CreateAccountFailInsert = 2012,
        LoginFailAddRedis = 2014,
        CheckAuthFailNotExist = 2015,
        CheckAuthFailNotMatch = 2016,
        CheckAuthFailException = 2017,

        TicketAlreadyMax = 2018,
        TicketRemainingTime = 2019,

        DailyTaskIsAllocate = 2020,
        TicketFailedUpdateCount = 2021,
        TicketFailedUpdateLastTime = 2022,

        DailyQuestInvalidList = 2300,
        DailyQuestNotMatch = 2301,
        DailyQuestNotEnough = 2302,
        DailyQuestAlreadyComplelte = 2303,
        DailyQuestFailedRandom = 2304,
        DailyQuestFailedInsert = 2305,


        // Authentication 2500 ~
        ServerCodeNotFound = 2501,

        IsNotValidateServerCode,

        MyPlayerIdMismatch,
        MyPlayerFailException,

        SignInFailUserNotExist,
        SignInFailMismatchService,

        SetNicknameInitNickname,
        SetNicknameFailSameNickname,

        // Character 3000 ~
        CreateCharacterRollbackFail = 3001,
        CreateCharacterFailNoSlot = 3002,
        CreateCharacterFailException = 3003,
        CharacterNotExist = 3004,
        CountCharactersFail = 3005,
        DeleteCharacterFail = 3006,
        GetCharacterInfoFail = 3007,
        InvalidCharacterInfo = 3008,
        GetCharacterItemsFail = 3009,
        CharacterCountOver = 3010,
        CharacterArmorTypeMisMatch = 3011,
        CharacterHelmetTypeMisMatch = 3012,
        CharacterCloakTypeMisMatch = 3012,
        CharacterDressTypeMisMatch = 3013,
        CharacterPantsTypeMisMatch = 3012,
        CharacterMustacheTypeMisMatch = 3012,
        CharacterArmorCodeMisMatch = 3013,
        CharacterHelmetCodeMisMatch = 3014,
        CharacterCloakCodeMisMatch = 3015,
        CharacterDressCodeMisMatch = 3016,
        CharacterPantsCodeMisMatch = 3017,
        CharacterMustacheCodeMisMatch = 3018,
        CharacterHairCodeMisMatch = 3019,
        CharacterCheckCodeError = 3010,
        CharacterLookTypeError = 3011,

        CharacterStatusChangeFail = 3012,
        CharacterIsExistGame = 3013,
        GetCharacterListFail = 3014,

        // Matchmaking 4000 ~
        PushPlayerSkillIsExist = 4000,
        PushPlayerSkillFailException,
        PushPlayerNoTicket,                 //참여할 티켓이 없음

        PopPlayersExitRequested,            //Lock이 걸려있어 우선 매칭 큐에서 빠져나가기 요청
        PopPlayersExitSuccess,              //매칭 큐에서 빠져나가는데 성공함
        PopPlayersJoinForced,               //매칭 큐에서 빠져나오지 못하고 게임에 진입함

        FindMatchFailFindPossibleOpponents,
        FindMatchToFailException,

        ScanPlayersToFailException,
        RemoveMatchPlayersFailException,
        MatchmakingFailToRemovePlayer,

        UpdatePlayerToFailException,

        // GameServerManager 5000 ~
        EmptySession = 5001,
        NoPendingSession = 5002,

        //GameDb 10000~ 
        GetGameDbConnectionFail = 10001,

        //Redis 11000~ 
        GetRedisConnectionFail = 11001,
    }
}
