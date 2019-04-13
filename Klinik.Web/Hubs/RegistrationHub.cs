using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Klinik.Web.Hubs
{
    public class RegistrationHub : Hub
    {
        [HubMethodName("NotifyClients")]
        public static void BroadcastDataToAllClients()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<RegistrationHub>();
            context.Clients.All.updatedClients();
        }
    }
}