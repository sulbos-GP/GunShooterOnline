using Server.Database.Handler;
using Server.Game.Quest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Quest
{
    public class Quests
    {
        private Player owner;
        private List<IQuest> quests = new List<IQuest>();

        public Quests(Player owner)
        {
            this.owner = owner;
            LoadRegisterQuest().Wait();
        }

        public List<IQuest> QuestList
        {
            get
            {
                return quests;
            }
        }

        private async Task LoadRegisterQuest()
        {
            try
            {
                var questList = await DatabaseHandler.GameDB.LoadRegisterQuest(owner.UID);

                Console.WriteLine($"[LoadRegisterQuest]");
                Console.WriteLine("{");
                foreach (var quest in questList)
                {
                    QuestBase questBase = new QuestBase(quest);
                    Console.WriteLine($"\t player:{quest.uid} quest:{quest.quest_id} progress:{quest.progress} complete:{quest.completed}");
                    quests.Add(questBase);
                }
                Console.WriteLine("}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RegisterQuests.LoadRegisterQuest] {ex.ToString()}");
            }

        }
    }
}
