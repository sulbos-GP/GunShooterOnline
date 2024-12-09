using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

public class WebModels
{
    private ClientCredential credential = new ClientCredential();
    private DataLoadUserInfo data = new DataLoadUserInfo();
    private DailyLoadInfo dailyData = new DailyLoadInfo();
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

            if (property.PropertyType == typeof(int) && property.Name == "uid" && (int)value == 0)
            {
                return null;
            }

            //value가 null타입 인지 확인
            bool isNullable = Nullable.GetUnderlyingType(property.PropertyType) != null;

            //value가 null인 경우
            if (!isNullable && value == null)
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

    public FUser User
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

    public FUserSkill Rating
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

    public FUserMetadata Metadata
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

    public List<FUserLevelReward> LevelReward
    {
        get
        {
            foreach (var item in data.LevelReward)
            {
                if(null == GetModelOrDefaultNull("UserLevelReward", item))
                {
                    return null;
                }
            }
            return data.LevelReward;
        }
        set
        {
            data.LevelReward = value;
        }
    }

    public List<DB_GearUnit> Gear
    {
        get
        {
            return data.gears;
        }
        set
        {
            data.gears = value;
        }
    }

    public List<DB_ItemUnit> Inventory
    {
        get
        {
            return data.items;
        }
        set
        {
            data.items = value;
        }
    }


    public DailyLoadInfo DailyData
    {
        set
        {
            dailyData = value;
        }
    }

    public List<FUserRegisterQuest> DailyQuestData
    {
        get
        {
            foreach (var quest in dailyData.DailyQuset)
            {
                if (null == GetModelOrDefaultNull("DailyQuest", quest))
                {
                    return null;
                }
            }
            return dailyData.DailyQuset;
        }
        set
        {
            dailyData.DailyQuset = value;
        }
    }
}