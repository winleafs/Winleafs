﻿namespace Winleafs.Models.Layouts
{
    public class FrameListItem
    {
        public FrameListItem(Frame frame, string name)
        {
            Frame = frame;
            Name = name;
        }

        public string Name { get; set; }
        public Frame Frame { get; set; }
    }
}
