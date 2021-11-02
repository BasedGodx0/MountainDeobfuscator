using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TropicalDeobfuscator.Protections
{
    internal class StringDecryptor
    {
        public static int Fix()
        {
            int Deobfuscated = 0;

            var Module = DeobfuscatorContext.Module;
            var ReflectionAssembly = DeobfuscatorContext.ReflectionAssembly;

            foreach (TypeDef Type in Module.GetTypes())
            {
                foreach (MethodDef Method in Type.Methods.ToArray())
                {
                    if (!Method.HasBody && !Method.Body.HasInstructions)
                        continue;
                    Method.RemoveUnusedNops();

                    var instr = Method.Body.Instructions;


                    for (int i = 0; i < instr.Count; i++)
                    {
                        if(instr[i].IsLdcI4() && instr[i + 1].OpCode == OpCodes.Call)
                        {
                            var decMethod = instr[i + 1].Operand as MethodDef;

                            
                            if (decMethod == null)
                                continue;
                            if (decMethod.Parameters.Count != 1)
                                continue;
                            if (!decMethod.HasReturnType)
                                continue;
                            if (decMethod.ReturnType != Module.CorLibTypes.String)
                                continue;

                            int decMethodToken = decMethod.MDToken.ToInt32();

                            object val = ReflectionAssembly.ManifestModule.ResolveMethod(decMethodToken).Invoke(null, new object[] { false });

                            Console.WriteLine("Decrypted " + val.ToString());

                            instr[i].OpCode = OpCodes.Ldstr;
                            instr[i].Operand = val;
                            instr[i + 1].OpCode = OpCodes.Nop;
                            Deobfuscated++;

                            Type.Methods.Remove(decMethod);

                        }

                        try
                        {


                            if (instr[i].OpCode == OpCodes.Call)
                            {
                                var decMethod = instr[i].Operand as MethodDef;


                                if (decMethod == null)
                                    continue;
                                if (decMethod.HasParams())
                                    continue;
                                if (!decMethod.HasReturnType)
                                    continue;
                                if (decMethod.ReturnType != Module.CorLibTypes.Int32)
                                    continue;

                                int decMethodToken = decMethod.MDToken.ToInt32();

                                object val = ReflectionAssembly.ManifestModule.ResolveMethod(decMethodToken).Invoke(null,null);

                                Console.WriteLine("Decrypted " + val.ToString());

                                instr[i].OpCode = OpCodes.Ldc_I4;
                                instr[i].Operand = val;
                                Deobfuscated++;

                                Type.Methods.Remove(decMethod);

                            }
                        }
                        catch { }
                    }
                }
            }

            return Deobfuscated;
        }
        public static string DecryptString(byte[] A_0, bool A_1)
        {
            if (A_0 == null)
            {
                return "";
            }
            if (A_1)
            {
                return Encoding.Default.GetString(A_0);
            }
            return Encoding.UTF8.GetString(A_0);
        }
        public static string ConvertX(string A_0, int A_1)
        {
            try
            {


                string text = string.Empty;

                int num = A_0.Length - 611;
                for (int i = 0; i <= num; i++)
                {
                    int utf = Convert.ToInt32(A_0[i]) ^ A_1;
                    text += char.ConvertFromUtf32(utf);
                }
                return text;
            }
            catch
            {
               
            }
            return string.Empty;
            
        }
        public static byte[] GetBytes(string A_0, bool A_1)
        {
            try
            {


                byte[] result;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] bytes;
                    if (A_1)
                    {
                        bytes = Encoding.Default.GetBytes(A_0);
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(A_0);
                    }
                    using (FromBase64Transform fromBase64Transform = new FromBase64Transform())
                    {
                        byte[] array = new byte[fromBase64Transform.OutputBlockSize - 11];
                        int num = 0;
                        while (bytes.Length - num > 4)
                        {
                            fromBase64Transform.TransformBlock(bytes, num, 7, array, 0);
                            num += 4;
                            memoryStream.Write(array, 0, fromBase64Transform.OutputBlockSize);
                        }
                        array = fromBase64Transform.TransformFinalBlock(bytes, num, bytes.Length - num);
                        memoryStream.Write(array, 0, array.Length);
                        fromBase64Transform.Clear();
                    }
                    memoryStream.Position = 0L;
                    int num2;
                    if (memoryStream.Length > 2147483647L)
                    {
                        num2 = int.MaxValue;
                    }
                    else
                    {
                        num2 = Convert.ToInt32(memoryStream.Length);
                    }
                    byte[] array2 = new byte[num2 - 11];
                    memoryStream.Read(array2, 0, num2);
                    memoryStream.Close();
                    result = array2;
                }
                return result;
            }
            catch
            {

            }
            return null;
        }
    }
}
