// Lic:
// Bubble CLI
// Main
// 
// 
// 
// (c) Jeroen P. Broks, 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 19.04.28
// EndLic

using System;
using System.Collections.Generic;
using System.Diagnostics;    
using System.Text;
using TrickyUnits;
using UseJCR6;

namespace Bubble {

    class BCLIMain {
        static void ErrorHandler(string ct, string msg, string trace) {
            var of = Console.ForegroundColor;
            var ob = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{ct} error");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            if (trace != "") {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(trace);
            }
            if (Debugger.IsAttached) {
                Console.WriteLine("Hit any key to shut the program down!");
                Console.ReadKey(true);
            }
            var mcode = msg.Length % 255; if (mcode == 0) mcode++;
            Environment.Exit(mcode);
        }

        static void Main(string[] args) {
            JCR6_lzma.Init();
            JCR6_zlib.Init();
            JCR6_jxsrcca.Init();
            SBubble.Init("CLI", ErrorHandler);
            var mainfile = "";
            foreach(string e in SBubble.ResFiles) {
                var ce = e.ToUpper();
                if (ce == "MAIN.LUA" || qstr.Suffixed(ce, "/MAIN.LUA"))
                    mainfile = e;
            }
            if (mainfile == "") ErrorHandler("Bubble", "No main script could be found", "");
            SBubble.NewState("MAIN", mainfile);
            object[] r=null;
            string cmd="";
            try {
                cmd = $"assert(main and type(main)==\"function\",\"No 'main' function found!\")\nreturn main({SBubble.StringArray2Lua(args)},\"{MKL.MyExe.Replace("\\", "/")}\")";
                r = SBubble.State("MAIN").DoString(cmd);
            } catch (Exception e) {
                ErrorHandler("Main call", e.Message, cmd);
            }
            long exitcode = 0;
            try {
                if (r != null && r[0]!=null) exitcode = (long)(r[0]);
            } catch (Exception e) {
                ErrorHandler("Main Function", "Main function must either return 'nil' or an integer value!", $"{e.Message}\n\n{r[0].GetType()}");
            }
            Environment.Exit((int)exitcode);
        }
        
    }
}

