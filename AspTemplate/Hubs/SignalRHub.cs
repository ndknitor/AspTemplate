using Microsoft.AspNetCore.SignalR;

public class SignalRHub : Hub
{
    private readonly ILogger<SignalRHub> logger;
    public SignalRHub(ILogger<SignalRHub> logger)
    {
        this.logger = logger;
    }
    public async Task ClientMessage(string message)
    {
        int a = int.Parse("a");
        await Clients.All.SendAsync("ServerMessage", message);
    }

}