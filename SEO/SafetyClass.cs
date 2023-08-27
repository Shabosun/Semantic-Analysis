using System;
using System.Collections.Generic;
using System.IO;
using System.Management;

namespace SEO
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

    public class SafetyClass
    {
        private USBInfo[] USBs;
        Form1 mainForm;
        public SafetyClass(Form1 _mainForm)
        {
            mainForm = _mainForm;
        }
        public void ReloadUSBsInfoEvent(object sender, EventArgs e)
        {
            ReloadUSBsInfo();
        }
        public void ReloadUSBsInfo()
        {
            try
            {
                List<USBInfo> usbs = new List<USBInfo>();

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
                USBs = usbs.ToArray();
                usbs.Clear();
                Validation();
                //this.Invoke(new Action(() => {
                //    comboBox1.SelectedIndex = -1;
                //    comboBox1.Items.Clear();
                //    foreach (var d in USBs)
                //    {
                //        comboBox1.Items.Add(d.DriveLetter);
                //    }
                //}));

            }
            catch (Exception)
            {

            }
        }
        public void Validation()
        {
            mainForm.BeginInvoke(new Action(delegate { mainForm.Disable(); }));
            if (USBs != null && USBs.Length > 0)
                foreach (var usb in USBs)
                {
                    if (File.Exists(usb.DriveLetter + ":\\" + "Key"))
                    {
                        string key;
                        using (StreamReader SR = new StreamReader(usb.DriveLetter + ":\\" + "Key"))
                            key = SR.ReadLine();
                        //key = key.Substring(0,key.Length - 2);
                        VigenereCipher vigenereCipher = new VigenereCipher("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890#.");
                        key = vigenereCipher.Decrypt(key, usb.SerialNumber);
                        string[] keyInfo = key.Split('#');
                        if (keyInfo.Length == 4)
                        {
                            DateTime dateTime = DateTime.Parse(keyInfo[1]);
                            if (keyInfo[0] == usb.SerialNumber && dateTime > DateTime.Now && keyInfo[2] == Environment.UserName)
                            {
                                string accessLVL = keyInfo[3];

                                switch (accessLVL)
                                {
                                    case "Enable":
                                        {
                                            mainForm.BeginInvoke(new Action(delegate { mainForm.Enable(); }));
                                            break;
                                        }
                                    case "Disable":
                                        {
                                            mainForm.BeginInvoke(new Action(delegate { mainForm.Disable(); }));
                                            break;
                                        }
                                    default:
                                        {
                                            mainForm.BeginInvoke(new Action(delegate { mainForm.Disable(); }));
                                            break;
                                        }
                                }

                            }
                        }
                        else
                        {
                            continue;
                        }
                        //MessageBox.Show("ez");
                    }

                }
        }
    }
}
