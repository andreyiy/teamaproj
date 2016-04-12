using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using BusinessLayer.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;

namespace TeamA.SignalR
{
    //[Authorize]
    [HubName("messageHub")]
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> ChatUsers
            = new ConcurrentDictionary<string, string>();

        public void SendChatMessage(string who, string message)
        {
            
            Clients.All.addChatMessage(who, message);
            //Clients.All.broadcastMessage(name, message);
        }


        public void SendToSpecific(string to, string message)
        {
            string fromUserId = Context.ConnectionId;
            var toUser = ChatUsers.FirstOrDefault(x => x.Key == to);
            var fromUser = ChatUsers.FirstOrDefault(x => x.Value == fromUserId);

            if (toUser.Key !=null  && fromUser.Key != null)
            {
                // send to 
                Clients.Client(ChatUsers[toUser.Key]).sendPrivateMessage(toUser.Key, fromUser.Key, message);
                //Clients.Client(toUser.Key).openPrivateChatWindow(chat, fromUserId, fromUser.Key);
                // send to caller user
                Clients.Caller.sendPrivateMessage(toUser.Key, fromUser.Key, message);
            }
            //Clients.Caller.broadcastMessage(name, message);
            //Clients.Client(ChatUsers[to]).broadcastMessage(name, message);
        }

        public void Notify(string name, string id) 
        {
            if (ChatUsers.ContainsKey(name))
                {
                  Clients.Caller.differentName();
                }
              else
                {
                  ChatUsers.TryAdd(name, id);
                  foreach (KeyValuePair<String, String> entry in ChatUsers)
                  {
                     Clients.Caller.online(entry.Key,entry.Value);
                  }
                   Clients.Others.enters(name,id);
                }
         }



        public override Task OnDisconnected(bool stopCalled)
         {
             var name = ChatUsers.FirstOrDefault(x => x.Value == Context.ConnectionId.ToString());
             string s;
             if (name.Key != null)
             {
                 ChatUsers.TryRemove(name.Key, out s);
                 return Clients.All.disconnected(name.Key);
             }

             return base.OnDisconnected(stopCalled);
             //return null;
         }
    }
}