using Server.Database.Handler;
using Server.Game.Quest.Interfaces;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Models.GameDatabase;

namespace Server.Game.Quest
{
    public sealed class QuestSystem
    {

        private static readonly QuestSystem instance = new QuestSystem();

        public QuestSystem()
        {

        }

        public static QuestSystem Instance
        {
            get
            {
                return instance;
            }
        }

        public void Initialize()
        {
            EventBus.Subscribe(EEventBusType.Combat, this.OnCombat);
            EventBus.Subscribe(EEventBusType.Collect, this.OnCollect);
            EventBus.Subscribe(EEventBusType.Play, this.OnPlay);
        }

        private void OnCombat(Player player, object data)
        {
            CreatureObj creature = data as CreatureObj;
            string tag = "PLAYER"; //임시
            ProcessUpdateQuest(player, "전투", tag, 1);
        }

        private void OnCollect(Player player, object data)
        {
            ItemObject item = data as ItemObject;
            string tag = item.Data.code;
            int amount = item.Data.amount;
            ProcessUpdateQuest(player, "보급", tag, amount);
        }

        private void OnPlay(Player player, object data)
        {
            string tag = data as string;
            ProcessUpdateQuest(player, "플레이", tag, 1);
        }
 
        private void ProcessUpdateQuest(Player player, string category, string tag, int amount)
        {
            foreach (IQuest quest in player.Quest.QuestList)
            {
                if(quest.Data.category == category)
                {
                    quest.Update(player, tag, amount);
                }
            }
        }


    }
}
