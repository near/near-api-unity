using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public interface IExternalAuthService
    {
        bool OpenUrl(string url);
    }
}
