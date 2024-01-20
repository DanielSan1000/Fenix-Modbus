using System;

namespace ProjectDataLib
{
    public class CustomException
    {
        public DateTime Czas { get; set; }

        public CustomException(object sender, Exception ex)
        {
            Ex = ex;
            Sender = sender;
            Czas = DateTime.Now;
        }

        public Exception Ex { get; set; }
        public object Sender { get; set; }
    }
}