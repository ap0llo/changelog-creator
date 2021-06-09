using Cake.Common.Build;
using Cake.Core;

namespace Build
{
    internal static class CakeContextExtensions
    {
        public static bool IsRunningOnAzurePipelines(this ICakeContext context)
        {
            return context.AzurePipelines().IsRunningOnAzurePipelines ||
                   context.AzurePipelines().IsRunningOnAzurePipelinesHosted;
        }
    }
}
