using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;

using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Management;

namespace SEO
{
    public partial class Form1 : Form
    {
        string text;
        SafetyClass security;
      //  Dictionary<char, int> alphabet;
        Dictionary<KeyValuePair<string, int>, double> frequencyDictionary;
        int count_of_symbols = 0;
        ManagementEventWatcher watcher;
       // int count_of_words = 0;


        public Form1()
        {
            InitializeComponent();

            
           
            

            DLLAdapter.Start();
            DLLAdapter.LoadControls(DLLPanel);
            DLLAdapter.LoadMenuStrip(menuStrip1);
            VariablesClass.DLLs = DLLAdapter.GetControls();


            watcher = new ManagementEventWatcher();
            var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 or EventType = 3");
            watcher.EventArrived += new EventArrivedEventHandler(ReloadUSBsInfoEvent);
            watcher.Query = query;
            security = new SafetyClass(this);
            watcher.Start();

        }

        private void открытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log.SendActionLog("Выбор файла");
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Текстовый файл (*.txt) | *.txt";
            openFileDialog1.Title = "Открыть текстовый файл";

            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            
            try
            {
                if (richTextBox1.Text == null)
                {
                    
                    StreamReader f = new StreamReader(openFileDialog1.FileName);

                    text = f.ReadToEnd();

                    richTextBox1.Text = text;

                    f.Close();
                }
                else
                {
                    richTextBox1.Clear();
                    StreamReader f = new StreamReader(openFileDialog1.FileName);

                    text = f.ReadToEnd();

                    richTextBox1.Text = text;

                    f.Close();
                }
               
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
                MessageBox.Show("Выберите другой файл");
                return;
            }

            VariablesClass.text = text;
            VariablesClass.count_of_symbols = count_of_symbols;
            //VariablesClass.alphabet = alphabet;




        }

        //pdf report
        private void сохранитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "PDF файл (*.pdf) | *.pdf";
                saveFileDialog1.FileName = "Report";
                saveFileDialog1.Title = "Сохранить отчет";

                if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

                string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
                var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                //var font = new Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);
                var titleFont = new Font(baseFont, 16, iTextSharp.text.Font.BOLD);
                var textFont = new Font(baseFont, 12, iTextSharp.text.Font.NORMAL);


                PdfPTable tableStatistic = new PdfPTable(dataGridViewStatistic.Columns.Count);
                PdfPTable tableWords = new PdfPTable(dataGridViewWords.Columns.Count);


                //string folderPath = "C:\\Users\\User\\Desktop\\PDFs\\";
                using (FileStream stream = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);

                    PdfWriter.GetInstance(pdfDoc, stream);

                    pdfDoc.Open();


                    pdfDoc.AddCreationDate();
                    pdfDoc.Add(new Chunk("Teкст", titleFont));

                    pdfDoc.Add(new Paragraph(richTextBox1.Text, textFont));

                    pdfDoc.NewPage();
                    pdfDoc.Add(new Chunk("Статистика:", titleFont));

                    PdfPCell headerTableStatistic = new PdfPCell();

                    for (int j = 0; j < dataGridViewStatistic.Columns.Count; j++)
                    {
                        headerTableStatistic = new PdfPCell(new Phrase(dataGridViewStatistic.Columns[j].HeaderText, textFont));
                        headerTableStatistic.BackgroundColor = iTextSharp.text.BaseColor.GRAY;
                        tableStatistic.AddCell(headerTableStatistic);

                    }



