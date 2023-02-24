using Hanssens.Net;
using Microsoft.AspNetCore.SignalR;

namespace TGFPIZZAHUB.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendData(string formattedData, string rawData)
        {
            await Clients.All.SendAsync("ReceiveMessage", formattedData, rawData);
        }

        LocalStorage localStorage = new LocalStorage();

        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            string? orderSavedData = null;
            await base.OnConnectedAsync();

            try
            {
                orderSavedData = (string)localStorage.Get("Orders");
            }
            catch (Exception e)
            {
            }

            if (orderSavedData != null)
            {
                await Clients.Client(connectionId).SendAsync("OldMessages", orderSavedData);
            }
        }
    }
}
