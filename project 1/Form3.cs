using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project_1
{
    public partial class Form3 : Form
    {
        private Button buttonAgree;
        private CheckBox checkBoxAgree;
        public object Contols { get; private set; }
        public object Control { get; }
        private bool isForm4Opende = false;

        // Статические свойства для хранения выбора пользователя
        public static bool CreateDesktopShortcut { get; set; }
        public static bool CreateStartMenuShortcut { get; set; }

        public Form3()
        {
            InitializeComponent();
            button2.Click += button2_Click;
            //button1.Enabled = false;
            checkBox1.CheckedChanged += CheckBox_CheckedChanged;
            checkBox2.CheckedChanged += CheckBox_CheckedChanged;
            button1.Click += button1_Click;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Отключаем кнопку чтобы предотвратить повторное нажатие
            //button1.Enabled = false;
            if (isForm4Opende) return;
            if (!checkBox1.Checked && !checkBox2.Checked)
            {
                MessageBox.Show("Чтобы продолжить, поставьте нужные галочки для установки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Сохраняем выбор пользователя
                CreateDesktopShortcut = checkBox1.Checked;
                CreateStartMenuShortcut = checkBox2.Checked;

                button1.Enabled = false;
                isForm4Opende = true;
                Form4 form4 = new Form4();
                form4.Show();
                this.Hide();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = checkBox1.Checked || checkBox2.Checked;
        }
    }
}