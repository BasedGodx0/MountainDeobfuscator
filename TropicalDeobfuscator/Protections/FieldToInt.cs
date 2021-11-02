using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TropicalDeobfuscator.Protections
{
    internal class FieldToInt
    {
        public static int Fix()
        {
            int Deobfuscated = 0;

            var Module = DeobfuscatorContext.Module;
            var ReflectionAssembly = DeobfuscatorContext.ReflectionAssembly;

            foreach (TypeDef Type in Module.GetTypes())
            {
                foreach(MethodDef Method in Type.Methods)
                {
                    if (!Method.HasBody && !Method.Body.HasInstructions)
                        continue;

                    var instr = Method.Body.Instructions;

                    for(int i = 0; i < instr.Count; i++)
                    {
                        if(instr[i].OpCode == OpCodes.Ldsfld && instr[i + 1].IsLdcI4() && instr[i + 2].OpCode == OpCodes.Ldelem_I4)
                        {
                            FieldDef IntField = instr[i].Operand as FieldDef;
                            int FieldToken = IntField.MDToken.ToInt32();

                            int[] value = (int[])ReflectionAssembly.ManifestModule.ResolveField(FieldToken).GetValue(null);
                            int Index = instr[i + 1].GetLdcI4Value();

                            object realValue = value[Index];


                            instr[i].OpCode = OpCodes.Ldc_I4;
                            instr[i].Operand = realValue;

                        

                            instr[i + 1].OpCode = OpCodes.Nop;
                            instr[i + 2].OpCode = OpCodes.Nop;
                            Deobfuscated++;
                        }
                    }
                }
            }

            return Deobfuscated;
        }
    }
}
