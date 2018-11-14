namespace Apps.CodeHelper
{
    partial class CodeFrom
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lb_Tables = new System.Windows.Forms.ListBox();
            this.tab_CodeList = new System.Windows.Forms.TabControl();
            this.tp_Controller = new System.Windows.Forms.TabPage();
            this.txt_Controller = new System.Windows.Forms.TextBox();
            this.tp_Index = new System.Windows.Forms.TabPage();
            this.txt_Index = new System.Windows.Forms.TextBox();
            this.tp_Create = new System.Windows.Forms.TabPage();
            this.txt_Create = new System.Windows.Forms.TextBox();
            this.tp_Edit = new System.Windows.Forms.TabPage();
            this.txt_Edit = new System.Windows.Forms.TextBox();
            this.tb_Create2 = new System.Windows.Forms.TabPage();
            this.txt_CreateParent = new System.Windows.Forms.TextBox();
            this.tb_Edit2 = new System.Windows.Forms.TabPage();
            this.txt_EditParent = new System.Windows.Forms.TextBox();
            this.tp_BLL = new System.Windows.Forms.TabPage();
            this.txt_PartialBLL = new System.Windows.Forms.TextBox();
            this.tp_Model = new System.Windows.Forms.TabPage();
            this.txt_PartialModel = new System.Windows.Forms.TextBox();
            this.txt_SQL = new System.Windows.Forms.TextBox();
            this.btn_EditSQLCon = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtfilepath = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_prefix = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cb_EnableParent = new System.Windows.Forms.CheckBox();
            this.txt_TableName1 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cb_MulView = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.txt_TableKey1 = new System.Windows.Forms.TextBox();
            this.cb_tree = new System.Windows.Forms.CheckBox();
            this.tab_CodeList.SuspendLayout();
            this.tp_Controller.SuspendLayout();
            this.tp_Index.SuspendLayout();
            this.tp_Create.SuspendLayout();
            this.tp_Edit.SuspendLayout();
            this.tb_Create2.SuspendLayout();
            this.tb_Edit2.SuspendLayout();
            this.tp_BLL.SuspendLayout();
            this.tp_Model.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_Tables
            // 
            this.lb_Tables.FormattingEnabled = true;
            this.lb_Tables.ItemHeight = 12;
            this.lb_Tables.Location = new System.Drawing.Point(12, 46);
            this.lb_Tables.Name = "lb_Tables";
            this.lb_Tables.Size = new System.Drawing.Size(259, 436);
            this.lb_Tables.TabIndex = 0;
            this.lb_Tables.SelectedIndexChanged += new System.EventHandler(this.lb_Tables_SelectedIndexChanged);
            // 
            // tab_CodeList
            // 
            this.tab_CodeList.Controls.Add(this.tp_Controller);
            this.tab_CodeList.Controls.Add(this.tp_Index);
            this.tab_CodeList.Controls.Add(this.tp_Create);
            this.tab_CodeList.Controls.Add(this.tp_Edit);
            this.tab_CodeList.Controls.Add(this.tb_Create2);
            this.tab_CodeList.Controls.Add(this.tb_Edit2);
            this.tab_CodeList.Controls.Add(this.tp_BLL);
            this.tab_CodeList.Controls.Add(this.tp_Model);
            this.tab_CodeList.Location = new System.Drawing.Point(277, 12);
            this.tab_CodeList.Name = "tab_CodeList";
            this.tab_CodeList.SelectedIndex = 0;
            this.tab_CodeList.Size = new System.Drawing.Size(833, 471);
            this.tab_CodeList.TabIndex = 1;
            // 
            // tp_Controller
            // 
            this.tp_Controller.Controls.Add(this.txt_Controller);
            this.tp_Controller.Location = new System.Drawing.Point(4, 22);
            this.tp_Controller.Name = "tp_Controller";
            this.tp_Controller.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Controller.Size = new System.Drawing.Size(825, 445);
            this.tp_Controller.TabIndex = 5;
            this.tp_Controller.Text = "Controller";
            this.tp_Controller.UseVisualStyleBackColor = true;
            // 
            // txt_Controller
            // 
            this.txt_Controller.Cursor = System.Windows.Forms.Cursors.Default;
            this.txt_Controller.Location = new System.Drawing.Point(0, 1);
            this.txt_Controller.Multiline = true;
            this.txt_Controller.Name = "txt_Controller";
            this.txt_Controller.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Controller.Size = new System.Drawing.Size(828, 443);
            this.txt_Controller.TabIndex = 4;
            this.txt_Controller.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.anyTextBox_KeyPress);
            // 
            // tp_Index
            // 
            this.tp_Index.Controls.Add(this.txt_Index);
            this.tp_Index.Location = new System.Drawing.Point(4, 22);
            this.tp_Index.Name = "tp_Index";
            this.tp_Index.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Index.Size = new System.Drawing.Size(825, 445);
            this.tp_Index.TabIndex = 6;
            this.tp_Index.Text = "Index";
            this.tp_Index.UseVisualStyleBackColor = true;
            // 
            // txt_Index
            // 
            this.txt_Index.Location = new System.Drawing.Point(0, 1);
            this.txt_Index.Multiline = true;
            this.txt_Index.Name = "txt_Index";
            this.txt_Index.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Index.Size = new System.Drawing.Size(828, 443);
            this.txt_Index.TabIndex = 4;
            this.txt_Index.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.anyTextBox_KeyPress);
            // 
            // tp_Create
            // 
            this.tp_Create.Controls.Add(this.txt_Create);
            this.tp_Create.Location = new System.Drawing.Point(4, 22);
            this.tp_Create.Name = "tp_Create";
            this.tp_Create.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Create.Size = new System.Drawing.Size(825, 445);
            this.tp_Create.TabIndex = 7;
            this.tp_Create.Text = "Create";
            this.tp_Create.UseVisualStyleBackColor = true;
            // 
            // txt_Create
            // 
            this.txt_Create.Location = new System.Drawing.Point(0, 1);
            this.txt_Create.Multiline = true;
            this.txt_Create.Name = "txt_Create";
            this.txt_Create.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Create.Size = new System.Drawing.Size(828, 443);
            this.txt_Create.TabIndex = 3;
            this.txt_Create.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.anyTextBox_KeyPress);
            // 
            // tp_Edit
            // 
            this.tp_Edit.Controls.Add(this.txt_Edit);
            this.tp_Edit.Location = new System.Drawing.Point(4, 22);
            this.tp_Edit.Name = "tp_Edit";
            this.tp_Edit.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Edit.Size = new System.Drawing.Size(825, 445);
            this.tp_Edit.TabIndex = 8;
            this.tp_Edit.Text = "Edit";
            this.tp_Edit.UseVisualStyleBackColor = true;
            // 
            // txt_Edit
            // 
            this.txt_Edit.Location = new System.Drawing.Point(0, 1);
            this.txt_Edit.Multiline = true;
            this.txt_Edit.Name = "txt_Edit";
            this.txt_Edit.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Edit.Size = new System.Drawing.Size(824, 443);
            this.txt_Edit.TabIndex = 4;
            this.txt_Edit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.anyTextBox_KeyPress);
            // 
            // tb_Create2
            // 
            this.tb_Create2.Controls.Add(this.txt_CreateParent);
            this.tb_Create2.Location = new System.Drawing.Point(4, 22);
            this.tb_Create2.Name = "tb_Create2";
            this.tb_Create2.Size = new System.Drawing.Size(825, 445);
            this.tb_Create2.TabIndex = 12;
            this.tb_Create2.Text = "Create父表";
            this.tb_Create2.UseVisualStyleBackColor = true;
            // 
            // txt_CreateParent
            // 
            this.txt_CreateParent.Location = new System.Drawing.Point(-2, 1);
            this.txt_CreateParent.Multiline = true;
            this.txt_CreateParent.Name = "txt_CreateParent";
            this.txt_CreateParent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_CreateParent.Size = new System.Drawing.Size(828, 443);
            this.txt_CreateParent.TabIndex = 4;
            this.txt_CreateParent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.anyTextBox_KeyPress);
            // 
            // tb_Edit2
            // 
            this.tb_Edit2.Controls.Add(this.txt_EditParent);
            this.tb_Edit2.Location = new System.Drawing.Point(4, 22);
            this.tb_Edit2.Name = "tb_Edit2";
            this.tb_Edit2.Size = new System.Drawing.Size(825, 445);
            this.tb_Edit2.TabIndex = 13;
            this.tb_Edit2.Text = "Edit父表";
            this.tb_Edit2.UseVisualStyleBackColor = true;
            // 
            // txt_EditParent
            // 
            this.txt_EditParent.Location = new System.Drawing.Point(-2, 1);
            this.txt_EditParent.Multiline = true;
            this.txt_EditParent.Name = "txt_EditParent";
            this.txt_EditParent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_EditParent.Size = new System.Drawing.Size(828, 443);
            this.txt_EditParent.TabIndex = 5;
            this.txt_EditParent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.anyTextBox_KeyPress);
            // 
            // tp_BLL
            // 
            this.tp_BLL.BackColor = System.Drawing.Color.Transparent;
            this.tp_BLL.Controls.Add(this.txt_PartialBLL);
            this.tp_BLL.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.tp_BLL.Location = new System.Drawing.Point(4, 22);
            this.tp_BLL.Name = "tp_BLL";
            this.tp_BLL.Size = new System.Drawing.Size(825, 445);
            this.tp_BLL.TabIndex = 9;
            this.tp_BLL.Text = "BLL分部类";
            this.tp_BLL.UseVisualStyleBackColor = true;
            // 
            // txt_PartialBLL
            // 
            this.txt_PartialBLL.Location = new System.Drawing.Point(0, 1);
            this.txt_PartialBLL.Multiline = true;
            this.txt_PartialBLL.Name = "txt_PartialBLL";
            this.txt_PartialBLL.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_PartialBLL.Size = new System.Drawing.Size(824, 443);
            this.txt_PartialBLL.TabIndex = 5;
            this.txt_PartialBLL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.anyTextBox_KeyPress);
            // 
            // tp_Model
            // 
            this.tp_Model.Controls.Add(this.txt_PartialModel);
            this.tp_Model.Location = new System.Drawing.Point(4, 22);
            this.tp_Model.Name = "tp_Model";
            this.tp_Model.Size = new System.Drawing.Size(825, 445);
            this.tp_Model.TabIndex = 11;
            this.tp_Model.Text = "Model分部类";
            this.tp_Model.UseVisualStyleBackColor = true;
            // 
            // txt_PartialModel
            // 
            this.txt_PartialModel.Location = new System.Drawing.Point(0, 1);
            this.txt_PartialModel.Multiline = true;
            this.txt_PartialModel.Name = "txt_PartialModel";
            this.txt_PartialModel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_PartialModel.Size = new System.Drawing.Size(824, 443);
            this.txt_PartialModel.TabIndex = 6;
            this.txt_PartialModel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.anyTextBox_KeyPress);
            // 
            // txt_SQL
            // 
            this.txt_SQL.Location = new System.Drawing.Point(121, 536);
            this.txt_SQL.Name = "txt_SQL";
            this.txt_SQL.Size = new System.Drawing.Size(384, 21);
            this.txt_SQL.TabIndex = 4;
            // 
            // btn_EditSQLCon
            // 
            this.btn_EditSQLCon.Location = new System.Drawing.Point(511, 535);
            this.btn_EditSQLCon.Name = "btn_EditSQLCon";
            this.btn_EditSQLCon.Size = new System.Drawing.Size(75, 23);
            this.btn_EditSQLCon.TabIndex = 6;
            this.btn_EditSQLCon.Text = "确定";
            this.btn_EditSQLCon.UseVisualStyleBackColor = true;
            this.btn_EditSQLCon.Click += new System.EventHandler(this.btn_EditSQLCon_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 540);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "SQL Connection：";
            // 
            // txtfilepath
            // 
            this.txtfilepath.Enabled = false;
            this.txtfilepath.Location = new System.Drawing.Point(707, 536);
            this.txtfilepath.Name = "txtfilepath";
            this.txtfilepath.Size = new System.Drawing.Size(196, 21);
            this.txtfilepath.TabIndex = 15;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(960, 535);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(131, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "导出生成";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(909, 535);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(45, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "选择";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(612, 540);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "设置导出路径：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 601);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(881, 12);
            this.label5.TabIndex = 19;
            this.label5.Text = "第一次使用：需要修改数据库（SQL）连接串，如果运行出现错误提示Xml不存在，手动复制Apps.CodeHelper目录下的Config.Xml文件到Bin的De" +
    "bug和Release文件夹下";
            // 
            // txt_prefix
            // 
            this.txt_prefix.Location = new System.Drawing.Point(78, 17);
            this.txt_prefix.Name = "txt_prefix";
            this.txt_prefix.Size = new System.Drawing.Size(100, 21);
            this.txt_prefix.TabIndex = 8;
            this.txt_prefix.Text = "Apps";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "命名空间：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 622);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(761, 12);
            this.label3.TabIndex = 20;
            this.label3.Text = "关于表关联：表与表之间在数据库必须有关系，有表关联会生成BLL父表,Model父表 复制到解决方案对应的目录即可(需要启用并按确定来生成)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(129, 500);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 21;
            this.label6.Text = "表名：";
            // 
            // cb_EnableParent
            // 
            this.cb_EnableParent.AutoSize = true;
            this.cb_EnableParent.Location = new System.Drawing.Point(21, 499);
            this.cb_EnableParent.Name = "cb_EnableParent";
            this.cb_EnableParent.Size = new System.Drawing.Size(96, 16);
            this.cb_EnableParent.TabIndex = 22;
            this.cb_EnableParent.Text = "启用父表关联";
            this.cb_EnableParent.UseVisualStyleBackColor = true;
            // 
            // txt_TableName1
            // 
            this.txt_TableName1.Location = new System.Drawing.Point(160, 9);
            this.txt_TableName1.Name = "txt_TableName1";
            this.txt_TableName1.Size = new System.Drawing.Size(118, 21);
            this.txt_TableName1.TabIndex = 23;
            this.txt_TableName1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_TableName1_KeyDown);
            this.txt_TableName1.Leave += new System.EventHandler(this.txt_TableName1_Leave);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.cb_tree);
            this.panel1.Controls.Add(this.cb_MulView);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.txt_TableName1);
            this.panel1.Controls.Add(this.txt_TableKey1);
            this.panel1.Location = new System.Drawing.Point(12, 485);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1094, 40);
            this.panel1.TabIndex = 34;
            // 
            // cb_MulView
            // 
            this.cb_MulView.AutoSize = true;
            this.cb_MulView.Location = new System.Drawing.Point(453, 11);
            this.cb_MulView.Name = "cb_MulView";
            this.cb_MulView.Size = new System.Drawing.Size(132, 16);
            this.cb_MulView.TabIndex = 36;
            this.cb_MulView.Text = "可在子表中编辑父表";
            this.cb_MulView.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(281, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 35;
            this.label7.Text = "关联外键：";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(946, 7);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(131, 23);
            this.button3.TabIndex = 34;
            this.button3.Text = "确定生成关联";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txt_TableKey1
            // 
            this.txt_TableKey1.Location = new System.Drawing.Point(343, 9);
            this.txt_TableKey1.Name = "txt_TableKey1";
            this.txt_TableKey1.Size = new System.Drawing.Size(102, 21);
            this.txt_TableKey1.TabIndex = 24;
            // 
            // cb_tree
            // 
            this.cb_tree.AutoSize = true;
            this.cb_tree.Location = new System.Drawing.Point(591, 11);
            this.cb_tree.Name = "cb_tree";
            this.cb_tree.Size = new System.Drawing.Size(72, 16);
            this.cb_tree.TabIndex = 37;
            this.cb_tree.Text = "树形结构";
            this.cb_tree.UseVisualStyleBackColor = true;
            // 
            // CodeFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1113, 668);
            this.Controls.Add(this.cb_EnableParent);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtfilepath);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_prefix);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_EditSQLCon);
            this.Controls.Add(this.txt_SQL);
            this.Controls.Add(this.tab_CodeList);
            this.Controls.Add(this.lb_Tables);
            this.Controls.Add(this.panel1);
            this.Name = "CodeFrom";
            this.Text = "MVC模版生成byYmNets";
            this.Load += new System.EventHandler(this.CodeFrom_Load);
            this.tab_CodeList.ResumeLayout(false);
            this.tp_Controller.ResumeLayout(false);
            this.tp_Controller.PerformLayout();
            this.tp_Index.ResumeLayout(false);
            this.tp_Index.PerformLayout();
            this.tp_Create.ResumeLayout(false);
            this.tp_Create.PerformLayout();
            this.tp_Edit.ResumeLayout(false);
            this.tp_Edit.PerformLayout();
            this.tb_Create2.ResumeLayout(false);
            this.tb_Create2.PerformLayout();
            this.tb_Edit2.ResumeLayout(false);
            this.tb_Edit2.PerformLayout();
            this.tp_BLL.ResumeLayout(false);
            this.tp_BLL.PerformLayout();
            this.tp_Model.ResumeLayout(false);
            this.tp_Model.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lb_Tables;
        private System.Windows.Forms.TabControl tab_CodeList;
        private System.Windows.Forms.TabPage tp_Controller;
        private System.Windows.Forms.TabPage tp_Index;
        private System.Windows.Forms.TabPage tp_Create;
        private System.Windows.Forms.TabPage tp_Edit;
        private System.Windows.Forms.TextBox txt_SQL;
        private System.Windows.Forms.Button btn_EditSQLCon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_Create;
        private System.Windows.Forms.TextBox txt_Controller;
        private System.Windows.Forms.TextBox txt_Index;
        private System.Windows.Forms.TextBox txt_Edit;
        private System.Windows.Forms.TextBox txtfilepath;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_prefix;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cb_EnableParent;
        private System.Windows.Forms.TextBox txt_TableName1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txt_TableKey1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tp_BLL;
        private System.Windows.Forms.TabPage tp_Model;
        private System.Windows.Forms.TextBox txt_PartialBLL;
        private System.Windows.Forms.TextBox txt_PartialModel;
        private System.Windows.Forms.CheckBox cb_MulView;
        private System.Windows.Forms.TabPage tb_Create2;
        private System.Windows.Forms.TabPage tb_Edit2;
        private System.Windows.Forms.TextBox txt_CreateParent;
        private System.Windows.Forms.TextBox txt_EditParent;
        private System.Windows.Forms.CheckBox cb_tree;
    }
}

