//using FaceLocalization.DLL;
//using AEditor.DLL;
using SEO.DLL;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SEO
{
    //Assembly a = Assembly.Load(System.IO.Path.GetFileNameWithoutExtension(arr[10]));
    //Object o = a.CreateInstance("DLLs.FirstDllClass");
    //Type t = a.GetType("DLLs.FirstDllClass");
    //Object[] numbers = new Object[1];
    //numbers[0] = 11;
    //MethodInfo mi = t.GetMethod("RetO");
    //groupBox1.Controls.Add( (Control)mi.Invoke(o, null));

    static class DLLAdapter
    {
        static DLLAdapter()
        {

        }
        public static List<DLLClass> DLLs;
        public static void Start()
        {
            string[] DLLNames = GettingDLLNames();
            //DLLs = new DLLClass[DLLNames.Length];

            List<DLLClass> l = new List<DLLClass>();

            for (int i = 0; i < DLLNames.Length; i++)
            {
                var dll = LoadDLL(DLLNames[i]);
                if (dll.t != null && dll.function != null)
                {
                    l.Add(dll);
                }
            }
            DLLs = l;//.ToArray();
        }
        public static string[] GettingDLLNames()
        {
            return Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");
        }
        public static DLLClass LoadDLL(string path)
        {
            return new DLLClass(path);
        }
        public static void LoadControls(Panel panel)
        {
            panel.Controls.Clear();
            foreach (var item in DLLs)
            {
                if (item.control != null)
                {
                    panel.Controls.Add(item.control);
                }
            }
        }
        public static void LoadMenuStrip(MenuStrip menuStrip)
        {
            //menuStrip.Items.Remove();
            //загрузка сведений о dll в menustrip
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)menuStrip.Items["pluginsToolStripMenuItem"];
            toolStripMenuItem.DropDownItems.Clear();
            foreach (var item in DLLs)
            {
                if (item.control != null)
                {
                    toolStripMenuItem.DropDownItems.Add(item.itt);
                }
            }
        }
        public static List<DLLClass> GetControls()
        {
            return DLLs;
        }

    }
}
