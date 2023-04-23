namespace DarkSoulsModManager
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.viewModEngine = new System.Windows.Forms.Button();
            this.openModDirWizard = new System.Windows.Forms.Button();
            this.toolsDD = new System.Windows.Forms.ComboBox();
            this.selectPath = new System.Windows.Forms.Button();
            this.launchDS3 = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.notificationLabel = new System.Windows.Forms.Label();
            this.blockNetworkAccess = new System.Windows.Forms.Button();
            this.useAlternateSaveFile = new System.Windows.Forms.Button();
            this.loadLooseParams = new System.Windows.Forms.Button();
            this.loadUXMFiles = new System.Windows.Forms.Button();
            this.useModOverrideDirectory = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Garamond", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Garamond", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Garamond", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dataGridView1.Location = new System.Drawing.Point(178, 7);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Garamond", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.RowHeadersWidth = 62;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView1.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Black;
            this.dataGridView1.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(475, 423);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // viewModEngine
            // 
            this.viewModEngine.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.viewModEngine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.viewModEngine.Location = new System.Drawing.Point(909, 437);
            this.viewModEngine.Name = "viewModEngine";
            this.viewModEngine.Size = new System.Drawing.Size(138, 23);
            this.viewModEngine.TabIndex = 1;
            this.viewModEngine.Text = "View Mod Engine";
            this.viewModEngine.UseVisualStyleBackColor = true;
            this.viewModEngine.Click += new System.EventHandler(this.viewModEngine_Click);
            // 
            // pickModdingDirectory
            // 
            this.openModDirWizard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.openModDirWizard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openModDirWizard.Location = new System.Drawing.Point(12, 437);
            this.openModDirWizard.Name = "pickModdingDirectory";
            this.openModDirWizard.Size = new System.Drawing.Size(146, 23);
            this.openModDirWizard.TabIndex = 2;
            this.openModDirWizard.Text = "Modding Directory";
            this.openModDirWizard.UseVisualStyleBackColor = true;
            this.openModDirWizard.Click += new System.EventHandler(this.pickModdingDirectory_Click);
            // 
            // toolsDD
            // 
            this.toolsDD.BackColor = System.Drawing.Color.Black;
            this.toolsDD.ForeColor = System.Drawing.Color.White;
            this.toolsDD.FormattingEnabled = true;
            this.toolsDD.Location = new System.Drawing.Point(207, 436);
            this.toolsDD.Name = "toolsDD";
            this.toolsDD.Size = new System.Drawing.Size(331, 26);
            this.toolsDD.TabIndex = 3;
            this.toolsDD.Text = "Pick a modding tool here to launch it";
            this.toolsDD.SelectedIndexChanged += new System.EventHandler(this.toolsDD_SelectedIndexChanged);
            // 
            // selectPath
            // 
            this.selectPath.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.selectPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectPath.Location = new System.Drawing.Point(544, 437);
            this.selectPath.Name = "selectPath";
            this.selectPath.Size = new System.Drawing.Size(108, 23);
            this.selectPath.TabIndex = 4;
            this.selectPath.Text = "Steam\'s Drive";
            this.selectPath.UseVisualStyleBackColor = true;
            this.selectPath.Click += new System.EventHandler(this.selectPath_Click);
            // 
            // launchDS3
            // 
            this.launchDS3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.launchDS3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.launchDS3.Location = new System.Drawing.Point(1053, 437);
            this.launchDS3.Name = "launchDS3";
            this.launchDS3.Size = new System.Drawing.Size(121, 23);
            this.launchDS3.TabIndex = 5;
            this.launchDS3.Text = "Launch Game";
            this.launchDS3.UseVisualStyleBackColor = true;
            this.launchDS3.Click += new System.EventHandler(this.launchDS3_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(659, 22);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(515, 397);
            this.webBrowser1.TabIndex = 6;
            // 
            // notificationLabel
            // 
            this.notificationLabel.AutoSize = true;
            this.notificationLabel.Location = new System.Drawing.Point(9, 463);
            this.notificationLabel.Name = "notificationLabel";
            this.notificationLabel.Size = new System.Drawing.Size(124, 18);
            this.notificationLabel.TabIndex = 7;
            this.notificationLabel.Text = "Notification Label";
            // 
            // blockNetworkAccess
            // 
            this.blockNetworkAccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.blockNetworkAccess.Location = new System.Drawing.Point(13, 13);
            this.blockNetworkAccess.Name = "blockNetworkAccess";
            this.blockNetworkAccess.Size = new System.Drawing.Size(159, 23);
            this.blockNetworkAccess.TabIndex = 8;
            this.blockNetworkAccess.Text = "blockNetworkAccess";
            this.blockNetworkAccess.UseVisualStyleBackColor = true;
            this.blockNetworkAccess.Click += new System.EventHandler(this.blockNetworkAccess_Click);
            // 
            // useAlternateSaveFile
            // 
            this.useAlternateSaveFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.useAlternateSaveFile.Location = new System.Drawing.Point(12, 42);
            this.useAlternateSaveFile.Name = "useAlternateSaveFile";
            this.useAlternateSaveFile.Size = new System.Drawing.Size(160, 23);
            this.useAlternateSaveFile.TabIndex = 9;
            this.useAlternateSaveFile.Text = "useAlternateSaveFile";
            this.useAlternateSaveFile.UseVisualStyleBackColor = true;
            this.useAlternateSaveFile.Click += new System.EventHandler(this.useAlternateSaveFile_Click);
            // 
            // loadLooseParams
            // 
            this.loadLooseParams.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadLooseParams.Location = new System.Drawing.Point(12, 71);
            this.loadLooseParams.Name = "loadLooseParams";
            this.loadLooseParams.Size = new System.Drawing.Size(160, 23);
            this.loadLooseParams.TabIndex = 10;
            this.loadLooseParams.Text = "loadLooseParams";
            this.loadLooseParams.UseVisualStyleBackColor = true;
            this.loadLooseParams.Click += new System.EventHandler(this.loadLooseParams_Click);
            // 
            // loadUXMFiles
            // 
            this.loadUXMFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadUXMFiles.Location = new System.Drawing.Point(12, 100);
            this.loadUXMFiles.Name = "loadUXMFiles";
            this.loadUXMFiles.Size = new System.Drawing.Size(160, 23);
            this.loadUXMFiles.TabIndex = 11;
            this.loadUXMFiles.Text = "loadUXMFiles";
            this.loadUXMFiles.UseVisualStyleBackColor = true;
            this.loadUXMFiles.Click += new System.EventHandler(this.loadUXMFiles_Click);
            // 
            // useModOverrideDirectory
            // 
            this.useModOverrideDirectory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.useModOverrideDirectory.Location = new System.Drawing.Point(12, 129);
            this.useModOverrideDirectory.Name = "useModOverrideDirectory";
            this.useModOverrideDirectory.Size = new System.Drawing.Size(160, 23);
            this.useModOverrideDirectory.TabIndex = 12;
            this.useModOverrideDirectory.Text = "useModOverrideDirectory";
            this.useModOverrideDirectory.UseVisualStyleBackColor = true;
            this.useModOverrideDirectory.Click += new System.EventHandler(this.useModOverrideDirectory_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1186, 519);
            this.Controls.Add(this.useModOverrideDirectory);
            this.Controls.Add(this.loadUXMFiles);
            this.Controls.Add(this.loadLooseParams);
            this.Controls.Add(this.useAlternateSaveFile);
            this.Controls.Add(this.blockNetworkAccess);
            this.Controls.Add(this.notificationLabel);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.launchDS3);
            this.Controls.Add(this.selectPath);
            this.Controls.Add(this.toolsDD);
            this.Controls.Add(this.openModDirWizard);
            this.Controls.Add(this.viewModEngine);
            this.Controls.Add(this.dataGridView1);
            this.Font = new System.Drawing.Font("Garamond", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Form1";
            this.Text = "Dark Souls Mod Manager";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button viewModEngine;
        private System.Windows.Forms.Button openModDirWizard;
        private System.Windows.Forms.ComboBox toolsDD;
        private System.Windows.Forms.Button selectPath;
        private System.Windows.Forms.Button launchDS3;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Label notificationLabel;
        private System.Windows.Forms.Button blockNetworkAccess;
        private System.Windows.Forms.Button useAlternateSaveFile;
        private System.Windows.Forms.Button loadLooseParams;
        private System.Windows.Forms.Button loadUXMFiles;
        private System.Windows.Forms.Button useModOverrideDirectory;
    }
}