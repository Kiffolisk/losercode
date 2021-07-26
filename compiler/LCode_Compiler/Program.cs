using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LCode_Compiler
{
    class LVar
    {
        public string name;
        public object val;
        public LVar(string valname, object valval)
        {
            this.name = valname;
            this.val = valval;
        }
    }
    class Program
    {
        public static string[] INSTRUCTIONS = {"CPR","CPRVAR","AD","SB","DV","ML","MPR","MPL","DF","GI"};
        public static LVar[] SAVEDVARS = new LVar[2048];
        public static int CURRENTPOINTERPOSITION = 0;
        static string SanitizeFolder(string folder)
        {
            string sanitized = folder;
            if (!folder.EndsWith(@"\"))
            {
                sanitized += @"\";
            }
            return sanitized;
        }

        static string SanitizeString(string toSanitize)
        {
            string sanitized = toSanitize.Replace("\"", "");
            return sanitized;
        }

        static LVar GetLVarFromName(string name)
        {
            LVar toreturn = null;
            foreach (LVar var in SAVEDVARS) 
            { 
                if (var.name == name)
                {
                    toreturn = var;
                }
            }
            return toreturn;
        }

        static void Main(string[] args)
        {
            string FileFolder = SanitizeFolder(AppDomain.CurrentDomain.BaseDirectory) + args[0];

            foreach (string fileLine in File.ReadLines(FileFolder))
            {
                string[] InstructionsToExecute = Regex.Split(fileLine, " (?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                switch(InstructionsToExecute[0])
                {
                    // 
                    case "CPR":
                        Console.WriteLine(SanitizeString(InstructionsToExecute[1]));
                        break;
                    case "MPR":
                        CURRENTPOINTERPOSITION++;
                        break;
                    case "MPL":
                        CURRENTPOINTERPOSITION--;
                        break;
                    case "MPRNUM":
                        CURRENTPOINTERPOSITION += int.Parse(InstructionsToExecute[1]);
                        break;
                    case "MPLNUM":
                        CURRENTPOINTERPOSITION -= int.Parse(InstructionsToExecute[1]);
                        break;
                    case "CPRVAR":
                        Console.WriteLine(SanitizeString(SAVEDVARS[CURRENTPOINTERPOSITION].val.ToString()));
                        break;
                    case "CPRVAR:NAME":
                        Console.WriteLine(SanitizeString(GetLVarFromName(InstructionsToExecute[1]).val.ToString()));
                        break;
                    // MATH
                    case "AD:PRINT":
                        Console.WriteLine(float.Parse(InstructionsToExecute[1]) + float.Parse(InstructionsToExecute[2]));
                        break;
                    case "AD:VAR":
                        float _ = float.Parse(InstructionsToExecute[1]) + float.Parse(InstructionsToExecute[2]);
                        SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[3]), _);
                        break;
                    case "SB:PRINT":
                        Console.WriteLine(float.Parse(InstructionsToExecute[1]) - float.Parse(InstructionsToExecute[2]));
                        break;
                    case "SB:VAR":
                        _ = float.Parse(InstructionsToExecute[1]) - float.Parse(InstructionsToExecute[2]);
                        SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[3]), _);
                        break;
                    case "ML:PRINT":
                        Console.WriteLine(float.Parse(InstructionsToExecute[1]) * float.Parse(InstructionsToExecute[2]));
                        break;
                    case "ML:VAR":
                        _ = float.Parse(InstructionsToExecute[1]) * float.Parse(InstructionsToExecute[2]);
                        SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[3]), _);
                        break;
                    case "DV:PRINT":
                        Console.WriteLine(float.Parse(InstructionsToExecute[1]) / float.Parse(InstructionsToExecute[2]));
                        break;
                    case "DV:VAR":
                        _ = float.Parse(InstructionsToExecute[1]) / float.Parse(InstructionsToExecute[2]);
                        SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[3]), _);
                        break;
                    //
                    case "DF":
                        SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[1]), InstructionsToExecute[2]);
                        break;
                    case "GI:VAR":
                        Console.WriteLine("Press any key:");
                        ConsoleKeyInfo cki = Console.ReadKey();
                        SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[1]), cki.KeyChar);
                        break;
                    case "GI:VAR,NOPROMPT":
                        cki = Console.ReadKey();
                        SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[1]), cki.KeyChar);
                        break;
                }
            }
        }
    }
}
