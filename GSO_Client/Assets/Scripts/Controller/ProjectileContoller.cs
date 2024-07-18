using Google.Protobuf.Protocol;

public class ProjectileContoller : BaseController
{
    public int SkillId { get; set; }


    protected override void Init()
    {
        State = CreatureState.Moving;

        base.Init();
    }
}