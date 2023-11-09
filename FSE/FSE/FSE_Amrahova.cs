using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {
        static Dictionary<string, int> Frequency(string s)//словарь, где будут храниться буква<->частота
        {
            var res = new Dictionary<string, int>();
            var m = s.Split(' ');
            foreach (var e in m)
            {
                var w = e.Split(':');
                res[w[0]] = int.Parse(w[1]);
            }
            return res;
        }
        static int SumAllFrequency(Dictionary<string, int> d)//сумма всех частот
        {
            var res = 0;
            foreach (var e in d)
                res += e.Value;
            return res;
        }
        static int BigIntervalSize(int N, int q)//количество битов для больших интервалов(где N - сумма всех частот,q - частота данной буквы)
        {
            var res = 0;
            while (Math.Pow(2, res) < (double)N / q)
                res++;
            
            return res;
        }
        static int SmallIntervalSize(int N, int q) => BigIntervalSize(N, q) - 1;//количество битов для малых интервалов(где N - сумма всех частот,q - частота данной буквы)
        static int CountOfSmallInterval(int N, int q)//количество малых интервалов
        {
            var maxbit = BigIntervalSize(N, q);
            
            return (q - (N / (int)Math.Pow(2, maxbit))) * 2;
        }
        static int CountOfBigInterval(int N, int q) => q - CountOfSmallInterval(N, q);//количество больших интервалов

        static Dictionary<string, List<int[]>> Tabl(Dictionary<string, int> d)//словарь, в котором для каждой буквы есть список массивов(диапазоны ячеек)(последний элемент - состояние)
        {
            var res = new Dictionary<string, List<int[]>>();
            var state = 1;

            foreach (var e in d)
            {
                var n = 1;
                var mas = new List<int[]>();
                var sm = CountOfSmallInterval(SumAllFrequency(d), e.Value);

                var bm = CountOfBigInterval(SumAllFrequency(d), e.Value);
                for (int i = 0; i < sm; i++)
                {

                    var m = SmallIntervalSize(SumAllFrequency(d), e.Value);
                    
                    var p = new int[(int)Math.Pow(2, m) + 1];
                    for (int j = 0; j < p.Length - 1; j++)
                    {
                        p[j] = n;
                        n++;
                    }

                    p[p.Length - 1] = state;
                    state++;
                    mas.Add(p);
                }
                for (int i = 0; i < bm; i++)
                {

                    var m = BigIntervalSize(SumAllFrequency(d), e.Value);
                    var p = new int[(int)Math.Pow(2, m) + 1];
                    for (int j = 0; j < p.Length - 1; j++)
                    {
                        p[j] = n;
                        n++;
                    }
                    p[p.Length - 1] = state;
                    state++;
                    mas.Add(p);
                }
                res[e.Key] = mas;
            }
            return res;
        }
        static string Bin(int n, int bit_count)//перевод в двоичную систему. n - число в десятичной, bit_count - количество битов
        {
            if (bit_count == 0)
                return "0";
            string res = "";
            if (n == 0)
            {
                for (int i = 0; i < bit_count; i++)
                {
                    res += 0;
                }
            }
            else { 
            var s = "";

            while (n > 0)
            {
                s += n % 2;
                n /= 2;
            }
            while (s.Length < bit_count)
                s += "0";
            for (int i = s.Length - 1; i >= 0; i--)
            {
                res += s[i];
            }
            }
            return res;
        }
        static int FromBin(string n)
        {
            var nn = Reverse(n);
            var pow = 0;
            var result = 0;
            for (var i = 0; i < nn.Length; i++)
            {
                result += int.Parse(n[i].ToString()) * (int)Math.Pow(2, i);


                pow++;
            }

            return result;

        }


        static string EntropyCoding(string freq, string word)
        {


            var d_frequence = (Frequency(freq));
            var tabl = Tabl(d_frequence);
            var lst = tabl[word[0].ToString()];
            var res = "";
            var state = lst[0][lst[0].Length - 1];

            var f = true;
            for (int i = 1; i < word.Length; i++)
            {
                lst = tabl[word[i].ToString()];

                for (int j = 0; j < lst.Count; j++)
                {
                    for (int q = 0; q < lst[j].Length - 1; q++)
                    {

                        if (state == lst[j][q])
                        {
                            state = lst[j][lst[j].Length - 1];
                            
                            res += Bin(q, (int)Math.Log(lst[j].Length - 1, 2));
                            //Console.WriteLine(Bin(q, (int)Math.Log(lst[j].Length - 1, 2))+" "+state+word[i]);
                            f = false;
                            break;
                        }

                    }
                    if (!f)
                    {

                        f = true;
                        break;
                    }

                }

            }

            return res + " " + state;
        }
        static string Decoding(string freq, string s)
        {
            var result = "";
            var d_frequence = (Frequency(freq));
            var tabl = Tabl(d_frequence);
            var m = s.Split(' ');
            var state = int.Parse(m[1]);
            var encoded_message = m[0];
            var res = Reverse(encoded_message);

            while (res.Length != 0)
            {

                var str = "";
                foreach (var e in tabl)
                {
                    var lst = e.Value;
                    foreach (var j in lst)
                    {
                        if (j[j.Length - 1] == state)
                        {
                            result += e.Key;

                            var bits = (int)Math.Log(j.Length - 1, 2);
                            if (bits == 0)
                                str = res.Substring(0, 1);
                            else
                            str = res.Substring(0, bits);

                            var x = FromBin(str);
                           
                            state = j[x];
                            
                            res = res.Remove(0, str.Length);
                           
                            break;
                        }
                    }
                }
               
            }
            foreach (var e in tabl)
            {
                var lst = e.Value;
                foreach (var j in lst)
                {
                    if (j[j.Length - 1] == state)
                    {
                        result += e.Key;
                    }
                }
            }
                        
            return Reverse(result);
        }
            
            

            


        
        static string Reverse(string s)
        {
            var res = "";
            for (int i = s.Length-1; i >=0 ; i--)
            {
                res+=s[i];
            }
            return res;
        }
        static void Main(string[] args)
        {

            //var t = "A:3 B:2 C:2 D:1";
            //var w = "AABABCDC";
            Console.WriteLine("Введите количество символов в алфавите");
            var e = Console.ReadLine();
            Console.WriteLine("Введите символы и их часоты");
            var t = Console.ReadLine();
            Console.WriteLine("Введите строку, которую необходимо сжать");
            var w = Console.ReadLine();
            Console.WriteLine("Сжатая строка и ее состояние");
            Console.WriteLine(EntropyCoding(t, w));
            Console.WriteLine("Строка после расжатия");
            Console.WriteLine(Decoding(t, EntropyCoding(t, w)));
            
            
        }
    }
}
