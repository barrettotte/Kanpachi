using System;
using System.Text.RegularExpressions;

namespace Kanpachi.Lib{

    public static class RegexUtils{

        private static string ibmiObjRegex = @"[a-zA-Z@#$][a-zA-Z0-9_.@#$]{0,9}";
        
        // 10 character valid IBMi object name
        public static bool MatchIbmiObject(string objName){
            return Regex.IsMatch(objName, $"^{ibmiObjRegex}$");
        }

        // BOLIB/QRPGLESRC
        public static bool MatchSrcPfPath(string qsysPath){
            return Regex.IsMatch(qsysPath, $"^({ibmiObjRegex})\\/({ibmiObjRegex})$");
        }

        // BOLIB/QRPGLESRC/HELLORPG
        public static bool MatchSrcMbrPath(string qsysPath){
            return Regex.IsMatch(qsysPath, $"^({ibmiObjRegex})\\/({ibmiObjRegex})\\/({ibmiObjRegex})$");
        }

        // TODO: profile names
        // TODO: user
        // TODO: system name?
    }
}