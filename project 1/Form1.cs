using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace project_1
{




    public partial class Form1 : Form
    {
        private CheckBox checkBoxAgree;
        private Button buttonAgree;

        public object Contols { get; private set; }
        public object Control { get; }

        public Form1()
        {
            InitializeComponent();
            this.button1.Click += button1_Click;
        }
        private void InitializeComponent(object obj)
        {
            this.checkBoxAgree = new CheckBox();
            this.button2 = new Button();

            // чекбокс
            this.checkBoxAgree.Text = "Я согласен";
            this.checkBoxAgree.AutoSize = true;
            this.checkBoxAgree.Location = new System.Drawing.Point(20, 20);
            this.checkBoxAgree.CheckedChanged += checkBoxAgree_CheckedChanged;

            // кнопка2
            this.button2.Text = "далее";
            this.button2.Location = new System.Drawing.Point(20, 60);
            this.button2.Enabled = false;
            this.button2.Click += button2_Click;
            this.button2.Enabled = this.checkBoxAgree.Checked;

            // форма
            this.Controls.Add(buttonAgree);
            this.ClientSize = new System.Drawing.Size(260, 120);
            this.Controls.Add(this.checkBoxAgree);
            this.Controls.Add(this.button2);
            this.Text = "Form1";
        }

        private void checkBoxAgree_CheckedChanged(object? sender, EventArgs e)
        {

        }

        private void button2_Click(object? sender, EventArgs e)
        {
            {

            }
            var form2 = new Form2();
            form2.Show();
            this.Hide();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}