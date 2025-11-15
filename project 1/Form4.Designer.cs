namespace project_1
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ProgressBar progressBar;

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
            textBox1 = new TextBox();
            progressBar1 = new ProgressBar();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 338);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(462, 27);
            textBox1.TabIndex = 1;
            textBox1.Text = "Идет установка программы, пожалуйста не закрывайте это окно";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 387);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(643, 29);
            progressBar1.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.ImageLocation = "D:\\Системное программирование\\project 1\\project 1\\images.jpg";
            pictureBox1.Location = new Point(500, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(155, 156);
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // Form4
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(667, 553);
            Controls.Add(pictureBox1);
            Controls.Add(progressBar1);
            Controls.Add(textBox1);
            Name = "Form4";
            Text = "Install";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox textBox1;
        private ProgressBar progressBar1;
        private PictureBox pictureBox1;
    }
}