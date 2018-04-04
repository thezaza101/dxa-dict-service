using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DXANET;

namespace dxa_dict_service.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        List<DictElement> data;        

        // GET api/values/5
        [HttpGet("{query}")]
        public string Get(string query, List<string> domain)
        {
            string domains = "";
            domain.ForEach(i => domains = domains + i + " ");
            Console.WriteLine("{0}:{1}: Request recieved: {2} in the {3} domains", DateTime.Now, DateTime.Now.Millisecond, query,domains);
            //Console.WriteLine(Request.ToString());
            string toReturn = "";
            if (!(query == null))
            {
                
                if (data == null)
                {
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead("https://raw.githubusercontent.com/thezaza101/dxa-dict-src-creator/master/DictData.csv");
                    StreamReader sr = new StreamReader(stream);
                    string x = sr.ReadToEnd();
                    sr.Close();
                    data = DictElement.ReadDictElementsFromCSV(x);
                    Console.WriteLine("Read {0} elements into memory", data.Count());
                }
                if (!string.IsNullOrWhiteSpace(query))
                {
                    if (query.Length > 3)
                    {
                        toReturn = GetMatch(query, domain);
                    }
                }
            }
            return toReturn;
             
        }



        string GetMatch(string input, List<string> domains)
        {
            string output = "";
            DistanceResult[] result;
            bool searchSpecificDomains = false;
            if (!domains.Count.Equals(0))
            {
                searchSpecificDomains = true;   
            }

            int i = 0;
            if (searchSpecificDomains)
            {
                List<DictElement> filterdDataElements = data.Where(d => domains.Contains(d.domainAcronym)).ToList();
                result = new DistanceResult[filterdDataElements.Count];
                foreach (DictElement s in filterdDataElements)
                {
                    result[i].value = s.elementName;
                    result[i].distance = Levenshtein(input.ToLower(), s.elementName.ToLower());
                    i++;                
                }
            }
            else
            {
                result = new DistanceResult[data.Count];
                foreach (DictElement s in data)
                {
                    result[i].value = s.elementName;
                    result[i].distance = Levenshtein(input.ToLower(), s.elementName.ToLower());
                    i++;                
                }
            }
            
            DistanceResult maxVal = result.OrderBy(y => y.distance).FirstOrDefault();
            if(maxVal.distance>=Convert.ToInt32(input.Length*.5))
            {
                Console.WriteLine("{0}:{1}: Nothing found", DateTime.Now, DateTime.Now.Millisecond);               
            }
            else
            {
                Console.WriteLine("{0}:{1}: {2} (Match distance: {3})", DateTime.Now, DateTime.Now.Millisecond, maxVal.value, maxVal.distance);
                output = maxVal.value;
            }

            return output;
        }

        /// <summary>
        /// Compute the Levenshtein distance between two strings.
        /// </summary>
        public int Levenshtein(string s, string t)
        {
            try
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
            catch
            {
                return 100;
            }
        }        
    }
}
