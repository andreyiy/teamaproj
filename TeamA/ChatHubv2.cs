using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TeamA
{
    public class ChatHubv2 : Hub
    {
        static ConcurrentDictionary<UserDetails, string> dictConn = new ConcurrentDictionary<UserDetails, string>();
        static List<MessageDetails> CurrentMessage = new List<MessageDetails>();
        static List<PrivateMessageDetails> PrivateMessage = new List<PrivateMessageDetails>();

        public void Send(string groupname, string name, string message)
        {
            AddMessageinCache(name, message, groupname);
            Clients.Group(groupname).broadcastMessage(name, message);
        }

        public void SendToUser(string name, string message, string to)
        {
            UserDetails usr = new UserDetails();
            var temp = dictConn.FirstOrDefault(x => x.Key.UserName == to);
            usr = temp.Key;
            AddPrivateMessageinCache(name, to, message);
            Clients.Caller.broadcastPrivateMessage(name, message, to);
            Clients.Client(dictConn[usr]).broadcastPrivateMessage(name, message, to);
        }

        public void Notify(string name, string id, string groupName)
        {
            UserDetails usr = new UserDetails();
            usr.UserName = name;
            usr.RoomName = groupName;
            if (dictConn.ContainsKey(usr))
            {
                Clients.Caller.differentName();
            }
            else
            {
                dictConn.TryAdd(usr, id);
                foreach (KeyValuePair<UserDetails, string> tryUser in dictConn)
                {
                    if (tryUser.Key.RoomName == groupName)
                    {
                        Clients.Caller.online(tryUser.Key.UserName);
                    }
                }
                Clients.Caller.retrievePrivateMessage(PrivateMessage);
                Clients.Caller.retrieveMessages(CurrentMessage);
                Clients.Group(groupName).enters(name);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var name = dictConn.FirstOrDefault(x => x.Value == Context.ConnectionId.ToString());
            string s;
            try
            {
                dictConn.TryRemove(name.Key, out s);
                LeaveRoom(name.Key.RoomName);
            }
            catch
            {
                return Clients.Group(name.Key.RoomName).disconnected(name.Key.UserName);
            }
            return Clients.Group(name.Key.RoomName).disconnected(name.Key.UserName);
        }

        public Task JoinRoom(string roomName)
        {
            return Groups.Add(Context.ConnectionId, roomName);
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.Remove(Context.ConnectionId, roomName);
        }

        private void AddMessageinCache(string userName, string message, string groupName)
        {
            CurrentMessage.Add(new MessageDetails { UserName = userName, Message = message, RoomName = groupName });

            if (CurrentMessage.Count > 300)
                CurrentMessage.RemoveAt(0);
        }

        private void AddPrivateMessageinCache(string from, string to, string message)
        {
            PrivateMessage.Add(new PrivateMessageDetails { FromUser = from, ToUser = to, Message = message });
            if (PrivateMessage.Count > 200)
                PrivateMessage.RemoveAt(0);
        }
    }
}