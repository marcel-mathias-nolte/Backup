using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCARS
{
    class iCalToken
    {
        public iCalToken(iCalToken Parent)
        {
            this.Parent = Parent;
        }
        public iCalToken()
        { }
        public iCalToken Parent;
        public int Count
        {
            get
            {
                return childs.Count;
            }
        }

        Dictionary<string, iCalToken> childs = new Dictionary<string, iCalToken>();
        public iCalToken this[string key]
        {
            get 
            { 
                return this.childs.ContainsKey(key) ? this.childs[key] : null; 
            }
            set
            {
                if (this.childs.ContainsKey(key))
                    this.childs[key] = value;
                else
                    this.childs.Add(key, value);
            }
        }
    }
    class iCalPropertyToken : iCalToken
    {
        public iCalPropertyToken(iCalToken Parent, string Value)
        {
            this.Parent = Parent;
            this.Value = Value;
        }
        public iCalPropertyToken()
        { }
        public new int Count
        {
            get
            {
                return 1;
            }
        }

        public string Value;
    }
    class iCalListToken : iCalToken
    {
        public iCalListToken(iCalToken Parent)
        {
            this.Parent = Parent;
        }
        public iCalListToken()
        { }
        List<iCalToken> childs = new List<iCalToken>();
        public iCalToken this[int key]
        {
            get
            {
                return key < this.childs.Count ? this.childs[key] : null;
            }
            set
            {
                if (key < this.childs.Count)
                    this.childs[key] = value;
                else
                    this.childs.Add(value);
            }
        }
        public new int Count
        {
            get
            {
                return childs.Count;
            }
        }
        public void Add(iCalToken Token)
        {
            this.childs.Add(Token);
        }
    }
    class iCalParser
    {
        public iCalToken Root;
        public void ParseFile(string file)
        {
            string name;
            iCalToken token = new iCalToken(), temp;
            if (System.IO.File.Exists(file))
                foreach (string line in System.IO.File.ReadAllLines(file))
                {
                    if (line.StartsWith("BEGIN:"))
                    {
                        name = line.Split(':')[1];
                        if (token[name] != null)
                        {
                            if (token[name].GetType() != typeof(iCalListToken))
                            {
                                temp = token[name];
                                token[name] = new iCalListToken(token);
                                ((iCalListToken)token[name]).Add(temp);
                            }
                            ((iCalListToken)token[name]).Add(new iCalToken(token[name]));
                            token = ((iCalListToken)token[name])[((iCalListToken)token[name]).Count - 1];
                        }
                        else
                        {
                            token[name] = new iCalToken(token);
                            token = token[name];
                        }
                    }
                    else if (line.StartsWith("END:"))
                    {
                        token = token.Parent;
                        if (token.GetType() == typeof(iCalListToken))
                        {
                            token = token.Parent;
                        }
                    }
                    else
                    {
                        name = line.Split(':')[0];
                        token[name] = new iCalPropertyToken(token, line.Split(':')[1]);
                    }
                }
            Root = token;
        }
    }
}
