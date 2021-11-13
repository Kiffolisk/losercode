using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LCode_Interpreter
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
    class Loop
    {
        public long line;
        public long endLine;
        public string name;
        public Loop(string name, long line)
        {
            this.line = line;
            this.name = name;
        }
    }
    class MainClass
    {

        public static LVar[] SAVEDVARS = new LVar[2048];
        public static Loop[] LOOPS = new Loop[2048];
        public static int CURRENTPOINTERPOSITION = 0;

        public static bool inLoop = false;
        public static long loopEndPos = 0;

        public static bool isLoadingLoop = false;

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

        static Loop GetLoopFromName(string name)
        {
            Loop toreturn = null;
            foreach (Loop var in LOOPS)
            {
                if (var.name == name)
                {
                    toreturn = var;
                }
            }
            return toreturn;
        }

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("\tWindows: LCode_Interpreter.exe [filename]");
                Console.WriteLine("\tLinux: mono LCode_Interpreter.exe [filename]");
            }
            else
            {
                string FileFolder = AppDomain.CurrentDomain.BaseDirectory + args[0];
                long LineCount = File.ReadAllText(FileFolder).Split('\n').Length;
                long curStep = 0;
                for (curStep = 0; curStep < LineCount; curStep++)
                {
                    string fileLine = File.ReadAllText(FileFolder).Split('\n')[curStep];
                    string[] InstructionsToExecute = Regex.Split(fileLine, " (?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    switch (InstructionsToExecute[0])
                    {
                        // 
                        case "CPR":
                            if (isLoadingLoop)
                                break;
                            Console.WriteLine(SanitizeString(InstructionsToExecute[1]));
                            break;
                        case "MPR":
                            if (isLoadingLoop)
                                break;
                            CURRENTPOINTERPOSITION++;
                            break;
                        case "MPL":
                            if (isLoadingLoop)
                                break;
                            CURRENTPOINTERPOSITION--;
                            break;
                        case "STARTGOTO":
                            if (inLoop)
                            {
                                // You're not meant to do this.
                                throw new Exception("STARTGOTO in loop code execution");
                            }
                            isLoadingLoop = true;
                            LOOPS[CURRENTPOINTERPOSITION] = new Loop(SanitizeString(InstructionsToExecute[1]), curStep + 1);
                            break;
                        case "ENDGOTO":
                            if (inLoop)
                            {
                                inLoop = false;
                                curStep = loopEndPos;
                            }
                            else
                            {
                                isLoadingLoop = false;
                                LOOPS[CURRENTPOINTERPOSITION].endLine = curStep;
                            }
                            break;
                        case "GOTO":
                            if (isLoadingLoop)
                                break;
                            loopEndPos = curStep + 1;
                            inLoop = true;
                            curStep = LOOPS[CURRENTPOINTERPOSITION].line;
                            break;
                        case "MPTO":
                            if (isLoadingLoop)
                                break;
                            CURRENTPOINTERPOSITION = int.Parse(InstructionsToExecute[1]);
                            break;
                        case "MPRNUM":
                            if (isLoadingLoop)
                                break;
                            CURRENTPOINTERPOSITION += int.Parse(InstructionsToExecute[1]);
                            break;
                        case "MPLNUM":
                            if (isLoadingLoop)
                                break;
                            CURRENTPOINTERPOSITION -= int.Parse(InstructionsToExecute[1]);
                            break;
                        case "CPRVAR":
                            if (isLoadingLoop)
                                break;
                            Console.WriteLine(SanitizeString(SAVEDVARS[CURRENTPOINTERPOSITION].val.ToString()));
                            break;
                        case "CPRVAR:NAME":
                            if (isLoadingLoop)
                                break;
                            Console.WriteLine(SanitizeString(GetLVarFromName(InstructionsToExecute[1]).val.ToString()));
                            break;
                        case "ADD":
                            if (isLoadingLoop)
                                break;
                            int pos1 = int.Parse(SanitizeString(InstructionsToExecute[1]));
                            int pos2 = int.Parse(SanitizeString(InstructionsToExecute[2]));

                            object val1 = SAVEDVARS[pos1].val;
                            object val2 = SAVEDVARS[pos2].val;

                            int int1 = int.Parse(val1.ToString());
                            int int2 = int.Parse(val2.ToString());

                            SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[3]), (int1 + int2));
                            break;
                        case "SUB":
                            if (isLoadingLoop)
                                break;
                            pos1 = int.Parse(SanitizeString(InstructionsToExecute[1]));
                            pos2 = int.Parse(SanitizeString(InstructionsToExecute[2]));

                            val1 = SAVEDVARS[pos1].val;
                            val2 = SAVEDVARS[pos2].val;

                            int1 = int.Parse(val1.ToString());
                            int2 = int.Parse(val2.ToString());

                            SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[3]), (int1 - int2));
                            break;
                        case "MUL":
                            if (isLoadingLoop)
                                break;
                            pos1 = int.Parse(SanitizeString(InstructionsToExecute[1]));
                            pos2 = int.Parse(SanitizeString(InstructionsToExecute[2]));

                            val1 = SAVEDVARS[pos1].val;
                            val2 = SAVEDVARS[pos2].val;

                            int1 = int.Parse(val1.ToString());
                            int2 = int.Parse(val2.ToString());

                            SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[3]), (int1 * int2));
                            break;
                        case "DIV":
                            if (isLoadingLoop)
                                break;
                            pos1 = int.Parse(SanitizeString(InstructionsToExecute[1]));
                            pos2 = int.Parse(SanitizeString(InstructionsToExecute[2]));

                            val1 = SAVEDVARS[pos1].val;
                            val2 = SAVEDVARS[pos2].val;

                            int1 = int.Parse(val1.ToString());
                            int2 = int.Parse(val2.ToString());

                            SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[3]), (int1 / int2));
                            break;
                        case "MOD":
                            if (isLoadingLoop)
                                break;
                            pos1 = int.Parse(SanitizeString(InstructionsToExecute[1]));
                            pos2 = int.Parse(SanitizeString(InstructionsToExecute[2]));

                            val1 = SAVEDVARS[pos1].val;
                            val2 = SAVEDVARS[pos2].val;

                            int1 = int.Parse(val1.ToString());
                            int2 = int.Parse(val2.ToString());

                            SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[3]), (int1 % int2));
                            break;
                        case "SET":
                            if (isLoadingLoop)
                                break;
                            int posset = int.Parse(SanitizeString(InstructionsToExecute[1]));
                            int posset2 = int.Parse(SanitizeString(InstructionsToExecute[2]));
                            SAVEDVARS[posset].val = SAVEDVARS[posset2].val;
                            break;
                        case "MVAR":
                            if (isLoadingLoop)
                                break;
                            SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[1]), InstructionsToExecute[2]);
                            break;
                        case "GI:VAR":
                            if (isLoadingLoop)
                                break;
                            Console.WriteLine("Press any key:");
                            ConsoleKeyInfo cki = Console.ReadKey();
                            SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[1]), cki.KeyChar);
                            break;
                        case "GI:VAR,NOPROMPT":
                            if (isLoadingLoop)
                                break;
                            cki = Console.ReadKey();
                            SAVEDVARS[CURRENTPOINTERPOSITION] = new LVar(SanitizeString(InstructionsToExecute[1]), cki.KeyChar);
                            break;
                    }
                }
            }
        }
    }
}
