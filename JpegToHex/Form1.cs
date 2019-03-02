using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace JpegToHex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void open_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "JPEG Images| *.jpeg; *.jpg| All Files| *.*";
            openFileDialog.Title = "Open Image";
            // openFileDialog.InitialDirectory = @"C:\";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
            }
        }

        private void export_button_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "Text Files| *.txt| All Files| *.*";
                saveFileDialog.Title = "Select File to Export";
                // saveFileDialog.InitialDirectory = @"C:\";
                saveFileDialog.RestoreDirectory = true;
                //saveFileDialog.CheckFileExists = true;
                saveFileDialog.CheckPathExists = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Image img = pictureBox1.Image;
                    byte[] arr;
                    string str = "";
                    string init = "0x";
                    byte[] byte_str;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        arr = ms.ToArray();

                        File.WriteAllText(saveFileDialog.FileName, string.Empty); // clear file

                        progressBar1.Maximum = arr.Length;
                        progressBar1.Value = 0;
                        label1.Text = "";

                        int j = 0;
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (i == 0)
                            {
                                str ="uint8_t image[" + arr.Length + "] = " + 
                                    "{ \r\n" + init + arr[i].ToString("X" + 2) + ",";
                                j+=2;
                            }
                            else if (i < arr.Length - 1)
                            {

                                if (j >= 32)
                                {
                                    str += init + arr[i].ToString("X" + 2) + "," + "\r\n";
                                    j = 0;
                                }
                                else
                                {
                                    str += init + arr[i].ToString("X" + 2) + ",";
                                }
                                j++;
                            }
                            else if (i == arr.Length - 1)
                            {
                                str += init + arr[i].ToString("X" + 2) + "\r\n};";
                            }


                            progressBar1.Value = i;
                            Application.DoEvents();
                        }
                        byte_str = Encoding.ASCII.GetBytes(str);
                        using (FileStream fstream = new FileStream(saveFileDialog.FileName, FileMode.Append))
                        {
                            fstream.Write(byte_str, 0, str.Length);
                            fstream.Flush();
                        }
                        progressBar1.Value = 0;
                        label1.Text = "Done!";
                        //MessageBox.Show("OK");

                    }
                }
            }
        }
    }
}
