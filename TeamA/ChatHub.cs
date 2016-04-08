using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TeamA
{
    public class ChatHub : Hub
    {

        static ConcurrentDictionary<string, string> dictConn = new ConcurrentDictionary<string, string>();

        public void Send(string name, string message)
        {
            Clients.All.broadcastMessage(name, message);
        }

        public void SendToUser(string name, string message, string to)
        {
            Clients.Caller.broadcastPrivateMessage(name, message,to);
            Clients.Client(dictConn[to]).broadcastPrivateMessage(name, message,to);
        }
        
        public void Notify(string name,string id)
        {
            if (dictConn.ContainsKey(name))
            {
                Clients.Caller.differentName();
            }
            else
            {
                dictConn.TryAdd(name, id);
                foreach(KeyValuePair<string,string> usr in dictConn)
                {
                    Clients.Caller.online(usr.Key);
                }
                Clients.Others.enters(name);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var name = dictConn.FirstOrDefault(x => x.Value == Context.ConnectionId.ToString());
            string s;
            dictConn.TryRemove(name.Key, out s);
            return Clients.All.disconnected(name.Key);
        }


        #region oldcodev2

        /*
        #region Data Members
        static List<UserDetails> ConnectedUsers = new List<UserDetails>();
        static List<MessageDetails> CurrentMessage = new List<MessageDetails>();

        #endregion

        #region Methods

        public void Connect(string userName)
        {
            var id = Context.ConnectionId;

            userName = HttpContext.Current.Request.Cookies["Cookie"].Values["username"];
            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new UserDetails { ConnectionId = id, UserName = userName });

                // send to caller
                Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);

                // send to all except caller client
                Clients.AllExcept(id).onNewUserConnected(id, userName);

            }

        }

        public void SendMessageToAll(string userName, string message)
        {
            userName = HttpContext.Current.Request.Cookies["Cookie"].Values["username"];
            // store last 100 messages in cache
            AddMessageinCache(userName, message);

            // Broad cast message
            Clients.All.messageReceived(userName, message);
        }

        public void SendPrivateMessage(string toUserId, string message)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId) ;
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser!=null)
            {
                // send to 
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.UserName, message); 

                // send to caller user
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message); 
            }

        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserName);

            }

            return base.OnDisconnected(stopCalled);
        }

     
        #endregion

        #region private Messages

        private void AddMessageinCache(string userName, string message)
        {
            userName = HttpContext.Current.Request.Cookies["Cookie"].Values["username"];
            CurrentMessage.Add(new MessageDetails { UserName = userName, Message = message });

            if (CurrentMessage.Count > 100)
                CurrentMessage.RemoveAt(0);
        }

        #endregion
        */
        #endregion
        #region oldcode
        //private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        //public void SendChatMessage(string who, string message)
        //{
        //    string name = HttpContext.Current.Request.Cookies["Cookie"].Values["username"];

        //    foreach (var connectionId in _connections.GetConnections(who))
        //    {
        //        Clients.Client(connectionId).addChatMessage(name + ": " + message);
        //    }
        //}

        //public override Task OnConnected()
        //{
        //    string name = HttpContext.Current.Request.Cookies["Cookie"].Values["username"];
        //    _connections.Add(name, Context.ConnectionId);
        //    return base.OnConnected();
        //}

        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    string name = HttpContext.Current.Request.Cookies["Cookie"].Values["username"];
        //    _connections.Remove(name, Context.ConnectionId);
        //    return base.OnDisconnected(stopCalled);
        //}

        //public override Task OnReconnected()
        //{
        //    string name = HttpContext.Current.Request.Cookies["Cookie"].Values["username"];
        //    if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
        //    {
        //        _connections.Add(name, Context.ConnectionId);
        //    }
        //    return base.OnReconnected();
        //}


        //public void BroadcastMessage(string name, string msg)
        //{
        //    string mystring = HttpContext.Current.Request.Cookies["Cookie"].Values["username"];
        //    Clients.All.receiveMessage(mystring, msg);
        //}

        //public void GetUsers()
        //{

        //}
        #endregion
    }
}