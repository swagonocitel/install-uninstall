namespace project_1
{
    partial class Form5
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            progressBar1 = new ProgressBar();
            textBox1 = new TextBox();
            checkBoxConfirm = new CheckBox();
            buttonDelete = new Button();
            buttonCancel = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(35, 69);
            progressBar1.Margin = new Padding(4, 3, 4, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(467, 27);
            progressBar1.TabIndex = 0;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(35, 104);
            textBox1.Margin = new Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(466, 23);
            textBox1.TabIndex = 1;
            // 
            // checkBoxConfirm
            // 
            checkBoxConfirm.AutoSize = true;
            checkBoxConfirm.Location = new Point(35, 150);
            checkBoxConfirm.Margin = new Padding(4, 3, 4, 3);
            checkBoxConfirm.Name = "checkBoxConfirm";
            checkBoxConfirm.Size = new Size(234, 19);
            checkBoxConfirm.TabIndex = 2;
            checkBoxConfirm.Text = "Я подтверждаю удаление программы";
            checkBoxConfirm.UseVisualStyleBackColor = true;
            checkBoxConfirm.CheckedChanged += checkBoxConfirm_CheckedChanged;
            // 
            // buttonDelete
            // 
            buttonDelete.Location = new Point(309, 196);
            buttonDelete.Margin = new Padding(4, 3, 4, 3);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new Size(88, 27);
            buttonDelete.TabIndex = 3;
            buttonDelete.Text = "Удалить";
            buttonDelete.UseVisualStyleBackColor = true;
            buttonDelete.Click += buttonDelete_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(414, 196);
            buttonCancel.Margin = new Padding(4, 3, 4, 3);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(88, 27);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Отмена";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(35, 35);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(156, 15);
            label1.TabIndex = 5;
            label1.Text = "Удаление программы с ПК";
            // 
            // Form5
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(537, 254);
            Controls.Add(label1);
            Controls.Add(buttonCancel);
            Controls.Add(buttonDelete);
            Controls.Add(checkBoxConfirm);
            Controls.Add(textBox1);
            Controls.Add(progressBar1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form5";
            Text = "Удаление программы";
            Load += Form5_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBoxConfirm;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
    }
}