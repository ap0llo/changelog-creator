using Cake.AzurePipelines.Module;
using Cake.Frosting;

namespace Build
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return new CakeHost()
                .UseModule<AzurePipelinesModule>()
                .UseContext<BuildContext>()
                .Run(args);
        }
    }
}
