using System;
using System.Net;

namespace Shared
{
    public class ImpatientWebClient : WebClient
    {
        public int Timeout { get; set; }

        public ImpatientWebClient()
        {
            Timeout = 10000;
        }

        public ImpatientWebClient(int timeout)
        {
            Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest w = base.GetWebRequest(address);
            if (w != null)
            {
                w.Timeout = Timeout;
            }
            return w;
        }
    }
}
