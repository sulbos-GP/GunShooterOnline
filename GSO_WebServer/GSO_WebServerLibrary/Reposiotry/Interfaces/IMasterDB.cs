using WebCommonLibrary.DTO.Middleware;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDB;

namespace GSO_WebServerLibrary.Reposiotry.Interfaces
{
    public interface IMasterDB : IMasterDataDB
    {
        /// <summary>
        /// 초기 마스터 테이블 로드
        /// </summary>
        public Task<bool> LoadMasterDatabase();

        /// <summary>
        /// 초기 마스터 테이블 로드
        /// </summary>
        public Task<bool> LoadMasterTables();

        /// <summary>
        /// 가장 최근의 앱 버전을 불러온다
        /// </summary>
        /// 
        public Task<DB_Version> LoadLatestAppVersion();

        /// <summary>
        /// 가장 최근의 데이터 버전을 불러온다
        /// </summary>
        public Task<DB_Version> LoadLatestDataVersion();


    }
}
