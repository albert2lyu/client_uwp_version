using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFRobotWindowsIoTTempelate
{
    interface SmartService
    {
        string connectionDB();
        string insertData();
        string getData();
        string deleteData();
    }
}
