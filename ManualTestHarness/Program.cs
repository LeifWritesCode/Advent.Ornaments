using Ornaments.App;

namespace ManualTestHarness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventApp.CreateDefault().RunAsync(args).GetAwaiter().GetResult();
        }
    }
}