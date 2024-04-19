using ServiceStack;
using StandaloneRepo.ServiceModel;

namespace StandaloneRepo.ServiceInterface;

public class MyServices : Service
{
    public object Any(Hello request)
    {
        return new HelloResponse { Result = $"Hello, {request.Name}!" };
    }
}