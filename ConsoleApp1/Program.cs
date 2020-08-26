using Jint;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new Engine().SetValue("log", new Action<object>(Console.WriteLine));
            engine.Execute(@"
              function hello() { 
                log('Hello World');
              };      
              hello();
            ");

            var add = new Engine()
      .Execute("function add(a, b) { return a + b; }")
      .GetValue("add")
      ;

           var a= add.Invoke(1, 2); // -> 3

            var engine2 = new Engine().Execute("function add(a, b) { return a + b; }");
            var b= engine2.Invoke("add", 1, 2); // -> 3
            Console.Read();
        }
    }
}
