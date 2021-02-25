using System.Threading.Tasks;
using Orleans;

namespace GrainsAbstractions
{
    public interface IHelloWorldGrain : IGrainWithIntegerKey
    {
        Task<string> SayHello(string name);
    }
}
