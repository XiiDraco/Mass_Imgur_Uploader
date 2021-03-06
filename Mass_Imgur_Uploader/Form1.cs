﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Mass_Imgur_Uploader
{

    public partial class Form1 : Form
    {

        bool inverted = false;
        float value = 1.5f;

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread uploadImages = new Thread(new ThreadStart(delegate {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("Authorization", "Client-ID dfa7e126c16a262");
                    String intake = null;
                    richTextBox1.Invoke(new Action(() => intake = richTextBox1.Text));
                    String[] lines = intake.Trim().Split('\n');

                    progressBar1.Invoke(new Action(() => {
                        progressBar1.Maximum = lines.Length * 3;
                        progressBar1.Value = 0;
                        progressBar1.Step = 1;
                    }));
                    
                    System.Diagnostics.Debug.WriteLine("Test");
                    foreach (String s in lines)
                    {
                        try
                        {
                            if (!inverted)
                            {
                                NameValueCollection val = new NameValueCollection
                            {
                                { "image", Convert.ToBase64String(File.ReadAllBytes(s)) }
                            };

                                progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));

                                byte[] response = webClient.UploadValues("https://api.imgur.com/3/upload.xml", val);

                                progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));

                                XDocument doc = XDocument.Load(new MemoryStream(response));
                                richTextBox2.Invoke(new Action(() => richTextBox2.AppendText(doc.Root.Element("link").Value + "\n")));
                            }
                            else
                            {

                                Bitmap img = convertImage(new Bitmap(s));
                                for (int y = 0; y < img.Height; y++)
                                {
                                    for (int x = 0; x < img.Width; x++)
                                    {
                                        Color pixel = img.GetPixel(x, y);
                                        pixel = Color.FromArgb(255, Math.Min(255, (255 - pixel.R) + 50), Math.Min(255, (255 - pixel.G) + 50), Math.Min(255, (255 - pixel.B) + 50));

                                        pixel = Color.FromArgb(255, (int)Math.Max(0, Math.Min(255, ((((pixel.R / 255f) - 0.5f) * value) + 0.5f) * 255f)),
                                            (int)Math.Max(0, Math.Min(255, ((((pixel.G / 255f) - 0.5f) * value) + 0.5f) * 255f)),
                                            (int)Math.Max(0, Math.Min(255, ((((pixel.B / 255f) - 0.5f) * value) + 0.5f) * 255f)));

                                        img.SetPixel(x, y, pixel);
                                    }
                                }

                                byte[] imageBytes = null;

                                using (MemoryStream str = new MemoryStream())
                                {
                                    img.Save(str, System.Drawing.Imaging.ImageFormat.Png);
                                    imageBytes = str.ToArray();
                                }

                                progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));

                                NameValueCollection val = new NameValueCollection
                            {
                                { "image", Convert.ToBase64String(imageBytes) }
                            };

                                byte[] response = webClient.UploadValues("https://api.imgur.com/3/upload.xml", val);

                                XDocument doc = XDocument.Load(new MemoryStream(response));
                                richTextBox2.Invoke(new Action(() => richTextBox2.AppendText(doc.Root.Element("link").Value + "\n")));

                                progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
                            }


                        }
                        catch (FileNotFoundException e2)
                        {
                            System.Windows.Forms.MessageBox.Show(s + " was not found.");
                        }
                        catch (Exception e3)
                        {
                            System.Diagnostics.Debug.WriteLine(e3.ToString());
                        }
                        progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
                    }
                }
            }));
            uploadImages.Start();
        }

        public Bitmap convertImage(Image src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = @"C:\";
            open.Multiselect = true;
            if (open.ShowDialog() == DialogResult.OK)
            {
                foreach (String s in open.FileNames)
                {
                    this.richTextBox1.AppendText(s + "\n");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] lines = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (String s in lines)
            {
                this.richTextBox1.AppendText(s + "\n");
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            inverted = checkBox1.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            value = ((float)(numericUpDown1.Value) / 100f);
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }

}
