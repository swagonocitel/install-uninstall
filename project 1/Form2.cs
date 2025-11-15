using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project_1
{
    public partial class Form2 : Form
    {
        private Button buttonAgree;
        public object Contols { get; private set; }
        public object Control { get; }

        public Form2()
        {
            InitializeComponent();
            this.button1.Click += button1_Click;
            this.button2.Click += button2_Click;
            this.button3.Click += button3_Click;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string myText = textBox1.Text;

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Выберите папку для установки программы";
                folderDialog.ShowNewFolderButton = true; // разрешаем создавать новую папку
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // вставляется выбранный путь в textbox1
                    textBox1.Text = folderDialog.SelectedPath;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
        private void CheckInstallPath()
        {
            bool isPathValid = !string.IsNullOrWhiteSpace(textBox1.Text);
            button3.Enabled = isPathValid;


        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CheckInstallPath();
            button3.Enabled = !string.IsNullOrWhiteSpace(textBox1.Text);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                
            {
                MessageBox.Show("Путь установки не был выбран. Пожалуйста, укажите путь", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
             

            }
            else
            {
                Form3 form3 = new Form3();
                form3.Show();
                this.Hide();
                
                
            }
        }
        
    }
}
