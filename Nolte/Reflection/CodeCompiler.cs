using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.ComponentModel;
using System.Collections;

namespace Nolte.Reflection
{
    public class CodeCompiler
    {

        protected object Objekt;
        public ArrayList ErrorMessages { get; protected set; }
        public bool Suceeded { get; protected set; }

        public CodeCompiler(string src, string classNameToInstanciate)
        {
            ErrorMessages = new ArrayList();
            CSharpCodeProvider cp = new CSharpCodeProvider();
            //ICodeCompiler ic = cp.CreateCompiler();
            CompilerParameters cpar = new CompilerParameters();
            cpar.GenerateInMemory = true;
            cpar.GenerateExecutable = false;
            cpar.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            cpar.ReferencedAssemblies.Add("System.Drawing.dll");
            cpar.ReferencedAssemblies.Add("system.dll");
            CompilerResults cr = cp.CompileAssemblyFromSource(cpar, src);
            foreach (CompilerError ce in cr.Errors)
                ErrorMessages.Add("Zeile " + (ce.Line - 13) + ": " + ce.ErrorText);
            if (cr.Errors.Count == 0 && cr.CompiledAssembly != null)
            {
                Type ObjType = cr.CompiledAssembly.GetType(classNameToInstanciate);
                try
                {
                    if (ObjType != null)
                    {
                        Objekt = Activator.CreateInstance(ObjType);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessages.Add(ex.Message);
                }
                Suceeded = true;
            }
            else
                Suceeded = false;
        }

        public object CallFunction(string functionName, object[] param)
        {
            if (Objekt != null)
            {
                MethodInfo evalMethod = Objekt.GetType().GetMethod(functionName);
                try
                {
                    return evalMethod.Invoke(Objekt, param);
                }
                catch
                {
                }
            }
            return null;
        }

    }
}
