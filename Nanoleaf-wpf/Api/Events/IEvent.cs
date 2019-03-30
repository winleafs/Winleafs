namespace Winleafs.Wpf.Api.Events
{
    public interface IEvent
    {
        void StopEvent();

        bool IsActive();

        string GetDescription();

        int GetBrightness();
    }
}
