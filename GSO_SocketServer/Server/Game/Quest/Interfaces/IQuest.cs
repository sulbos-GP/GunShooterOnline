using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.MasterDatabase;

namespace Server.Game.Quest.Interfaces
{



    public interface IQuest
    {
        public FUserRegisterQuest Register { get; }
        public FMasterQuestBase Data { get; }
        public PS_RegisterQuest Packet { get; }
        public void Update(Player owner, string tag, int amount);

    }
}
