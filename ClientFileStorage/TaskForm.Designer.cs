namespace ClientFileStorage
{
    partial class TaskForm
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
            this.forced_archivate_button = new System.Windows.Forms.Button();
            this.actionbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // forced_archivate_button
            // 
            this.forced_archivate_button.Location = new System.Drawing.Point(13, 13);
            this.forced_archivate_button.Name = "forced_archivate_button";
            this.forced_archivate_button.Size = new System.Drawing.Size(164, 23);
            this.forced_archivate_button.TabIndex = 0;
            this.forced_archivate_button.Text = "Принудительная архивация";
            this.forced_archivate_button.UseVisualStyleBackColor = true;
            this.forced_archivate_button.Click += new System.EventHandler(this.forced_archivate_button_Click);
            // 
            // actionbutton
            // 
            this.actionbutton.Location = new System.Drawing.Point(183, 13);
            this.actionbutton.Name = "actionbutton";
            this.actionbutton.Size = new System.Drawing.Size(164, 23);
            this.actionbutton.TabIndex = 1;
            this.actionbutton.Text = "2";
            this.actionbutton.UseVisualStyleBackColor = true;
            this.actionbutton.Click += new System.EventHandler(this.actionbutton_Click);
            // 
            // TaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.actionbutton);
            this.Controls.Add(this.forced_archivate_button);
            this.Name = "TaskForm";
            this.Text = "TaskForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button forced_archivate_button;
        private System.Windows.Forms.Button actionbutton;
    }
}