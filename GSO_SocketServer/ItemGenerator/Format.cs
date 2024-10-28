using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemGenerator
{
    internal class Format
    {

        #region Item

        // {0} 아이템 id
        public static string ItemCoolRegister =
            @" _handler.Add({0}, cool{0});";

        // {0} 아이템 id
        public static string ItemCoolDeclaration =
            @"short cool{0}= 0;";

        // {0} 아이템 id
        public static string ManagerHandlerRegister =
            @"_handler.Add({0}, ItemHandler.HandleItem{0});";

        #endregion
    }
}
