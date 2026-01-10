using System.Buffers.Binary;
using System.Security.Cryptography;
namespace PCIShield.Domain.ModelsDto;
public static class UuidV7GeneratorV2
{
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
    private static readonly object Lock = new object();
    private static long lastTimestamp = 0;
    private static byte[] lastRandom = new byte[10];
    public static Guid NewUuidV7()
    {
        return Guid.CreateVersion7();
        byte[] uuidBytes = new byte[16];
        long timestamp;
        byte[] randomBytes = new byte[10];
        lock (Lock)
        {
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (timestamp <= lastTimestamp)
            {
                timestamp = lastTimestamp;
                IncrementRandomPortion(lastRandom);
                randomBytes = lastRandom;
            }
            else
            {
                Rng.GetBytes(randomBytes);
                lastTimestamp = timestamp;
                lastRandom = randomBytes;
            }
        }
        BinaryPrimitives.WriteInt64BigEndian(uuidBytes, timestamp);
        uuidBytes[6] = (byte)((uuidBytes[6] & 0x0F) | 0x70);
        uuidBytes[8] = (byte)((randomBytes[0] & 0x3F) | 0x80);
        Array.Copy(randomBytes, 1, uuidBytes, 9, 7);
        return new Guid(
            BitConverter.ToInt32(new[] { uuidBytes[3], uuidBytes[2], uuidBytes[1], uuidBytes[0] }, 0),
            BitConverter.ToInt16(new[] { uuidBytes[5], uuidBytes[4] }, 0),
            BitConverter.ToInt16(new[] { uuidBytes[7], uuidBytes[6] }, 0),
            uuidBytes[8],
            uuidBytes[9],
            uuidBytes[10],
            uuidBytes[11],
            uuidBytes[12],
            uuidBytes[13],
            uuidBytes[14],
            uuidBytes[15]
        );
    }
    private static void IncrementRandomPortion(byte[] randomBytes)
    {
        for (int i = randomBytes.Length - 1; i >= 0; i--)
        {
            if (randomBytes[i] < 255)
            {
                randomBytes[i]++;
                break;
            }
            randomBytes[i] = 0;
        }
    }
}