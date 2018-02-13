using System;
using System.Threading.Tasks;

namespace ForensicScenarios.Tools
{
    public static class Wait
    {
        public static async Task ForSecondsAsync(TimeSpan seconds)
        {
            await Task.Delay(seconds);
        }
    }
}
