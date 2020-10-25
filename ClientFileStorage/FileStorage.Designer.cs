namespace ClientFileStorage
{
    partial class FileStorage
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.IDUser = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReleaseDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView1 = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.менюToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.получитьИлиОбновитьСписокФайловToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.управлениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.скачатьВыбранныйФаилToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьВыбранныйФайлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 620);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(1185, 20);
            this.textBox1.TabIndex = 1;
            // 
            // IDUser
            // 
            this.IDUser.Text = "IDUser";
            this.IDUser.Width = 429;
            // 
            // Name
            // 
            this.Name.Text = "FileStorage";
            this.Name.Width = 102;
            // 
            // ReleaseDate
            // 
            this.ReleaseDate.Text = "ReleaseDate";
            this.ReleaseDate.Width = 135;
            // 
            // FileName
            // 
            this.FileName.Text = "FileName";
            this.FileName.Width = 512;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IDUser,
            this.Name,
            this.ReleaseDate,
            this.FileName});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 24);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1185, 596);
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.менюToolStripMenuItem,
            this.управлениеToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1185, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // менюToolStripMenuItem
            // 
            this.менюToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.получитьИлиОбновитьСписокФайловToolStripMenuItem,
            this.выходToolStripMenuItem});
            this.менюToolStripMenuItem.Name = "менюToolStripMenuItem";
            this.менюToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.менюToolStripMenuItem.Text = "Меню";
            // 
            // получитьИлиОбновитьСписокФайловToolStripMenuItem
            // 
            this.получитьИлиОбновитьСписокФайловToolStripMenuItem.Name = "получитьИлиОбновитьСписокФайловToolStripMenuItem";
            this.получитьИлиОбновитьСписокФайловToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.получитьИлиОбновитьСписокФайловToolStripMenuItem.Text = "Получить или обновить список файлов";
            this.получитьИлиОбновитьСписокФайловToolStripMenuItem.Click += new System.EventHandler(this.получитьИлиОбновитьСписокФайловToolStripMenuItem_Click);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // управлениеToolStripMenuItem
            // 
            this.управлениеToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.скачатьВыбранныйФаилToolStripMenuItem,
            this.удалитьВыбранныйФайлToolStripMenuItem});
            this.управлениеToolStripMenuItem.Name = "управлениеToolStripMenuItem";
            this.управлениеToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
            this.управлениеToolStripMenuItem.Text = "Управление";
            // 
            // скачатьВыбранныйФаилToolStripMenuItem
            // 
            this.скачатьВыбранныйФаилToolStripMenuItem.Name = "скачатьВыбранныйФаилToolStripMenuItem";
            this.скачатьВыбранныйФаилToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.скачатьВыбранныйФаилToolStripMenuItem.Text = "Скачать выбранный фаил";
            this.скачатьВыбранныйФаилToolStripMenuItem.Click += new System.EventHandler(this.скачатьВыбранныйФаилToolStripMenuItem_Click);
            // 
            // удалитьВыбранныйФайлToolStripMenuItem
            // 
            this.удалитьВыбранныйФайлToolStripMenuItem.Name = "удалитьВыбранныйФайлToolStripMenuItem";
            this.удалитьВыбранныйФайлToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.удалитьВыбранныйФайлToolStripMenuItem.Text = "Удалить выбранный файл";
            // 
            // FileStorage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1185, 640);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name.Text = "FileStorage";
            this.Text = "FileStorage";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ColumnHeader IDUser;
        private System.Windows.Forms.ColumnHeader Name;
        private System.Windows.Forms.ColumnHeader ReleaseDate;
        private System.Windows.Forms.ColumnHeader FileName;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem менюToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem получитьИлиОбновитьСписокФайловToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem управлениеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem скачатьВыбранныйФаилToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem удалитьВыбранныйФайлToolStripMenuItem;
    }
}