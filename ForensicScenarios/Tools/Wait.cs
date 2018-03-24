using System;
using System.Threading.Tasks;

namespace ForensicScenarios.Tools
{
    public static class Wait
    {
        public static async Task ForTimeAsync(TimeSpan seconds)
        {
            await Task.Delay(seconds);
        }
    }
}
