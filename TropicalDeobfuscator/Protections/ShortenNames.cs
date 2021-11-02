using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TropicalDeobfuscator.Protections
{
    internal class ShortenNames
    {
        public static void Fix()
        {
            var module = DeobfuscatorContext.Module;
            foreach(TypeDef Type in module.GetTypes())
            {

                foreach (MethodDef Method in Type.Methods)
                {
                    if (Method.HasParams())
                    {
                        foreach(var param in Method.Parameters)
                        {
                            param.Name = "";
                        }
                    }
                }
            }
        }
    }
}
