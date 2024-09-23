using WebCommonLibrary.DTO.Middleware;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDatabase;
using WebCommonLibrary.Reposiotry.Interfaces;
using WebCommonLibrary.Reposiotry.MasterDatabase;

namespace GSO_WebServerLibrary.Reposiotry.Interfaces
{
    public interface IMasterDB
    {
        /// <summary>
        /// 데이터베이스 Context
        /// </summary>
        public MasterDatabaseContext Context { get; }

        /// <summary>
        /// 초기 마스터 테이블 로드
        /// </summary>
        public Task<bool> LoadMasterDatabase();

        /// <summary>
        /// 가장 최근의 앱 버전을 불러온다
        /// </summary>
        /// 
        public Task<FMasterVersionApp> LoadLatestAppVersion();

        /// <summary>
        /// 가장 최근의 데이터 버전을 불러온다
        /// </summary>
        public Task<FMasterVersionData> LoadLatestDataVersion();


    }
}
