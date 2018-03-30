using System.Diagnostics;
using System.IO;

namespace Debug_Server
{
    internal class Debugger
    {
        public static Debug StartProcess(Debug entity)
        {
            string filename = entity._id;
            string workingDirectory = "/usr/bin/sudo";
            string argument = "";
            switch (entity.Language)
            {
                case Languages.CSharp:
                    using (var sr = new StreamWriter(filename = filename + ".cs"))
                    {
                        sr.Write(entity.Code);
                    }
                    break;
                case Languages.Java:
                    using (var sr = new StreamWriter(filename = filename + ".java"))
                    {
                        sr.Write(entity.Code);
                        argument = "javac " + filename;

                    }
                    break;

                case Languages.C:
                    using (var sr = new StreamWriter(filename = filename + ".c"))
                    {
                        sr.Write(entity.Code);
                        argument = "gcc " + filename;

                    }

                    break;
                case Languages.Cpp:
                    using (var sr = new StreamWriter(filename = filename + ".cpp"))
                    {
                        sr.Write(entity.Code);
                        argument = "g++ " + filename;
                    }


                    break;
                case Languages.Python:
                    using (var sr = new StreamWriter(filename = filename + ".py"))
                    {
                        sr.Write(entity.Code);
                        argument = "python -m py_compile " + filename;
                    }
                    break;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = filename,
                WorkingDirectory = workingDirectory,
                Arguments = argument,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            entity.SuccessResult = process.StandardOutput.ReadToEnd();
            entity.ErrorResult = process.StandardError.ReadToEnd();
            process.WaitForExit();
            File.Delete(filename);
            return entity;
        }
    }
}
