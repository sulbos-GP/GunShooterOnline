using GsoWebServer.Models.GameDB;

namespace GsoWebServer.DTO.DataLoad
{
    public class UserDataLoadResponse : ErrorCodeDTO
    {
        public DataLoadUserInfo? UserData { get; set; } = null;
    }

    public class DataLoadUserInfo
    {
        public UserInfo? UserInfo { get; set; } = null;
        public UserSkillInfo? SkillInfo { get; set; } = null;
        public UserMetadataInfo? MetadataInfo { get; set; } = null;
    }
}
