using System;
using System.Collections.Generic;

namespace NearClientUnity.Utilities
{
    public interface IConnectionInfo
    {
        string Url { get; set; }
        string User { get; set; }
        string Password { get; set; }
        bool AllowInsecure { get; set; }
        TimeSpan Timeout { get; set; }
        Dictionary<string, string> Headers { get; set; }
    }
}