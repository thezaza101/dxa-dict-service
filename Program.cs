using System;
using System.IO;
using System.Collections;
using System.Linq;

namespace DEFCAT
{
    class Program
    {
        static string[] data;

        static void Main(string[] args)
        {
            string filePath;
            string toSearch;
            try
            {
                filePath = args[1];
            }
            catch (System.IndexOutOfRangeException)
            {
                filePath = "DefCat.txt";
            }
            
            try
            {
                toSearch = args[0];
            }
            catch (System.IndexOutOfRangeException)
            {
                toSearch = "";
            }
            

            StreamReader sr = new StreamReader(filePath);
            string x = sr.ReadToEnd();
            sr.Close();

            data = x.Split(Environment.NewLine);
            Console.WriteLine("{0}:{1}: Read {2} elements", DateTime.Now, DateTime.Now.Millisecond, data.Length);

            bool toQuit = false;

            if (string.IsNullOrWhiteSpace(toSearch))
            {
                while (!toQuit)
                {
                    Console.WriteLine("Enter Value to find:");
                    toSearch = Console.ReadLine();
                    if (toSearch == "quit")
                    {
                        toQuit = true;
                        Console.WriteLine("Closing");
                    }
                    else
                    {
                        Console.WriteLine("{0}:{1}: Searching for term \"{2}\"", DateTime.Now, DateTime.Now.Millisecond, toSearch);
                        GetMatch(toSearch);
                    }
                }
            } 
            else 
            {
                Console.WriteLine("{0}:{1}: Searching for term \"{2}\"", DateTime.Now, DateTime.Now.Millisecond, toSearch);
                GetMatch(toSearch);
            }
        }


        static void GetMatch(string input)
        {
            DistanceResult[] result = new DistanceResult[data.Length];
            
            int i = 0;
            foreach (string s in data)
            {
                result[i].value = s;
                result[i].distance = Levenshtein(input.ToLower(), s.ToLower());
                i++;
            }
            DistanceResult maxVal = result.OrderBy(y => y.distance).FirstOrDefault();

            Console.WriteLine(maxVal.distance + " " +Convert.ToInt32(input.Length*.5));

            if(maxVal.distance>=Convert.ToInt32(input.Length*.5))
            {
                Console.WriteLine("{0}:{1}: Nothing found", DateTime.Now, DateTime.Now.Millisecond);               
            }
            else
            {
                Console.WriteLine("{0}:{1}: {2} (Match distance: {3})", DateTime.Now, DateTime.Now.Millisecond, maxVal.value, maxVal.distance);
            }
        }



        /// <summary>
        /// Compute the Levenshtein distance between two strings.
        /// </summary>
        public static int Levenshtein(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
    public struct DistanceResult
    {
        public string value;
        public int distance;
    }
}
