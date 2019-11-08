using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFromGraph
{
    class Program
    {
        public static void Main(string[] args)
        {
            String en = "enum States{";
            Console.WriteLine("makeCodeFromGraph 1.1.0\n"+
                "В долгожданном обновлении появились ключевые слова, а также возможность их добавления:\n" +
                "digit и letter. Например в символах перехода вместо 1..9а..я_ можно писать digitletter_\n" +
                "Будут использованы функции IsDigit IsLetter  остальные символы перехода пишем также без пробела\n" +
                "-=+)(digitletter| и тд\n"+
                "С каждым обновлением наша программа становится всё более человечной (*.*)\n"+
                "Удачи");
            string s = "";
            Console.Write("Введите кол-во состояний: ");
            int num = Convert.ToInt32(Console.ReadLine());
            State[] states = new State[num];
            for(int i = 0; !s.Equals("F"); i++)
            {
                Console.WriteLine("--------------------------------------------------------------");
                Console.Write("Введите название состояния: ");
                s = Console.ReadLine();
                en += s + ',';
                State state = new State(s);
                Console.Write("Введите кол-во переходов: ");
                int n = Convert.ToInt32(Console.ReadLine());
                for(int j = 0; j < n; j++)
                {
                    Console.Write("Введите следующее состояние: ");
                    string nextState = Console.ReadLine();
                    Console.Write("Введите символы перехода в него: ");
                    string symbols = Console.ReadLine();
                    state.addToMap(symbols, nextState);
                }
                if (n > 0)
                {
                 //   Console.Write("Введите состояние ошибки:");
                    state.setErrorState("E"+i);
                    en += "E" + i + ',';
                }
               
                Console.WriteLine("--------------------------------------------------------------");
                states[i] = state;
            }
            Console.WriteLine(en);
            Console.ReadLine();
            getCode(states);
        }
        public static void getCode(State[] states)
        {
            StreamWriter streamWriter = new StreamWriter("result.txt");
            streamWriter.WriteLine("switch (states)");
            streamWriter.WriteLine("{");
            for(int i = 0; i < states.Length; i++)
            {
                streamWriter.Write(states[i].ToString());
            }
            streamWriter.WriteLine("}");
            streamWriter.Close();
        }
        

    }
    class State
    {
        private string name = "";
        private string errorState = "";
        private Dictionary<int, string> symbols = new Dictionary<int, string>();
        private Dictionary<int,string> nextStates = new Dictionary<int, string>();
        private Dictionary<string, bool> words = new Dictionary<string, bool>();
     
        public State(string n)
        {
            words.Add("digit", false);
            words.Add("letter", false);
            this.name = n;
        }
        public void setErrorState(string s)
        {
            errorState = s;
        }
        public string getName()
        {
            return name;
        }
        public int getNumOfNextStates()
        {
            return symbols.Count;
        }
        public void addToMap(string symb, string nextSt)
        {
            symbols.Add(symbols.Count, symb);
            nextStates.Add(nextStates.Count,nextSt);
        }
        private string getCondition(string s)
        {
           
            s = s.ToLower();
            string condition = "";
            foreach (var v in words.Keys.ToArray<string>()) {
                words[v] = false;
                if (s.Contains(v))
                {
                    words[v] = true;
                    int index = s.LastIndexOf(v);
                   s = s.Remove(index, v.Length);
                }
            }
            if (words["digit"])
            {
                if (words["letter"])
                {
                    condition += "Char.IsLetterOrDigit(s[i])";
                }
                else
                {
                    condition += "Char.IsDigit(s[i])";
                }
            }else if (words["letter"])
            {
                condition += "Char.IsLetter(s[i])";
            }
            if (s.Length > 0)
            {
                if (condition.Length > 0) { condition += " || "; }
                for(int i = 0; i < s.Length; i++)
                {
                    condition += "s[i] == \'" + s[i]+"\'";
                    if (i < s.Length - 1) { condition += " || "; }
                }
            }
            return condition;
        }
        public override string ToString()
        {
            
            string s = "";
           
            if (getNumOfNextStates() != 0)
            {
                s += " case States." + name+": \n";
                s += " if (" + getCondition(symbols[0]) +"){\n ";
                s += " states = States." + nextStates[0] + ";\n";
                s += "}\n";
                for(int i = 1; i < getNumOfNextStates(); i++)
                {
                    s += "else if(" + getCondition(symbols[i]) + "){\n";
                    s +=" states = States." + nextStates[i] + ";\n";
                    s += "}\n";
                }
                s += "else{"+"states = States."+errorState + ";} break;\n";
            //    s += "; break;\n";
            }
            return s;
        }
    }
}
