using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace KeyGenFaceApp
{
    public struct USBInfo
    {
        public string SerialNumber;
        public char DriveLetter;
        public USBInfo(string SN, char DL)
        {
            SerialNumber = SN;
            DriveLetter = DL;
        }
    }

    public partial class KeyGenForm : Form
    {
        private static USBInfo[] USBs;

        public KeyGenForm()
        {
            InitializeComponent();
            //ReloadUSBsInfo();
            dateTimePicker1.Value = dateTimePicker1.Value.AddDays(7);
            textBox1.Text = Environment.UserName;
            comboBox2.SelectedIndex = 0;


            var watcher = new ManagementEventWatcher();
            var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 or EventType = 3");
            watcher.EventArrived += new EventArrivedEventHandler(ReloadUSBsInfoEvent);
            watcher.Query = query;
            watcher.Start();

            //ReloadUSBsInfo();
        }
        public void ReloadUSBsInfoEvent(object sender, EventArgs e)
        {
            ReloadUSBsInfo();
        }
        public void ReloadUSBsInfo()
        {
            List<USBInfo> usbs = new List<USBInfo>();

            try
            {
                ManagementObjectSearcher diskDrives = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
                foreach (ManagementObject diskDrive in diskDrives.Get())
                {
                    string DeviceID = diskDrive["DeviceID"].ToString();
                    string DriveLetter = "";
                    string DriveDescription = "";

                    // associate physical disks with partitions
                    ManagementObjectSearcher partitionSearcher = new ManagementObjectSearcher(String.Format(
                        "associators of {{Win32_DiskDrive.DeviceID='{0}'}} where AssocClass = Win32_DiskDriveToDiskPartition",
                        diskDrive["DeviceID"]));

                    foreach (ManagementObject partition in partitionSearcher.Get())
                    {
                        // associate partitions with logical disks (drive letter volumes)
                        ManagementObjectSearcher logicalSearcher = new ManagementObjectSearcher(String.Format(
                            "associators of {{Win32_DiskPartition.DeviceID='{0}'}} where AssocClass = Win32_LogicalDiskToPartition",
                            partition["DeviceID"]));

                        foreach (ManagementObject logical in logicalSearcher.Get())
                        {
                            // finally find the logical disk entry to determine the volume name
                            ManagementObjectSearcher volumeSearcher = new ManagementObjectSearcher(String.Format(
                                "select * from Win32_LogicalDisk where Name='{0}'",
                                logical["Name"]));

                            foreach (ManagementObject volume in volumeSearcher.Get())
                            {
                                DriveLetter = volume["Name"].ToString();
                                if (volume["VolumeName"] != null)
                                    DriveDescription = volume["VolumeName"].ToString();

                                char VolumeLetter = DriveLetter[0];
                                string VolumeName = DriveDescription;
                                string Manufacturer = (string)diskDrive["Manufacturer"];
                                string MediaType = (string)diskDrive["MediaType"];
                                string Model = (string)diskDrive["Model"];
                                string SerialNumber = (string)diskDrive["SerialNumber"];
                                long Size = Convert.ToInt64(volume["Size"]);
                                long FreeSpace = Convert.ToInt64(volume["FreeSpace"]);
                                usbs.Add(new USBInfo(SerialNumber, VolumeLetter));
                                //textBox1.Text += VolumeLetter + "   " + SerialNumber + Environment.NewLine;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            USBs = usbs.ToArray();

            this.Invoke(new Action(() => {
                comboBox1.SelectedIndex = -1;
                comboBox1.Items.Clear();
                foreach (var d in USBs)
                {
                    comboBox1.Items.Add(d.DriveLetter);
                }
            }));
            usbs.Clear();
        }
        private void BGenerate_Click(object sender, EventArgs e)
        {
            string path = USBs[comboBox1.SelectedIndex].DriveLetter + ":\\Key";

            string key = USBs[comboBox1.SelectedIndex].SerialNumber + '#' + dateTimePicker1.Value.Date.ToString("dd.MM.yyyy") + '#' + textBox1.Text + '#' + comboBox2.Text;

            //VigenereCipher vigenereCipher = new VigenereCipher("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ1234567890#.");
            VigenereCipher vigenereCipher = new VigenereCipher("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890#.");
            //ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890#.
            key = vigenereCipher.Encrypt(key, USBs[comboBox1.SelectedIndex].SerialNumber);
            //string a = vigenereCipher.Decrypt(key, USBs[comboBox1.SelectedIndex].SerialNumber);
            StreamWriter SW = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write));
            SW.Write(key);
            SW.Close();
            File.SetAttributes(path, FileAttributes.Hidden);
            for (int i = 0; i <= 100; i++)
            {
                progressBar1.Value = i;
            }
        }
        private void ChangeVeref(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && textBox1.Text != "" && dateTimePicker1.Value > DateTime.Now && comboBox2.SelectedIndex >= 0)
            {
                BGenerate.Enabled = true;
            }
            else BGenerate.Enabled = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ReloadUSBsInfo();
        }
        private void BCurrentUser_Click(object sender, EventArgs e)
        {
            textBox1.Text = Environment.UserName;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
