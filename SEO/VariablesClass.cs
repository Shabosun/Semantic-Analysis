//using FaceLocalization.DLL;
using System.Collections.Generic;
using System.Drawing;
using SEO.DLL;


namespace SEO
{
    internal static class VariablesClass
    {

        public static string text { get; set; }
        public static int count_of_symbols { get; set; }
        public static Dictionary<char, int> alphabet { get; set; }
        public static Dictionary<string, int> dictionary { get; set; }
        public static double classic_nausea { get; set; }
        public static double academic_nausea { get; set; }
        public static int count_of_words { get; set; }

        //public static Dictionary<string, int> dictionary { get; set; }
        //public static int count_of_symbols { get; set; }
        //public static int count_of_words { get; set; }
        
        
        internal static List<DLLClass> DLLs { get; set; }

        

        public static Form1 mainForm { get; set; }

       
    }
}
