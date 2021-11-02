using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TropicalDeobfuscator.Protections;

namespace TropicalDeobfuscator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filename;
           
            try
            {
                filename = args[0];
            }
            catch
            {
                Console.WriteLine("Enter file path:");
                filename = Console.ReadLine().Replace("\"", "");
            }

            DeobfuscatorContext.Module = ModuleDefMD.Load(filename);
            DeobfuscatorContext.ReflectionAssembly = Assembly.LoadFile(filename);

            ShortenNames.Fix();
            Console.WriteLine("Fields Fixed : " + FieldToInt.Fix());
            Console.WriteLine("SizeOfs Fixed : " + SizeofDeobfuscator.Fix());
            Console.WriteLine("Operations Fixed : " + OperationFixer.Fix());
            Console.WriteLine("ProxyInt Fixed : " + FixProxy.ProxyInt());
            Console.WriteLine("ProxyString Fixed : " + FixProxy.ProxyStrings());
            Console.WriteLine("Booleans Decrypted  : " + BoolDecryptor.Fix());
            Console.WriteLine("Strings Decrypted  : " + StringDecryptor.Fix());

            var options = new ModuleWriterOptions(DeobfuscatorContext.Module);
            options.MetadataLogger = DummyLogger.NoThrowInstance;

            DeobfuscatorContext.Module.Write(filename.Replace(".exe","-Deobfuscated.exe"),options);

            Console.ReadKey();
        }
    }
}
