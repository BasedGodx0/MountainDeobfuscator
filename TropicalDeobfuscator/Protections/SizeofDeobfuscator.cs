using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace TropicalDeobfuscator.Protections
{
    internal class SizeofDeobfuscator
    {
        public static int Fix()
        {
            int Deobfuscated = 0;

            var Module = DeobfuscatorContext.Module;
            foreach (TypeDef Type in Module.GetTypes())
            {
                foreach (MethodDef Method in Type.Methods)
                {
                    if (!Method.HasBody && !Method.Body.HasInstructions)
                        continue;

                    var instr = Method.Body.Instructions;

                    for (int i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode == OpCodes.Sizeof)
                        {
                           
                            Type TypeToGet = System.Type.GetType(instr[i].Operand.ToString());

                            int realValue = Marshal.SizeOf(TypeToGet);

                            instr[i].OpCode = OpCodes.Ldc_I4;
                            instr[i].Operand = realValue;
                           
                            Deobfuscated++;
                        }
                    }
                }
            }

            return Deobfuscated;
        }
    }
}
