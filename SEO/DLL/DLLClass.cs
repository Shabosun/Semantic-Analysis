using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SEO.DLL
{
    class DLLClass
    {
        public Assembly a { get; private set; }
        public object o { get; private set; }
        public Type t { get; private set; }
        //public MethodInfo mi { get; private set; }
        //public object[] args { get; private set; }
        public MethodInfo function { get; private set; }
        public Control control { get; private set; }
        public string propertyName { get; private set; }
        public string priority { get; private set; }
        public string pluginName { get; private set; }
        public string description { get; private set; }
        public string author { get; private set; }
        public string version { get; private set; }
        public string release { get; private set; }
        public string returnName { get; private set; }

        public ToolStripMenuItem itt { get; private set; }
        //public object value { get; private set; }
        public DLLClass(string path)
        {

            try
            {
                //path = System.IO.Path.GetFileNameWithoutExtension(path);

                a = Assembly.LoadFrom(path);
                var name = System.IO.Path.GetFileNameWithoutExtension(path);
                //o = a.CreateInstance(System.IO.Path.GetFileNameWithoutExtension(path));
                o = a.CreateInstance(name);
                t = a.GetType(name);
                if (t != null)
                {
                    MethodInfo mi = t.GetMethod("PluginName");
                    if (mi != null)
                    {
                        pluginName = (string)mi.Invoke(o, null);

                        mi = t.GetMethod("PluginDescription");
                        description = (string)mi.Invoke(o, null);
                        mi = t.GetMethod("PluginAuthor");
                        author = (string)mi.Invoke(o, null);
                        mi = t.GetMethod("PluginVersion");
                        version = (string)mi.Invoke(o, null);
                        mi = t.GetMethod("PluginRelease");
                        release = (string)mi.Invoke(o, null);
                        mi = t.GetMethod("PluginFunctionReturnName");
                        returnName = (string)mi.Invoke(o, null);

                        //загрузка информации о плагинах для menustrip
                        ToolStripMenuItem PluginName = new ToolStripMenuItem("Plugin Name", null, new EventHandler(PluginInfo_Click));
                        ToolStripMenuItem PluginDescription = new ToolStripMenuItem("Plugin Description", null, new EventHandler(PluginInfo_Click));
                        ToolStripMenuItem PluginAuthor = new ToolStripMenuItem("Plugin Author", null, new EventHandler(PluginInfo_Click));
                        ToolStripMenuItem PluginVersion = new ToolStripMenuItem("Plugin Version", null, new EventHandler(PluginInfo_Click));
                        ToolStripMenuItem PluginInfo = new ToolStripMenuItem(pluginName, null, new ToolStripItem[] { PluginName, PluginDescription, PluginAuthor, PluginVersion });
                        PluginInfo.Click += new EventHandler(delegate { MessageBox.Show($"Название: {pluginName}\nОписание: {description}\nАвтор: {author}\nВерсия: {version}", $"О плагине {pluginName}" ); });
                        PluginInfo.DropDownItems.Add(PluginName);
                        
                        itt = PluginInfo;

                        mi = t.GetMethod("GetControl");
                        control = (Control)mi.Invoke(o, null);

                        ContextMenuStrip context = new ContextMenuStrip()
                        {
                            ShowImageMargin = false
                        };
                        ToolStripMenuItem tool = new ToolStripMenuItem("Delete the plugin")
                        {
                            Name = pluginName,
                        };
                        tool.Click += new EventHandler(deletePlugin_Click);
                        context.Items.Add(tool);
                        control.ContextMenuStrip = context;

                        mi = t.GetMethod("GetPropName");
                        propertyName = (string)mi.Invoke(o, null);

                        mi = t.GetMethod("PluginPriority");
                        priority = (string)mi.Invoke(o, null);

                        function = t.GetMethod("PluginFunction");

                    }
                }
            }
            catch (Exception)
            {

            }
        }
        private void deletePlugin_Click(object sender, EventArgs e)
        {
            DLLAdapter.DLLs.Remove(DLLAdapter.DLLs.Find(x => x.pluginName == ((ToolStripMenuItem)sender).Name));
            //VariablesClass.mainForm.BeginInvoke(new Action(delegate { VariablesClass.mainForm.reloadControls(); }));
            //reloadControls();
            //DLLAdapter.LoadControls(((CheckBox)sender).Parent as Panel);
        }

        private void PluginInfo_Click(object sender, EventArgs e)
        {
            MethodInfo mi = t.GetMethod((sender as ToolStripMenuItem).Text.Replace(" ", ""));
            MessageBox.Show((string)mi.Invoke(o, null), (sender as ToolStripMenuItem).Text);
        }
    }
}
