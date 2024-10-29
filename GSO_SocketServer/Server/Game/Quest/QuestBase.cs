using Google.Protobuf.Protocol;
using Humanizer;
using Server.Database.Handler;
using Server.Game.Quest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.MasterDatabase;

namespace Server.Game.Quest
{
    public class QuestBase : IQuest
    {
        
        private readonly FUserRegisterQuest register;
        private readonly int initProgress;

        public QuestBase(FUserRegisterQuest quest)
        {
            this.register = quest;
            initProgress = quest.progress;
        }

        public FUserRegisterQuest Register
        {
            get
            {
                return register;
            }
        }

        public FMasterQuestBase Data
        {
            get
            {
                return DatabaseHandler.Context.MasterQuestBase.Find(Register.quest_id);
            }
        }

        public PS_RegisterQuest Packet
        {
            get
            {
                PS_RegisterQuest quest = new PS_RegisterQuest();
                quest.Id = Register.quest_id;
                quest.Progress = Register.progress;
                quest.Completed = Register.completed;
                return quest;
            }
        }

        public bool IsComplete()
        {
            return Register.completed;
        }

        public async void Update(Player owner, string tag, int amount)
        {

            if (owner == null)
            {
                Console.WriteLine("[QuestBase.Update] Owner가 존재하지 않음");
            }

            if (true == IsComplete())
            {
                return;
            }

            if (tag != Data.tag)
            {
                return;
            }

            Register.progress = Math.Clamp(Register.progress + amount, initProgress, Data.target);
            if (Register.progress == Data.target)
            {
                Register.completed = true;
            }
            else
            {
                Register.completed = false;
            }

            int result = await DatabaseHandler.GameDB.UpdateRegisterQuest(owner.UID, Register);
            if (result == 0)
            {
                Console.WriteLine($"[QuestBase.Update.UpdateRegisterQuest] {owner.UID}의 {Register.quest_id} quest를 update 하지 못함");
            }

            S_UpdateQuest packet = new S_UpdateQuest();
            packet.Quest = Packet;
            owner.Session.Send(packet);

        }
    }
}
