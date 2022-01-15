﻿using System;

namespace MovieNight.Common.Entities
{
    public interface IAudit
    {
        string CreatedBy { get; set; }
        DateTime CreatedAt { get; set; }
        string UpdatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
