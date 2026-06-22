using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;


namespace SecureOPS.Applications.Services
{
    public class NotificationHub : Hub
    {
		public override async Task OnConnectedAsync()
        {
            // Get the role from the connection context (usually from a JWT claim)
            var userRole = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(userRole))
            {
                // Add this specific connection to a group named after the role
                // e.g., anyone with the "L2 Analyst" role joins the "L2 Analyst" group
                await Groups.AddToGroupAsync(Context.ConnectionId, userRole);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userRole = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(userRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userRole);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}