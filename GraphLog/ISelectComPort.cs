using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLog
{
    public interface ISelectComPort
    {
        void setSelectedComPort(String port_A, String port_B, int baud);
    }
}
