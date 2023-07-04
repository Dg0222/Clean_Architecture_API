using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Hubs;

[Authorize]
public class TestHub : Hub<ITestHub>
{
    //public async override Task OnConnectedAsync()
    //{

    //}

    public async Task SendMessageAsync(string message)
    {
        await Clients.All.SendMessageAsync(message);
    }
}
