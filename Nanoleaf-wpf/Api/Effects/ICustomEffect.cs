using System.Threading.Tasks;

namespace Winleafs.Wpf.Api.Effects
{
    public interface ICustomEffect
    {
        Task Activate();

        Task Deactivate();

        bool IsContinuous();

        bool IsActive();
    }
}
