using FinancialBot.BL.DTOs;
using FinancialBot.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialBot.Services.Hubs
{
    [Authorize]
    public class HubChatServices : Hub
    {
        private readonly IMqtMessageSender _qtt;
        private static IList<HubUsers> _connectedUsers = new List<HubUsers>();
        private static IList<UsersMessages> _currentMessages = new List<UsersMessages>();

        public HubChatServices(IMqtMessageSender sender)
        {
            _qtt = sender;
        }

        private void AddToCurrentMessages(UsersMessages message)
        {
            _currentMessages.Add(message);

            if (_currentMessages.Count() > 50)
                _currentMessages.RemoveAt(0);
        }

        public override Task OnConnectedAsync()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            if (!_connectedUsers.Any(connectedUser => connectedUser.UserName == userName))
            {
                _connectedUsers.Add(new HubUsers { ConnectionId = connectionId, UserName = userName });
                Clients.All.SendAsync("ConnectedUsersChanged", _connectedUsers);
            }
            else
            {
                Clients.Caller.SendAsync("ConnectedUsersChanged", _connectedUsers);
            }

            Clients.Caller.SendAsync("CurrentMessages", _currentMessages);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(UsersMessages message)
        {
            await Clients.All.SendAsync("NewMessage", message);
            AddToCurrentMessages(message);

            if (message.Message.Contains("/stock="))
            {
                _qtt.SendMessage(message);
            }
        }

        public void SaveBotMessage(UsersMessages message)
        {
            AddToCurrentMessages(message);
        }

        public async Task DisconnectUser(string userName)
        {
            if (_connectedUsers.Any(currentUser => currentUser.UserName == userName))
            {
                _connectedUsers = _connectedUsers.Where(currentUser => currentUser.UserName != userName).ToList();
                await Clients.All.SendAsync("ConnectedUsersChanged", _connectedUsers);
            }
        }
    }
}
