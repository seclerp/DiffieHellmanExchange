using System;
using System.Collections.Generic;
using System.Numerics;

namespace DiffieHellmanExchange.Shared
{
  public class Diff
  {
    private Random _random;

    public Diff()
    {
      _random = new Random();
    }

    public int Nod(int a, int b)
    {
      while (a != 0 && b != 0)
      {
        if (a > b)
        {
          a = a % b;
        }
        else
        {
          b = b % a;
        }
      }
      
      return a == 0 ? b : a;
    }

    public bool IsPrime(int number)
    {
      if (number < 2)
      {
        return false;
      }

      var value = Math.Sqrt(number);
      for (var i = 2; i < value; i++)
      {
        if (number % i == 0)
        {
          return false;
        }
      }

      return true;
    }

    BigInteger PrimitiveRoot(long module)
    {
      Dictionary<BigInteger, bool> pairs = new Dictionary<BigInteger, bool>();

      for (long g = 2; g < module; g++)
      {
        BigInteger bigInteger = new BigInteger(g);

        for (long power = 0; power < module; power++)
        {
          BigInteger res = BigInteger.ModPow(bigInteger, power, module);

          if (res > 0 && res < module)
          {
            if (pairs.ContainsKey(res)) break;
            pairs.Add(res, true);
          }
        }

        if (pairs.Count == module - 1)
          return bigInteger;

        pairs.Clear();
      }

      return 5;
    }

    public int PrimeNumber(int min)
    {
      for (var i = min + 20; i < min * 10; i++)
      {
        if (IsPrime(i))
        {
          return i;
        }
      }
      
      throw new NotImplementedException();
    }

    public BigInteger CreateA(int g, int a, int module)
    {
      return BigInteger.ModPow(new BigInteger(g), a, module);
    }
    
    public BigInteger CreateKey(int b, int a, int module)
    {
      return BigInteger.ModPow(new BigInteger(b), a, module);
    }
  }
}