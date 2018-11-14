namespace CodeCreate
{
    partial class CodeForm
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
            this.lblServerName = new System.Windows.Forms.Label();
            this.ddlServerName = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.gbLogin = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.rbSQLServer = new System.Windows.Forms.RadioButton();
            this.rbWindows = new System.Windows.Forms.RadioButton();
            this.gbDataBase = new System.Windows.Forms.GroupBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.ddlDataBaseFile = new System.Windows.Forms.ComboBox();
            this.rbDataBaseFile = new System.Windows.Forms.RadioButton();
            this.ddlDataBaseName = new System.Windows.Forms.ComboBox();
            this.rbDataBaseName = new System.Windows.Forms.RadioButton();
            this.btnConnection = new System.Windows.Forms.Button();
            this.rtxtCode = new System.Windows.Forms.RichTextBox();
            this.ddlTableNames = new System.Windows.Forms.ComboBox();
            this.lblTableName = new System.Windows.Forms.Label();
            this.btnCreate = new System.Windows.Forms.Button();
            this.gbLogin.SuspendLayout();
            this.gbDataBase.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblServerName
            // 
            this.lblServerName.Location = new System.Drawing.Point(24, 40);
            this.lblServerName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblServerName.Name = "lblServerName";
            this.lblServerName.Size = new System.Drawing.Size(108, 42);
            this.lblServerName.TabIndex = 0;
            this.lblServerName.Text = "服务器名";
            this.lblServerName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ddlServerName
            // 
            this.ddlServerName.FormattingEnabled = true;
            this.ddlServerName.Items.AddRange(new object[] {
            "."});
            this.ddlServerName.Location = new System.Drawing.Point(28, 88);
            this.ddlServerName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ddlServerName.Name = "ddlServerName";
            this.ddlServerName.Size = new System.Drawing.Size(328, 32);
            this.ddlServerName.TabIndex = 2;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(372, 80);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(150, 56);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // gbLogin
            // 
            this.gbLogin.Controls.Add(this.txtPassword);
            this.gbLogin.Controls.Add(this.txtUserName);
            this.gbLogin.Controls.Add(this.lblPassword);
            this.gbLogin.Controls.Add(this.lblUserName);
            this.gbLogin.Controls.Add(this.rbSQLServer);
            this.gbLogin.Controls.Add(this.rbWindows);
            this.gbLogin.Location = new System.Drawing.Point(28, 160);
            this.gbLogin.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbLogin.Name = "gbLogin";
            this.gbLogin.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbLogin.Size = new System.Drawing.Size(494, 258);
            this.gbLogin.TabIndex = 4;
            this.gbLogin.TabStop = false;
            this.gbLogin.Text = "登录到服务器";
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(140, 192);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(312, 35);
            this.txtPassword.TabIndex = 5;
            // 
            // txtUserName
            // 
            this.txtUserName.Enabled = false;
            this.txtUserName.Location = new System.Drawing.Point(140, 134);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(312, 35);
            this.txtUserName.TabIndex = 4;
            // 
            // lblPassword
            // 
            this.lblPassword.Location = new System.Drawing.Point(44, 198);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(82, 36);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "密  码";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUserName
            // 
            this.lblUserName.Location = new System.Drawing.Point(44, 140);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(82, 36);
            this.lblUserName.TabIndex = 2;
            this.lblUserName.Text = "用户名";
            this.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbSQLServer
            // 
            this.rbSQLServer.AutoSize = true;
            this.rbSQLServer.Location = new System.Drawing.Point(12, 84);
            this.rbSQLServer.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbSQLServer.Name = "rbSQLServer";
            this.rbSQLServer.Size = new System.Drawing.Size(329, 28);
            this.rbSQLServer.TabIndex = 1;
            this.rbSQLServer.Text = "使用 SQL Server 身份验证";
            this.rbSQLServer.UseVisualStyleBackColor = true;
            this.rbSQLServer.CheckedChanged += new System.EventHandler(this.rbSQLServer_CheckedChanged);
            // 
            // rbWindows
            // 
            this.rbWindows.AutoSize = true;
            this.rbWindows.Checked = true;
            this.rbWindows.Location = new System.Drawing.Point(12, 40);
            this.rbWindows.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbWindows.Name = "rbWindows";
            this.rbWindows.Size = new System.Drawing.Size(293, 28);
            this.rbWindows.TabIndex = 0;
            this.rbWindows.TabStop = true;
            this.rbWindows.Text = "使用 Windows 身份验证";
            this.rbWindows.UseVisualStyleBackColor = true;
            // 
            // gbDataBase
            // 
            this.gbDataBase.Controls.Add(this.txtName);
            this.gbDataBase.Controls.Add(this.lblName);
            this.gbDataBase.Controls.Add(this.ddlDataBaseFile);
            this.gbDataBase.Controls.Add(this.rbDataBaseFile);
            this.gbDataBase.Controls.Add(this.ddlDataBaseName);
            this.gbDataBase.Controls.Add(this.rbDataBaseName);
            this.gbDataBase.Location = new System.Drawing.Point(28, 454);
            this.gbDataBase.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbDataBase.Name = "gbDataBase";
            this.gbDataBase.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbDataBase.Size = new System.Drawing.Size(494, 356);
            this.gbDataBase.TabIndex = 5;
            this.gbDataBase.TabStop = false;
            this.gbDataBase.Text = "链接一个数据库";
            // 
            // txtName
            // 
            this.txtName.Enabled = false;
            this.txtName.Location = new System.Drawing.Point(48, 290);
            this.txtName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(404, 35);
            this.txtName.TabIndex = 6;
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(44, 238);
            this.lblName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(82, 46);
            this.lblName.TabIndex = 9;
            this.lblName.Text = "逻辑名";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ddlDataBaseFile
            // 
            this.ddlDataBaseFile.Enabled = false;
            this.ddlDataBaseFile.FormattingEnabled = true;
            this.ddlDataBaseFile.Location = new System.Drawing.Point(48, 180);
            this.ddlDataBaseFile.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ddlDataBaseFile.Name = "ddlDataBaseFile";
            this.ddlDataBaseFile.Size = new System.Drawing.Size(404, 32);
            this.ddlDataBaseFile.TabIndex = 8;
            // 
            // rbDataBaseFile
            // 
            this.rbDataBaseFile.AutoSize = true;
            this.rbDataBaseFile.Location = new System.Drawing.Point(12, 136);
            this.rbDataBaseFile.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbDataBaseFile.Name = "rbDataBaseFile";
            this.rbDataBaseFile.Size = new System.Drawing.Size(257, 28);
            this.rbDataBaseFile.TabIndex = 7;
            this.rbDataBaseFile.Text = "附加一个数据库文档";
            this.rbDataBaseFile.UseVisualStyleBackColor = true;
            // 
            // ddlDataBaseName
            // 
            this.ddlDataBaseName.FormattingEnabled = true;
            this.ddlDataBaseName.Location = new System.Drawing.Point(48, 84);
            this.ddlDataBaseName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ddlDataBaseName.Name = "ddlDataBaseName";
            this.ddlDataBaseName.Size = new System.Drawing.Size(404, 32);
            this.ddlDataBaseName.TabIndex = 6;
            this.ddlDataBaseName.SelectedIndexChanged += new System.EventHandler(this.ddlDataBaseName_SelectedIndexChanged);
            // 
            // rbDataBaseName
            // 
            this.rbDataBaseName.AutoSize = true;
            this.rbDataBaseName.Checked = true;
            this.rbDataBaseName.Location = new System.Drawing.Point(12, 40);
            this.rbDataBaseName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbDataBaseName.Name = "rbDataBaseName";
            this.rbDataBaseName.Size = new System.Drawing.Size(305, 28);
            this.rbDataBaseName.TabIndex = 1;
            this.rbDataBaseName.TabStop = true;
            this.rbDataBaseName.Text = "选择或输入一个数据库名";
            this.rbDataBaseName.UseVisualStyleBackColor = true;
            // 
            // btnConnection
            // 
            this.btnConnection.Location = new System.Drawing.Point(28, 822);
            this.btnConnection.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnConnection.Name = "btnConnection";
            this.btnConnection.Size = new System.Drawing.Size(150, 58);
            this.btnConnection.TabIndex = 6;
            this.btnConnection.Text = "测试连接";
            this.btnConnection.UseVisualStyleBackColor = true;
            this.btnConnection.Click += new System.EventHandler(this.btnConnection_Click);
            // 
            // rtxtCode
            // 
            this.rtxtCode.Location = new System.Drawing.Point(568, 160);
            this.rtxtCode.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rtxtCode.Name = "rtxtCode";
            this.rtxtCode.Size = new System.Drawing.Size(970, 646);
            this.rtxtCode.TabIndex = 7;
            this.rtxtCode.Text = "";
            // 
            // ddlTableNames
            // 
            this.ddlTableNames.FormattingEnabled = true;
            this.ddlTableNames.Location = new System.Drawing.Point(680, 88);
            this.ddlTableNames.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ddlTableNames.Name = "ddlTableNames";
            this.ddlTableNames.Size = new System.Drawing.Size(328, 32);
            this.ddlTableNames.TabIndex = 8;
            // 
            // lblTableName
            // 
            this.lblTableName.Location = new System.Drawing.Point(564, 88);
            this.lblTableName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblTableName.Name = "lblTableName";
            this.lblTableName.Size = new System.Drawing.Size(82, 36);
            this.lblTableName.TabIndex = 6;
            this.lblTableName.Text = "表  名";
            this.lblTableName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(1116, 78);
            this.btnCreate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(150, 56);
            this.btnCreate.TabIndex = 9;
            this.btnCreate.Text = "生成";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // CodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1566, 904);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.lblTableName);
            this.Controls.Add(this.ddlTableNames);
            this.Controls.Add(this.rtxtCode);
            this.Controls.Add(this.btnConnection);
            this.Controls.Add(this.gbDataBase);
            this.Controls.Add(this.gbLogin);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.ddlServerName);
            this.Controls.Add(this.lblServerName);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "CodeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "情缘-代码生成器";
            this.Load += new System.EventHandler(this.CodeForm_Load);
            this.gbLogin.ResumeLayout(false);
            this.gbLogin.PerformLayout();
            this.gbDataBase.ResumeLayout(false);
            this.gbDataBase.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.ComboBox ddlServerName;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox gbLogin;
        private System.Windows.Forms.RadioButton rbWindows;
        private System.Windows.Forms.RadioButton rbSQLServer;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.GroupBox gbDataBase;
        private System.Windows.Forms.RadioButton rbDataBaseFile;
        private System.Windows.Forms.ComboBox ddlDataBaseName;
        private System.Windows.Forms.RadioButton rbDataBaseName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ComboBox ddlDataBaseFile;
        private System.Windows.Forms.Button btnConnection;
        private System.Windows.Forms.RichTextBox rtxtCode;
        private System.Windows.Forms.ComboBox ddlTableNames;
        private System.Windows.Forms.Label lblTableName;
        private System.Windows.Forms.Button btnCreate;
    }
}