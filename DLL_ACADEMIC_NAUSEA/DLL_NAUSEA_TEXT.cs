using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



    public class DLL_ACADEMIC_NAUSEA
    {
        CheckBox cb;
        public double PluginFunction(Dictionary<string, int> dictionary,  int count_of_words)
        {
            

        double academic_nausea = 0.0;
        int most_repeat_word = 0;
        

        foreach (var i in dictionary)
        {
            if (most_repeat_word < i.Value)
                most_repeat_word = i.Value;

        }

        academic_nausea = Math.Round(((double)most_repeat_word / count_of_words) * 100,2 );

        


        return academic_nausea;


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
            return "Академическая тошнота";
        }
        public string PluginDescription()
        {
            return "Подсчитывает академическую тошноту текста";
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
            return "8.10.2022";
        }
        public string PluginFunctionReturnName()
        {
            return "academic_nausea";
        }
    }

