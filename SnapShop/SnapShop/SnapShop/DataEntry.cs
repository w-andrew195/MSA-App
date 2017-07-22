using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapShop
{
    public class DataEntry
    {
        public string checker(string DataIn)
        {

            int count = 0;

            List<string> URL = new List<string>();
            URL.Add("http://www.thewarehouse.co.nz/");
            URL.Add("http://www.rebelsport.co.nz/");
            URL.Add("https://www.hallensteins.com/");


            List<string> places = new List<string>();
            places.Add("The Warehouse");
            places.Add("Rebel Sports");
            places.Add("Hallensteins");

            foreach (string n in places)
            {
                if (n == DataIn)
                {
                    break;
                }
                count++;
            }//End of loop
            return URL[count];

        }
    }
}
