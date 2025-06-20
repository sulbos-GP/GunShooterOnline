﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Quest.Interfaces
{
    public interface IQuestManager
    {
        void Attach(IQuest quest);
        void Detach(IQuest quest);
        void Notify();                   
    }
}
