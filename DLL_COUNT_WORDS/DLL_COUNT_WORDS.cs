using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



    public class DLL_COUNT_WORDS
    {
        CheckBox cb;
        public Dictionary<string, int> PluginFunction(string text)
        {
        
            text = new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
            text = text.ToLower();
            string[] words = text.Split();
            



            Dictionary<string, int> dictionary = new Dictionary<string, int>();
           
            
            foreach (string word in words)
            {
                if (dictionary.ContainsKey(word)) dictionary[word]++;
                else dictionary[word] = 1;
            }
 
            return dictionary;


        }
        public object GetControl()
        {
            cb = new CheckBox()
            {
                Text = PluginName(),
                Checked = false,
                Dock = DockStyle.Top,
                Name = "CBFacesRationing"
                //Name = "CBEcho"
            };
            cb.CheckedChanged += new EventHandler(Control_Click);


            return cb;
        }
        public void Control_Click(object sender, EventArgs e)
        {

        }
        public void TextBox_TextChange(object sender, EventArgs e)
        {
            ((TextBox)sender).Parent.Name = ((TextBox)sender).Parent.Controls["TBW"].Text + "_" + ((TextBox)sender).Parent.Controls["TBH"].Text;

        }
        public string GetPropName()
        {
            return "Checked";
        }
        public string PluginName()
        {
            return "Подсчет слов";
        }
        public string PluginDescription()
        {
            return "Считает количество слов в тексте.";
        }
        public string PluginAuthor()
        {
            return "Осипенко Артур";
        }
        public string PluginVersion()
        {
            return "1.1";
        }
        public string PluginPriority()
        {
            return "1";
            
        }
        public string PluginRelease()
        {
            return "7.10.2022";
        }
        public string PluginFunctionReturnName()
        {
            return "dictionary";
        }
    }

