namespace Load_Dist__Server_
{
    partial class ServerForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtbProcessing = new System.Windows.Forms.RichTextBox();
            this.btnClearForm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbProcessing
            // 
            this.rtbProcessing.Location = new System.Drawing.Point(3, 2);
            this.rtbProcessing.Name = "rtbProcessing";
            this.rtbProcessing.ReadOnly = true;
            this.rtbProcessing.Size = new System.Drawing.Size(490, 310);
            this.rtbProcessing.TabIndex = 0;
            this.rtbProcessing.Text = "";
            // 
            // btnClearForm
            // 
            this.btnClearForm.Location = new System.Drawing.Point(166, 326);
            this.btnClearForm.Name = "btnClearForm";
            this.btnClearForm.Size = new System.Drawing.Size(168, 47);
            this.btnClearForm.TabIndex = 1;
            this.btnClearForm.Text = "Очистить форму";
            this.btnClearForm.UseVisualStyleBackColor = true;
            this.btnClearForm.Click += new System.EventHandler(this.btnClearForm_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 385);
            this.Controls.Add(this.btnClearForm);
            this.Controls.Add(this.rtbProcessing);
            this.Name = "ServerForm";
            this.Text = "Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbProcessing;
        private System.Windows.Forms.Button btnClearForm;
    }
}

