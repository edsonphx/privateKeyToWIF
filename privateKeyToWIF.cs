using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

public class Program
{
    private static Random _random = new Random();

    public static void Main()
    {
        new Program().Start();
    }
    public void Start()
    {
        var privateKey = CreatePrivateKey();

        var WIF = CreateWIF(privateKey);

    }
    public IEnumerable<byte> CreatePrivateKey()
    {
        var privateKey = new byte[32];
        _random.NextBytes(privateKey);

        return privateKey;
    }
    public string CreateWIF(IEnumerable<byte> privateKey)
    {
        var WIF = new List<byte>();
        WIF.Add(0x80);

        WIF.AddRange(privateKey);

        var checkSun = GetCheckSun(WIF);
        WIF.AddRange(checkSun);

        var base58WIF = ConvertToBase58(WIF);

        return base58WIF;
    }
    private IEnumerable<byte> GetCheckSun(IEnumerable<byte> WIF)
    {
        using (var hashCreate = SHA256.Create())
        {
            var firstHash = hashCreate.ComputeHash(WIF.ToArray());
            return hashCreate.ComputeHash(firstHash).ToList().Take(4);
        }
    }

    private string ConvertToBase58(IEnumerable<byte> bytes)
    {
        var bytesRepresentation = new BigInteger(0);
        int i = 0;

        var bytesReverse = bytes.Reverse<byte>();

        foreach (var b in bytesReverse)
        {
            var power = i * 8;
            bytesRepresentation += b * BigInteger.Pow(2, power);
            i++;
        }

        var valueToMap = GetValueFromRepresentation(bytesRepresentation);

        var mappedValue = MapValues(valueToMap);
        return mappedValue;
    }
    private IEnumerable<int> GetValueFromRepresentation(BigInteger bytesRepresentation)
    {
        var returnList = new List<int>();

        while (bytesRepresentation >= 58)
        {
            bytesRepresentation = BigInteger.DivRem(bytesRepresentation, 58, out BigInteger remainer);

            returnList.Add((int)remainer);
        }

        returnList.Add((int)bytesRepresentation);

        returnList.Reverse();
        return returnList;
    }
    private string MapValues(IEnumerable<int> valuesToMap)
    {
        string result = null;
        var map = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToArray();

        foreach (var i in valuesToMap)
        {
            result += map[i];
        }

        return result;
    }
}
