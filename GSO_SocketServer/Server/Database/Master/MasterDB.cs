﻿using Server.Game;
using SqlKata.Execution;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Reposiotry.MasterDatabase;

namespace Server.Database.Master
{
    public partial class MasterDB : MySQL
    {

        public MasterDB() : base()
        {

        }

        public async Task<IEnumerable<T>> LoadTable<T>(string table)
        {
            var query = this.GetQueryFactory();
            return await query.Query(table).GetAsync<T>();
        }

    }
}
