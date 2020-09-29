namespace Winleafs.Models.Models.Scheduling.Triggers
{
    /// <summary>
    /// Model class for a process event trigger
    /// </summary>
    public class ProcessEventTrigger : EventTrigger
    {
        public string ProcessName { get; set; }

        public override string GetDescription()
        {
            return ProcessName;
        }
    }
}
