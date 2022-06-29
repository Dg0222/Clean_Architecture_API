using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Services;

public class HubService : IHubService
{
    private readonly IHubContext<TestHub, ITestHub> _testHubClient;
    public HubService(IHubContext<TestHub, ITestHub> testHubClient)
    {
        _testHubClient = testHubClient ?? throw new ArgumentNullException(nameof(testHubClient));
    }

    public async Task SendMessageAsync(string message)
    {
        await _testHubClient.Clients.All.SendMessageAsync(message);
    }
}
