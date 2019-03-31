using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Wpf.Api.Events
{
    public interface IEventTrigger
    {
        void StopEvent();

        bool IsActive();

        ITrigger GetTrigger();
    }
}
