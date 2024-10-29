using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum EEventBusType
    {
        None,
        Combat,
        Collect,
        Play,
    }

    public static class EventBus
    {
        
        private static Dictionary<EEventBusType, Action<Player, object>> events = new Dictionary<EEventBusType, Action<Player, object>>();

        /// <summary>
        /// 이벤트 버스 구독
        /// </summary>
        public static void Subscribe(EEventBusType type, Action<Player, object> listener)
        {
            if (!events.ContainsKey(type))
            {
                events[type] = delegate { };
            }

            events[type] += listener;
        }

        /// <summary>
        /// 이벤트 버스 해제
        /// </summary>
        public static void Unsubscribe(EEventBusType type, Action<Player, object> listener)
        {
            if (events.ContainsKey(type))
            {
                events[type] -= listener;
            }
        }

        /// <summary>
        /// 이벤트 발생
        /// </summary>
        public static void Publish(EEventBusType type, Player player, object data = null)
        {
            if (events.ContainsKey(type))
            {
                events[type]?.Invoke(player, data);
            }

        }
    }
}
