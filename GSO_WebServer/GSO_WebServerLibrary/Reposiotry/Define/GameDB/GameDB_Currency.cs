using GSO_WebServerLibrary;
using WebCommonLibrary.Models.GameDB;
using Google.Apis.Games.v1.Data;
using SqlKata.Execution;
using static Google.Apis.Requests.RequestError;
using System.Transactions;
using System.Data;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using WebCommonLibrary.Models.MasterDB;
using static Humanizer.In;

namespace GSO_WebServerLibrary.Reposiotry.Define.GameDB
{
    public partial class GameDB : IGameDB
    {

        public async Task<int> UpdateCurrency(int uid, int money, int ticket, int gacha, IDbTransaction? transaction = null)
        {
            return await mQueryFactory.Query("user")
                .Where("uid", uid)
                .UpdateAsync(new
                {
                    money = money,
                    ticket = ticket,
                    gacha = gacha,
                }, transaction);
        }

        public async Task<int> UpdateMoney(int uid, int money, IDbTransaction? transaction = null)
        {
            return await mQueryFactory.Query("user")
                .Where("uid", uid)
                .UpdateAsync(new
                {
                    money = money,
                }, transaction);
        }


        public async Task<int> UpdateTicket(int uid, int ticket, IDbTransaction? transaction = null)
        {
            return await mQueryFactory.Query("user")
                .Where("uid", uid)
                        .UpdateAsync(new
                {
                    ticket = ticket,
                }, transaction);
        }


        public async Task<int> UpdateGacha(int uid, int gacha, IDbTransaction? transaction = null)
        {
            return await mQueryFactory.Query("user")
                .Where("uid", uid)
                .UpdateAsync(new
                {
                    gacha = gacha,
                }, transaction);
        }
    }
}
