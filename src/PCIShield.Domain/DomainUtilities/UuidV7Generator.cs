using System;
using System.Buffers.Binary;
using System.Security.Cryptography;
namespace PCIShield.Domain.Entities
{
    public static class UuidV7Generator
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        private static readonly object Lock = new object();
        private static long lastTimestamp = 0;
        private static byte[] lastRandom = new byte[10];
        public static Guid NewUuidV7()
        {
            return Guid.CreateVersion7();
            Span<byte> uuidBytes = stackalloc byte[16];
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
            randomBytes.AsSpan(1).CopyTo(uuidBytes.Slice(9));
            return new Guid(uuidBytes);
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
}