using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DFrobotWindowIoTTempelate
{
    class ServiceSocket
    {
        String task= null;
        Socket _socket;
        IPEndPoint ipep = null;

        public ServiceSocket(String ip) {
            ipep =  new IPEndPoint(IPAddress.Parse(ip), 5000);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(ipep);
        }

        public void setTask(String task) {
            this.task = task;
        }
        public String getTask() {
            return task;
        }

        public void send() {
            
            try
            {
                if(getTask() != null)
                {
                    _socket.Send(Encoding.UTF8.GetBytes(getTask()));
                    _socket.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
           
        }

       
    }
}