                    for (int i = 0; i < dataGridViewStatistic.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridViewStatistic.Columns.Count; j++)
                        {

                            tableStatistic.AddCell(new Phrase(dataGridViewStatistic.Rows[i].Cells[j].Value.ToString(), textFont));


                        }
                    }

                    pdfDoc.Add(tableStatistic);

                    pdfDoc.NewPage();
                    pdfDoc.Add(new Chunk("Слова: ", titleFont));

                    PdfPCell headerTableWords = new PdfPCell();

                    for (int i = 0; i < dataGridViewWords.Columns.Count; i++)
                    {
                        headerTableWords = new PdfPCell(new Phrase(dataGridViewWords.Columns[i].HeaderText, textFont));
                        headerTableWords.BackgroundColor = iTextSharp.text.BaseColor.GRAY;
                        tableWords.AddCell(headerTableWords);
                    }
                    for (int i = 0; i < dataGridViewWords.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridViewWords.Columns.Count; j++)
                        {

                            tableWords.AddCell(new Phrase(dataGridViewWords.Rows[i].Cells[j].Value.ToString(), textFont)) ; //ошибка


                        }
                    }


                    pdfDoc.Add(tableWords);



                    pdfDoc.Close();
                    stream.Close();
                    Log.SendActionLog("Сохранение PDF отчета");
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            };

            
            


        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void DLLsStart(string Priority)
        {
            foreach (var item in VariablesClass.DLLs)
            {
                if (item.control != null && item.priority == Priority)
                {
                    object o = item.control.GetType().GetProperty(item.propertyName).GetValue(item.control, null);
                    if (Convert.ToBoolean(item.control.GetType().GetProperty(item.propertyName).GetValue(item.control, null)))
                    {
                        var PI = item.function.GetParameters();
                        object[] Parameters = new object[PI.Length];
                        for (int i = 0; i < PI.Length; i++)
                        {
                        
                            try
                            {
                            
                                Parameters[i] = (typeof(VariablesClass).GetProperty(PI[i].Name).GetValue(null));
                                
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                        var returnValue = item.function.Invoke(item.o, Parameters);
                        

                        if (returnValue != null)
                        {
                            
                            typeof(VariablesClass).GetProperty(item.returnName).SetValue(null, returnValue);
                            

                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            
            dataGridViewStatistic.Columns.Add("indicatorNameColumn", "Наименование показателя");
            dataGridViewStatistic.Columns.Add("valueColumn", "Значение");

            //dataGridViewStatistic.Columns.Add("keyColumn", "key");
            //dataGridViewStatistic.Columns.Add("valueColumn", "value");
            
            dataGridViewStatistic.Columns[0].Width = dataGridViewStatistic.Width / dataGridViewStatistic.Columns.Count;
            dataGridViewStatistic.Columns[1].Width = dataGridViewStatistic.Width / dataGridViewStatistic.Columns.Count;
            
            try
            {
                
                DLLsStart("1");
                VariablesClass.count_of_words = countOfWords(text);
                DLLsStart("2");

                if (VariablesClass.alphabet != null)
                {

                    
                    var sortedDict = VariablesClass.alphabet.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

                    dataGridViewStatistic.Rows.Add(new string[] {"Количество символов", countOfSymbols(text).ToString() });
                    dataGridViewStatistic.Rows.Add(new string[] { "Количество символов без пробелов", countOfWordsWithoutSpace(text).ToString()});
                    dataGridViewStatistic.Rows.Add(new string[] { "Количество слов" , countOfWords(text).ToString()});
                    dataGridViewStatistic.Rows.Add(new string[] { "Количество уникальных слов", countOfUniqueWords(VariablesClass.dictionary).ToString()});
                    dataGridViewStatistic.Rows.Add(new string[] { "Классическая тошнота", VariablesClass.classic_nausea.ToString()});
                    //dataGridViewStatistic.Rows.Add(new string[] { "Классическая тошнота", avg_classic_nausea(VariablesClass.dictionary, countOfWords()).ToString() });
                    dataGridViewStatistic.Rows.Add(new string[] { "Академичекая тошнота", VariablesClass.academic_nausea.ToString()});




                    foreach (var a in sortedDict)
                    {
                        
                        dataGridViewWords.Rows.Add(new string[] {a.Key.ToString(), a.Value.ToString() });

                      
                    }
                }

                else { MessageBox.Show("ne ok"); }
            }
            catch(Exception exception)
            {
                MessageBox.Show("Загрузите файл");
            }

           


        }

        //количество слов в тексте без пробелов
        private int countOfWordsWithoutSpace(string text)
        {
            string new_text = new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
            new_text = new_text.Replace(" ", "");


            return new_text.Length;
             
        }
        //количество символов в тексте
        private int countOfSymbols(string text)
        {
            string new_text = new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
          
            return new_text.Length;

        }
        //количество слов
        private int countOfWords(string text)
        {
            string[] words = text.Split();

            return words.Length;
        }
        //количество уникальных слов
        private int countOfUniqueWords(Dictionary<string, int> dictionary)
        {
            int cnt = 0;
            
            foreach(KeyValuePair<string,int> keyValuePair in dictionary)
            {
                if (keyValuePair.Value == 1) cnt++;
            }
            
            return cnt;
        }

        //частотность
        private Dictionary<KeyValuePair<string, int>, double> frequencyOfWords(Dictionary<string, int> dictionary, int count_of_words)
        {
            Dictionary<KeyValuePair<string, int>, double> frequencyDictionary = new Dictionary<KeyValuePair<string, int>, double>();
            double temp;

            foreach (var i in dictionary)
            {

                temp = Math.Round((double)i.Value / (double)count_of_words, 2);
                frequencyDictionary.Add(i, temp);

                //frequencyDictionary.Add(i, Math.Round((double)(i.Value / count_of_words), 2));
            }




            return frequencyDictionary;
        }


        private void buttonHandle_Click(object sender, EventArgs e)
        {
           
            if (text != null)
            {
                try {
                    
                    ClearTables();
                    DLLsStart("1");
                    VariablesClass.count_of_words = countOfWords(text);
                    if (VariablesClass.count_of_words != null)
                    {


                        DLLsStart("2");
                        

                    }
                    else
                    {
                        throw new Exception("Функция подсчета количества строк не сработала");
                    }
                    if (VariablesClass.alphabet != null)
                    {
                        dataGridViewStatistic.Columns.Add("indicatorNameColumn", "Наименование показателя");
                        dataGridViewStatistic.Columns.Add("valueColumn", "Значение");


                        dataGridViewStatistic.Rows.Add(new string[] { "Количество символов", countOfSymbols(text).ToString() });
                        dataGridViewStatistic.Rows.Add(new string[] { "Количество символов без пробелов", countOfWordsWithoutSpace(text).ToString() });
                        dataGridViewStatistic.Rows.Add(new string[] { "Количество слов", countOfWords(text).ToString() });
                        
                        if(VariablesClass.dictionary != null)
                        {
                            dataGridViewStatistic.Rows.Add(new string[] { "Количество уникальных слов", countOfUniqueWords(VariablesClass.dictionary).ToString() });

                            frequencyDictionary = frequencyOfWords(VariablesClass.dictionary, countOfWords(text));

                                dataGridViewWords.Columns.Add("wordColumn", "Слово");
                                dataGridViewWords.Columns.Add("countColumn", "Количество");
                                dataGridViewWords.Columns.Add("frequencyColumn", "Частота");




                            foreach (var i in frequencyDictionary)
                                dataGridViewWords.Rows.Add(new string[] { i.Key.Key, i.Key.Value.ToString(), i.Value.ToString() });



                        }
                        if (VariablesClass.classic_nausea != 0)
                            dataGridViewStatistic.Rows.Add(new string[] { "Классическая тошнота", VariablesClass.classic_nausea.ToString() });
                        if(VariablesClass.academic_nausea != 0)
                            dataGridViewStatistic.Rows.Add(new string[] { "Академичекая тошнота", VariablesClass.academic_nausea.ToString() });




                        //foreach (var a in sortedDict)
                        //{

                        //    dataGridViewWords.Rows.Add(new string[] { a.Key.ToString(), a.Value.ToString() });


                        //}
                        //после цикла не надо ничего вставлять, не сработает, потому что кидает в catch






                    }







                    Log.SendActionLog("Обработка текста");

                }
                catch (Exception ex) {
                    MessageBox.Show("При работе с плагинами произошла ошибка.\nОшибка: " + ex.Message);
                    MessageBox.Show("Включите плагин <<Подсчет символов>>.");

                };

            }
            else { MessageBox.Show("Загрузите файл"); }
        }

        private void ClearTables()
        {
            dataGridViewStatistic.Rows.Clear();
            dataGridViewStatistic.Columns.Clear();

            dataGridViewWords.Rows.Clear();
            dataGridViewWords.Columns.Clear();
           
        }

        internal void Enable()
        {
            buttonHandle.Enabled = menuStrip1.Enabled = DLLPanel.Enabled =  true;
            

        }

        internal void Disable()
        {
            buttonHandle.Enabled = menuStrip1.Enabled = DLLPanel.Enabled = false;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log.SendActionLog("Включение программы");
            security.ReloadUSBsInfo();
        }

        private void ReloadUSBsInfoEvent(object sender, EventArgs e)
        {
            security.ReloadUSBsInfo();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.SendActionLog("Программа завершила работу");
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Семантический анализ файлов\nВерсия 1.0\nРазработчик: Осипенко А.Ю.\nДанная программа предназначена для семантического анализа текстовых файлов.", "О программе");
        }

        private void pluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }




}
