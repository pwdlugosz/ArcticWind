using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.Aggregates;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Tables;
using ArcticWind.Alpha;

namespace ArcticWind
{

    class Program
    {

        static void Main(string[] args)
        {
               
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            
            Host Enviro = new Host();
            Enviro.Libraries.Allocate("CHRONO", new Libraries.ChronoLibrary(Enviro));
            Enviro.Libraries.Allocate("RANDOM", new Libraries.RandomLibrary(Enviro));
            Enviro.Libraries.Allocate("FILE", new Libraries.LibraryFile(Enviro));
            Enviro.Libraries.Allocate("STREAM", new Libraries.StreamLibrary(Enviro));
            Enviro.Libraries.Allocate("MATH", new Libraries.MathLibrary(Enviro));
            ////Enviro.Libraries.Allocate("MATH", new Libraries.MathLibrary(Enviro));
            ////Enviro.Libraries.Allocate("TABLE", new Libraries.TableLibrary(Enviro));
            ////Enviro.Libraries.Allocate("FILE", new Libraries.LibraryFile(Enviro));
            ////Enviro.Libraries.Allocate("WEB", new Libraries.LibraryWeb(Enviro));

            Scripting.ScriptProcessor sp = new Scripting.ScriptProcessor(Enviro);

            string script = System.IO.File.ReadAllText(@"C:\Users\pwdlu_000\Documents\ArcticWind\Scripting\TestScript.txt");

            sp.RenderAction(script);

            Enviro.ShutDown();

            
            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string t = Console.ReadLine();


        }

        public static void TestUTF8()
        {

            BString a = "Hello World! Hello again?? Hello one last time...";
            a = a.Replace("Hello", "Hey").ToUpper();
            Console.WriteLine(a);

            BString b = "    Hello World!!!    ";
            Console.WriteLine(b.Length);
            Console.WriteLine(b.Trim().Length);
            Console.WriteLine(b.Remove("!"));

            byte[] c = BString.Empty.ToByteArray;
            Console.WriteLine(c.Length);


        }

        public static void TestJoins()
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            Host Enviro = new Host();

            Table x = Testing.SampleTables.SampleHeapTable(Enviro, "Test1", 100000, 0);

            Enviro.ShutDown();

            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string t = Console.ReadLine();

        }

        public static void TestDictionary()
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            Host Enviro = new Host();
            Schema s = new Schema("Key int, Value double, xyz int");
            DictionaryTable t = Enviro.CreateTable("TEMP", "TEST1", new Schema("KEY1 LONG, KEY2 CSTRING.3"), new Schema("VALUE1 DOUBLE, VALUE2 LONG"));

            for (int i = 0; i < 10000; i++)
            {

                Record k = Record.Stitch(new Cell(i), Enviro.BaseRNG.NextCString(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
                Record v = Record.Stitch(Enviro.BaseRNG.NextDoubleGauss(), Enviro.BaseRNG.NextLong());
                t.Add(k, v);

            }

            t.Dump(@"C:\Users\pwdlu_000\Documents\Pulse_Projects\Test\DreamDictionartText.txt");

            Enviro.ShutDown();

            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string fibux = Console.ReadLine();

        }

        public static class AdHoc
        {

            public static byte[] RandomSeries(Random r)
            {

                byte[] series = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                int n = series.Length;
                while (n > 0)
                {
                    n--;
                    int k = r.Next(n + 1);
                    byte b = series[k];
                    series[k] = series[n];
                    series[n] = b;
                }

                return series;

            }

            public static void RandInts(Random r, out int LowValue, out int HighValue)
            {

                byte[] s = RandomSeries(r);
                LowValue = int.Parse(s[0].ToString() + s[1].ToString() + s[2].ToString() + s[3].ToString());
                HighValue = int.Parse(s[4].ToString() + s[5].ToString() + s[6].ToString() + s[7].ToString() + s[8].ToString());
                
            }

            public static void Simulation()
            {

                Random r = new Random(127);
                int max = 1000000;
                Dictionary<int, string> vals = new Dictionary<int, string>();

                for (int i = 0; i < max; i++)
                {

                    int low = 0, high = 0;
                    RandInts(r, out low, out high);

                    int m = high % low;
                    int n = high / low;

                    if (m == 0 && n < 10)
                    {
                        if (!vals.ContainsKey(n))
                            vals.Add(n, string.Format("Ratio {0} : Low {1} : High {2}", n, low, high));
                    }

                }

                foreach (var x in vals)
                {
                    Console.WriteLine(x.Value);
                }

            }



        }


    }

}
