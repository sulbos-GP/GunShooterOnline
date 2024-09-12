using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Models.GameDB;

public class WebModels
{
    private ClientCredential credential = new ClientCredential();
    private DataLoadUserInfo data = new DataLoadUserInfo();

    private void InputLog(string message)
    {
        var log = Managers.SystemLog;
        if (log == null)
        {
            return;
        }
        log.Message(message);
    }

    //임시 테스트 할떄만 사용할 예정
    private TModel GetModelOrDefaultNull<TModel>(string name, TModel model) where TModel : class
    {
        if (model == null)
        {
            InputLog($"[{name}] model is Null");
            return null;
        }

        foreach (var property in model.GetType().GetProperties())
        {

            var value = property.GetValue(model);

            //value가 null인지 확인
            if (value == null)
            {
                InputLog($"[{name}.{property.Name}] value is Null");
                return null;
            }

            //string의 경우 길이도 확인
            if (property.PropertyType == typeof(string) && string.IsNullOrEmpty(value as string))
            {
                InputLog($"[{name}.{property.Name}] string is Null or Empty");
                return null;
            }
        }

        return model;
    }

    public ClientCredential Credential
    {
        get
        {
            return GetModelOrDefaultNull("ClientCredential", credential);
        }
        set
        {
            credential = value;
        }
    }

    public DataLoadUserInfo Data
    {
        set
        {
            data = value;
        }
    }

    public UserInfo User
    {
        get
        {
            return GetModelOrDefaultNull("UserInfo", data.UserInfo);
        }
        set
        {
            data.UserInfo = value;
        }
    }

    public UserSkillInfo Rating
    {
        get
        {
            return GetModelOrDefaultNull("UserSkillInfo", data.SkillInfo);
        }
        set
        {
            data.SkillInfo = value;
        }
    }

    public UserMetadataInfo Metadata
    {
        get
        {
            return GetModelOrDefaultNull("UserMetadataInfo", data.MetadataInfo);
        }
        set
        {
            data.MetadataInfo = value;
        }
    }

    public List<UserLevelReward> LevelReward
    {
        get
        {
            return GetModelOrDefaultNull("UserLevelReward", data.LevelReward);
        }
        set
        {
            data.LevelReward = value;
        }
    }


}