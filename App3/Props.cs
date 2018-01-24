using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonySym
{
    class Props
    {
        private static Dictionary<string, double> dic;
        public Props()
        {
            dic = new Dictionary<string, double>();       

            // parse file into readable variables set
            Init();
        }

        private void Init()
        {
            dic.Clear();
            string filePath = "props.conf";
            foreach (string line in File.ReadAllLines(filePath))
            {
                if (!line.StartsWith("#") && !line.Trim().Equals(""))
                {
                    string[] vars = line.Split(new[] { '=' });
                    dic.Add(vars[0].Trim(), Convert.ToDouble(vars[1].Trim()));
                }
            }
        }
        public void Reload()
        {
            Init();
        }
        public int GetInt(string param)
        {
            int tmpRes = -1;
            try
            {
                tmpRes = Convert.ToInt32(dic[param]);
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }  
            return tmpRes;
        }
        public double GetDouble(string param)
        {
            double tmpRes = -1;
                tmpRes = dic[param];
            return tmpRes;
        }

        public string[] Keys()
        {
            return dic.Keys.ToArray();
        }

        public string Value(string TKey)
        {
            return dic[TKey].ToString();
        }
    }
}
