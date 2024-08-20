using Server.Database.Interface;
using System.Threading.Tasks;

namespace Server.Database.Game
{
    ///////////////////////////////////////////////
    ///                                         ///
    ///               INVENTORY                 ///
    ///                                         ///
    ///////////////////////////////////////////////

    public partial class GameDB : MySQL, IGameDatabase
    {
        public async Task InsertItem()
        {
            var query = this.GetQueryFactory();

        }

        public async Task DeleteItem()
        {
            var query = this.GetQueryFactory();

        }

        public async Task MoveItem()
        {
            var query = this.GetQueryFactory();

        }
    }
}
