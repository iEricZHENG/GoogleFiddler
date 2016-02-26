using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleFiddler
{
    public class DataReceivedEventArgs_Kiwi : SecurityQueue<Fiddler.Session>
    {
        private DataReceivedEventArgs_Kiwi() { }
        public static DataReceivedEventArgs_Kiwi Instance
        {
            get
            {
                return Nested.Inner;
            }
        }
        private static class Nested
        {
            public static readonly DataReceivedEventArgs_Kiwi Inner = new DataReceivedEventArgs_Kiwi();
        }
    }   
}
