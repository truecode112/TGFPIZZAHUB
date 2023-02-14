using Microsoft.AspNetCore.SignalR;

namespace TGFPIZZAHUB.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendData(string formattedData, string rawData)
        {
            await Clients.All.SendAsync("ReceiveMessage", formattedData, rawData);
        }
    }
}
