using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using DiffieHellmanExchange.Shared.Services;
using Microsoft.AspNetCore.SignalR;

namespace DiffieHellmanExchange.Hubs
{
  public class MainHub : Hub
  {
    private static readonly ConcurrentDictionary<string, (int Q, int A)> _aqPerClient;
    private static readonly ConcurrentDictionary<string, (int Xs, BigInteger Ys)> _xyPerClient;
    private static readonly ConcurrentDictionary<string, BigInteger> _keysPerClient;
    private static readonly KeyService _keyService;

    static MainHub()
    {
      _aqPerClient = new ConcurrentDictionary<string, (int Q, int A)>();
      _xyPerClient = new ConcurrentDictionary<string, (int Xs, BigInteger Ys)>();
      _keysPerClient = new ConcurrentDictionary<string, BigInteger>();
      _keyService = new KeyService();
      _keyService.GeneratePrimeTable();
    }
    
    public async Task GetAQY()
    {
      var q = _keyService.GetQ();
      var a = _keyService.GetA(q);

      _aqPerClient[Context.ConnectionId] = (Q : q, A : a);

      var x = _keyService.GetX(q);
      var y = _keyService.GetY(x, q, a);
      
      _xyPerClient[Context.ConnectionId] = (Xs : x, Ys : y);
      
      await Clients.Caller.SendAsync("RecieveQAY", q, a, y);
    }

    public async Task RecieveY(BigInteger y)
    {
      var xs = _xyPerClient[Context.ConnectionId].Xs;
      var key = _keyService.GetKey(y, xs, _aqPerClient[Context.ConnectionId].Q);

      _keysPerClient[Context.ConnectionId] = key;
      Console.WriteLine($"Generated key for client '{Context.ConnectionId}' is {key}");
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
      _aqPerClient.Remove(Context.ConnectionId, out _);
      _xyPerClient.Remove(Context.ConnectionId, out _);
      _keysPerClient.Remove(Context.ConnectionId, out _);
      
      return base.OnDisconnectedAsync(exception);
    }
  }
}