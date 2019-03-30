namespace Winleafs.Models.Models.Scheduling.Triggers
{
    public class ProcessEventTrigger : BaseEventTrigger
    {
        public string ProcessName { get; set; }

        public override string GetDescription()
        {
            return ProcessName;
        }
    }
}
