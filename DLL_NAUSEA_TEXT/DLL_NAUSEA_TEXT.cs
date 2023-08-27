using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



    public class DLL_NAUSEA_TEXT
    {
        CheckBox cb;
        public double PluginFunction(Dictionary<string, int> dictionary,  int count_of_words)
        {
            

        double classic_nausea = 0.0;
        int most_repeat_word = 0;
        

        foreach (var i in dictionary)
        {
            if (most_repeat_word < i.Value)
                most_repeat_word = i.Value;

        }

        classic_nausea = Math.Round(Math.Sqrt(most_repeat_word), 2);

        


        return classic_nausea;


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
            return "Классическая тошнота";
        }
        public string PluginDescription()
        {
            return "Рассчитывает классическую тошноту текста";
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
            return "2";
        }
        public string PluginRelease()
        {
            return "7.10.2022";
        }
        public string PluginFunctionReturnName()
        {
            return "classic_nausea";
        }
    }

