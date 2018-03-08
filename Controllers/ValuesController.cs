﻿using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dxa_dict_service.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        string[] data;

        // GET api/values/5
        [HttpGet("{query}")]
        public string Get(string query)
        {
            Console.WriteLine("Request recieved from: [Remote] {0} || {1} ------[Local] || {2} || {3}", Request.HttpContext.Connection.RemoteIpAddress, Request.HttpContext.Connection.RemotePort, Request.HttpContext.Connection.LocalIpAddress, Request.HttpContext.Connection.LocalPort);
            //Console.WriteLine(Request.ToString());
            string toReturn = "";
            if (!(query == null))
            {
                
                if (data == null)
                {
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead("https://raw.githubusercontent.com/thezaza101/dxa-dict-service/master/DefCat.txt");
                    
                    StreamReader sr = new StreamReader(stream);
                    string x = sr.ReadToEnd();
                    sr.Close();

                    data = x.Split(',');
                }
                if (!string.IsNullOrWhiteSpace(query))
                {
                    if (query.Length > 3)
                    {
                        toReturn = GetMatch(query);
                    }
                }
            }
            return toReturn;
             
        }



        string GetMatch(string input)
        {
            string output = "";
            DistanceResult[] result = new DistanceResult[data.Length];
            
            int i = 0;
            foreach (string s in data)
            {
                result[i].value = s;
                result[i].distance = Levenshtein(input.ToLower(), s.ToLower());
                i++;
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
            try{
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
    public struct DistanceResult
    {
        public string value;
        public int distance;
    }
}
