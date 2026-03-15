using Microsoft.AspNetCore.Http;
using System;

namespace _2FA.Tests.Helpers
{
    public static class TestSessionExtensions
    {
        public static void SetInt32(this ISession session, string key, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            session.Set(key, bytes);
        }

        public static int? GetInt32(this ISession session, string key)
        {
            if (session.TryGetValue(key, out var bytes) && bytes != null && bytes.Length == 4)
            {
                return BitConverter.ToInt32(bytes, 0);
            }
            return null;
        }
    }
}
