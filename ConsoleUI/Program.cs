using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectsIntoDll;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь к папке с файлами:");
            string pathToFolder = Console.ReadLine();

            List<Dictionary<string, List<string>>> dataFromDll = ResearchAssemblies.FindClassAndMethods(pathToFolder).ToList();

            dataFromDll.ForEach(dictionary =>
            {
                Console.WriteLine("\n");

                dictionary.ToList().ForEach(item =>
                {
                    Console.WriteLine(item.Key);
                    item.Value.ForEach(nameMethod => Console.WriteLine("\t" + nameMethod));
                });
            });
            Console.ReadKey();
        }
    }
}
