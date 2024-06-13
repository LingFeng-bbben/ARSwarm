namespace TagDetector
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            fpslabel = new Label();
            checkBox1 = new CheckBox();
            numericUpDown1 = new NumericUpDown();
            detlabel = new Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(80, 12);
            button1.Name = "button1";
            button1.Size = new Size(75, 24);
            button1.TabIndex = 0;
            button1.Text = "Start";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // fpslabel
            // 
            fpslabel.AutoSize = true;
            fpslabel.Location = new Point(161, 15);
            fpslabel.Name = "fpslabel";
            fpslabel.Size = new Size(31, 17);
            fpslabel.TabIndex = 2;
            fpslabel.Text = "FPS:";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Location = new Point(12, 41);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(99, 21);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "Show Image";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(12, 13);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(62, 23);
            numericUpDown1.TabIndex = 4;
            numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // detlabel
            // 
            detlabel.AutoSize = true;
            detlabel.Location = new Point(161, 42);
            detlabel.Name = "detlabel";
            detlabel.Size = new Size(31, 17);
            detlabel.TabIndex = 5;
            detlabel.Text = "Det:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(369, 79);
            Controls.Add(detlabel);
            Controls.Add(numericUpDown1);
            Controls.Add(checkBox1);
            Controls.Add(fpslabel);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label fpslabel;
        private CheckBox checkBox1;
        private NumericUpDown numericUpDown1;
        private Label detlabel;
    }
}