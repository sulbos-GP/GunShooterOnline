using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

//아고네스 텍스트 커멘드
//https://github.com/googleforgames/agones/tree/release-1.43.0/examples/simple-game-server

namespace WebCommonLibrary.Enum
{
    public enum EGameServerCommand
    {
        [Description("None")]
        None,

        [Description("Causes the game server to exit cleanly calling os.Exit(0)")]
        EXIT,

        [Description("Stopping sending health checks")]
        UNHEALTHY,

        [Description("Sends back the game server name")]
        GAMESERVER,

        [Description("Marks the server as Ready")]
        READY,

        [Description("Allocates the game server")]
        ALLOCATE,

        [Description("Reserves the game server after the specified duration")]
        RESERVE,

        [Description("Instructs the game server to log changes to the resource")]
        WATCH,

        [Description("Sets the specified label on the game server resource")]
        LABEL,

        [Description("Causes the game server to exit / crash immediately")]
        CRASH,

        [Description("Sets the specified annotation on the game server resource")]
        ANNOTATION,

        [Description("Gets or sets the player capacity (with one or two arguments)")]
        PLAYER_CAPACITY,

        [Description("Connects the specified player to the game server")]
        PLAYER_CONNECT,

        [Description("Disconnects the specified player from the game server")]
        PLAYER_DISCONNECT,

        [Description("Returns true/false depending on whether the specified player is connected")]
        PLAYER_CONNECTED,

        [Description("Returns a list of the connected players")]
        GET_PLAYERS,

        [Description("Returns a count of the connected players")]
        PLAYER_COUNT,

        [Description("Returns a count of a given Counter")]
        GET_COUNTER_COUNT,

        [Description("Increases the count of the given Counter by the given nonnegative integer amount")]
        INCREMENT_COUNTER,

        [Description("Decreases the count of the given Counter by the given nonnegative integer amount")]
        DECREMENT_COUNTER,

        [Description("Sets a count of the given Counter to the given amount")]
        SET_COUNTER_COUNT,

        [Description("Returns the Capacity of the given Counter")]
        GET_COUNTER_CAPACITY,

        [Description("Sets the Capacity of the given Counter to the given amount")]
        SET_COUNTER_CAPACITY,

        [Description("Returns the Capacity of the given List")]
        GET_LIST_CAPACITY,

        [Description("Sets the List to a new Capacity successfully (true) or not (false)")]
        SET_LIST_CAPACITY,

        [Description("Returns true if the given value is in the given List, false otherwise")]
        LIST_CONTAINS,

        [Description("Returns the length (number of values) of the given List as a string")]
        GET_LIST_LENGTH,

        [Description("Returns the values in the given List as a comma delineated string")]
        GET_LIST_VALUES,

        [Description("Returns true if the given value was successfully added to the List (true) or not (false)")]
        APPEND_LIST_VALUE,

        [Description("Returns true if the given value was successfully deleted from the List (true) or not (false)")]
        DELETE_LIST_VALUE
    }
}
