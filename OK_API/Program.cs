using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OK_API
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ok = new OpenKneeboardAPI();
            ok.SendCommand("NEXT_TAB", "NEXT_TAB");
        }
    }
}
