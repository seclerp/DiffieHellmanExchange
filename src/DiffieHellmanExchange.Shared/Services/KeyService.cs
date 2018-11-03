using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DiffieHellmanExchange.Shared.Services
{
  public class KeyService
  {
    private IReadOnlyCollection<int> _primes = new List<int>();
    private Random _random = new Random();

    public void GeneratePrimeTable() => _primes = GetPrimes(100000).ToList();

    private IEnumerable<int> GetPrimes(int bound)
    {
      if (bound < 2) yield break;
      yield return 2;

      var composite = new BitArray((bound - 1) / 2);
      var limit = ((int) (Math.Sqrt(bound)) - 1) / 2;
      for (var i = 0; i < limit; i++)
      {
        if (composite[i]) continue;
        int prime = 2 * i + 3;
        yield return prime;
        for (var j = (prime * prime - 2) >> 1; j < composite.Count; j += prime)
        {
          composite[j] = true;
        }
      }

      for (var i = limit; i < composite.Count; i++)
      {
        if (!composite[i]) yield return 2 * i + 3;
      }
    }

    private int PrimitiveRoot(long module)
    {
      var pairs = new Dictionary<BigInteger, bool>();

      for (long g = 2; g < module; g++)
      {
        var bigInteger = new BigInteger(g);

        for (long power = 0; power < module; power++)
        {
          var res = BigInteger.ModPow(bigInteger, power, module);

          if (res > 0 && res < module)
          {
            if (pairs.ContainsKey(res)) break;
            pairs.Add(res, true);
          }
        }

        if (pairs.Count == module - 1)
          return (int) bigInteger;

        pairs.Clear();
      }

      return 5;
    }

    public int GetQ() => _primes.ElementAt(Randomizer.GetRandom(1, _primes.Count));

    public int GetA(int q) => PrimitiveRoot(q);

    public int GetX(int q) => _random.Next(1, q);

    public BigInteger GetY(int x, int q, int a) => BigInteger.ModPow(a, x, q);

    public BigInteger GetKey(BigInteger y, int x, int q) => BigInteger.ModPow(y, x, q);
  }
}