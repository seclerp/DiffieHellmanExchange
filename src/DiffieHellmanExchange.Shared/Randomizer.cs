using System;
using System.Security.Cryptography;

namespace DiffieHellmanExchange.Shared
{
  public static class Randomizer
  {
    /// <summary>
    /// Real random generator
    /// Slower then Random().Next()!
    /// </summary>
    public static Int32 GetRandom(
      Int32 max)
    {
      return GetRandom(0, max);
    }

    /// <summary>
    /// Real random generator
    /// Slower than Random().Next()!
    /// </summary>
    public static Int32 GetRandom(
      Int32 min,
      Int32 max)
    {
      // Start a slower but more acurate randomizer service
      RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
      Byte[] randomBytes = new Byte[4];
      rngCryptoServiceProvider.GetBytes(randomBytes);
      Int32 seed = BitConverter.ToInt32(randomBytes, 0);

      return new Random(seed).Next(min, max);
    }
  }
}