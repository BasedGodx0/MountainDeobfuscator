using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.IO.Compression;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;

namespace TropicalDeobfuscator.Protections
{
    internal class ResourceDecrypt
    {
        private static ModuleDefMD Module = DeobfuscatorContext.Module;
        public static void Fix()
        {
            EmbeddedResource res = (EmbeddedResource)Module.Resources[0];
            if (res.Name != "<Duck!>‎‎" && Module.Resources.Count == 2)//skid
            {
                Console.WriteLine("Encrypted Resource Name : " + res.Name);

                var resource = res.CreateReader().AsStream();
                ModuleDefMD loader = null;
                using (MemoryStream memStream = new MemoryStream())
                {
                    Byte[] BufferFlow = new byte[0x1000];
                    int Read = resource.Read(BufferFlow, 0, BufferFlow.Length);
                    do
                    {
                        memStream.Write(BufferFlow, 0, Read);
                        Read = resource.Read(BufferFlow, 0, BufferFlow.Length);
                    }
                    while (Read != 0);

                    byte[] asmArray = Dec(memStream.ToArray());
                    Array.Reverse(asmArray);
                    loader = ModuleDefMD.Load(asmArray);

                    foreach (var theResources in loader.Resources)
                    {
                        Module.Resources.Add(theResources);
                    }
                }
            }
        }
        public static byte[] Dec(byte[] P_0)
        {
            try
            {
                return Get(new GZipStream(new MemoryStream(P_0), CompressionMode.Decompress, leaveOpen: false), P_0.Length);
            }
            catch
            {
                byte[] result = null;
                return result;
            }
        }
        public static byte[] Get(Stream P_0, int P_1)
        {
            int num = 0;
            try
            {
                byte[] array = default(byte[]);
                while (true)
                {
                    array = (byte[])Utils.CopyArray(array, new byte[num + P_1 + 1]);
                    int num2 = P_0.Read(array, num, P_1);
                    if (num2 == 0)
                        break;
                    num += num2;
                }
                array = (byte[])Utils.CopyArray(array, new byte[num - 1 + 1]);
                return array;
            }
            catch (Exception e)
            {
                byte[] result = null;
                return result;
            }
        }
    }
}
