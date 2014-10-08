using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace MathProcessor
{
    public class CommandCashe
    {
        List<string> cashedStrings = new List<string>();
        static int currentIndex;

        public int Count
        {
            get { return cashedStrings.Count; }
        }

        public void SaveXML(XElement root)
        {
            XElement element = new XElement("cache");
            foreach (string s in cashedStrings)
            {
                element.Add(new XElement("c", s));
            }
            root.Add(element);
        }

        public void Clear()
        {
            currentIndex = 0;
            cashedStrings.Clear();
        }

        public void LoadXML(XElement xe)
        {
            cashedStrings.Clear();
            XElement element = xe.Element("cache");
            foreach (var v in element.Elements("c"))
            {
                cashedStrings.Add(v.Value);
            }
        }

        public void AddString(string str)
        {
            if (str.Length > 0)
            {
                if (!cashedStrings.Contains(str))
                {
                    cashedStrings.Add(str);
                }
                else
                {
                    cashedStrings.RemoveAt(cashedStrings.IndexOf(str));
                    cashedStrings.Add(str);
                }
                currentIndex = cashedStrings.Count;
            }
        }

        public string Next()
        {
            if (cashedStrings.Count > 0)
            {
                if (currentIndex < Count-1)
                {
                    currentIndex++;
                }
                else if (currentIndex == Count-1)
                {
                    currentIndex++;
                    return "";
                }
                else if (currentIndex == Count)
                {
                    return "";
                }
                return cashedStrings[currentIndex];
            }
            return "";            
        }

        public string Previous()
        {
            if (Count > 0)
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex++;
                }
                return cashedStrings[currentIndex];

            }
            else
            {
                return "";
            }
        }
    }
}
