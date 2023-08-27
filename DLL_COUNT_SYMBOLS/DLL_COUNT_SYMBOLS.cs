using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



    public class DLL_COUNT_SYMBOLS
    {
        CheckBox cb;
        public Dictionary<char, int> PluginFunction(string text)
        {
            Dictionary<char, int> alphabet = new Dictionary<char, int>();
       
            //Удаляем знаки препинания
            text = new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
        
        
            text = text.ToLower();


            foreach (char symbol in text)
            {
                if (alphabet.ContainsKey(symbol)) alphabet[symbol]++;
                else alphabet[symbol] = 1;
            }

            return alphabet;


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
            return "Подсчет символов";
        }
        public string PluginDescription()
        {
            return "Подсчитывает количество символов в тексте";
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
            return "7.11.2022";
        }
        public string PluginFunctionReturnName()
        {
            return "alphabet";
        }
    }

