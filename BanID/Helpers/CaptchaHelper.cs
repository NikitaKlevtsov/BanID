using System;

namespace BanID.Helpers
{
    public static class CaptchaHelper
    {
        private static Random rand = new Random();
        public static string GenerateCaptcha() => rand.Next(1000, 10000).ToString();
    }
}