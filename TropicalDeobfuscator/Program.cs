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
            Console.Title = "Tropical Deobfuscator by Yeetret, Thanks to CursedSheep for FieldFixer { FieldToInt.cs }";
            Console.ForegroundColor = ConsoleColor.Cyan;
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

            var asmResolver = new AssemblyResolver();
            asmResolver.DefaultModuleContext = new ModuleContext(asmResolver);
            DeobfuscatorContext.Module = ModuleDefMD.Load(filename,asmResolver.DefaultModuleContext);
            DeobfuscatorContext.ReflectionAssembly = Assembly.LoadFile(filename);

            ShortenNames.Fix();

            Console.WriteLine("Fields Fixed : " + FieldToInt.Fix());
            Console.WriteLine("SizeOfs Fixed : " + SizeofDeobfuscator.Fix());

            Console.WriteLine("Arithmetic Fixed : " + OperationFixer.FixAirthmethic());
            Console.WriteLine("Operations Fixed : " + OperationFixer.Fix());
            Console.WriteLine("Arithmetic Fixed : " + OperationFixer.FixAirthmethic());

            Console.WriteLine("ProxyInt Fixed : " + FixProxy.ProxyInt());
            Console.WriteLine("ProxyString Fixed : " + FixProxy.ProxyStrings());
            Console.WriteLine("ProxyNewObj Fixed : " + FixProxy.ProxyNewObj());

            Console.WriteLine("Booleans Decrypted  : " + BoolDecryptor.Fix());
            Console.WriteLine("Constants Decrypted  : " + StringDecryptor.Fix());
            Console.WriteLine("Arithmetic Fixed : " + OperationFixer.FixAirthmethic());

            Console.WriteLine("ProxyBools Fixed : " + FixProxy.ProxyBool());
            Console.WriteLine("ProxyFloats Fixed : " + FixProxy.ProxyFloat());
            Console.WriteLine("ProxyDouble Fixed : " + FixProxy.ProxyDouble());

            ResourceDecrypt.Fix();

            RemoveJunk.Fix();


            var options = new ModuleWriterOptions(DeobfuscatorContext.Module);
            options.MetadataLogger = DummyLogger.NoThrowInstance;
            string filePath = filename.Replace(".exe", "-Deobfuscated.exe");
            DeobfuscatorContext.Module.Write(filePath, options); //ghetto because lol
            Console.WriteLine("File saved to: " + filePath);

            Console.ReadKey();
        }
    }
}
