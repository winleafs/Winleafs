﻿using Nanoleaf_Models.Enums;

namespace Nanoleaf_Models.Models.Scheduling.Triggers
{
    /// <summary>
    /// Trigger interface, all trigger should implement these methods
    /// </summary>
    public interface ITrigger
    {
        TriggerType GetTriggerType();

        bool ShouldTrigger();

        void Trigger();

        /// <summary>
        /// Used by the view to determine the display name
        /// </summary>
        string GetDisplayName();

        /// <summary>
        /// used by the view to determine the description
        /// </summary>
        string GetDescription();
    }
}