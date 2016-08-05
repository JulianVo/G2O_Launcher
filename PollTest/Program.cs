using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PollTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://www.contoso.com:8080/letters/readme.html";

            Regex r = new Regex(@"[:][0-9]{1,5}", RegexOptions.None);


            Match m = r.Match(url);
            Console.WriteLine(m);

            Console.ReadKey();
        }
    }
}
