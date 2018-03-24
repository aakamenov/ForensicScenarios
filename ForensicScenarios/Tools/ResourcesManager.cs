using System;

namespace ForensicScenarios.Tools
{
    public static class ResourcesManager
    {
        public static string[] Passwords { get; }

        public static string[] Words { get; }

        private static Random random;

        static ResourcesManager()
        {
            Passwords = Properties.Resources.Passwords.Split(new string[] { Environment.NewLine },
                                                             StringSplitOptions.RemoveEmptyEntries);

            Words = Properties.Resources.RandomWords.Split(new string[] { Environment.NewLine },
                                                           StringSplitOptions.RemoveEmptyEntries);

            random = new Random();
        }

        public static string GetRandomPassword()
        {
            var index = random.Next(0, Passwords.Length - 1);

            return Passwords[index];
        }

        public static string GetRandomWord()
        {
            var index = random.Next(0, Words.Length - 1);

            return Words[index];
        }
    }
}
