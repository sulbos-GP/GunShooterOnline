public class Define
{
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Shelter,
        loading,
        Forest,
        FieldOfViewTest,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount
    }

    public enum UIEvent
    {
        Click,
        Drag
    }
    
    public enum RewardsType
    {
        RandomItem = 0,
        LevelUp = 1,
        RandomSpawn = 2,
    }
    
}