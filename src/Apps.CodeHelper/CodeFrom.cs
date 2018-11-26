using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Apps.CodeHelper
{
    public partial class CodeFrom : Form
    {


        static string conn = "";
        public CodeFrom()
        {
            InitializeComponent();
        }

        private void CodeFrom_Load(object sender, EventArgs e)
        {
            init();
        }

        //加载数据库和表
        private void init()
        {
            string strXmlPath = "Config.xml";
            txt_SQL.Text = XmlHelper.GetXmlFileValue(strXmlPath, "DataBase");
            txtfilepath.Text = XmlHelper.GetXmlFileValue(strXmlPath, "Path");

            conn = txt_SQL.Text;//"Integrated Security=SSPI;Initial Catalog='AppDB';Data Source='.';User ID='sa';Password='zhaoyun123!@#';Connect Timeout=30";
            Dictionary<string, string> tables = SqlHelper.GetAllTableName(conn);
            lb_Tables.DataSource = new BindingSource(tables, null);
            lb_Tables.DisplayMember = "key";
            lb_Tables.DisplayMember = "value";
        }
        private void lb_Tables_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                OutputCode();
            }
            catch { }
        }
        //导出代码
        private void OutputCode()
        {

            string tableName = lb_Tables.Text;
            List<CompleteField> comFields = SqlHelper.GetColumnCompleteField(conn, tableName);
            CompleteField parentField = null;
            foreach (CompleteField field in comFields)
            {
                //经过约定，存在Id=>对应ParentId即为树形结构
                if (field.name == "ParentId")
                {
                    parentField = field;
                    continue;
                }
            }
            //经过约定，存在Id=>对应ParentId即为树形结构，并且人工启用的树选择
            if (parentField != null && parentField.name == "ParentId" && cb_tree.Enabled)
            {
                txt_Controller.Text = GetTreeController(tableName);
                txt_Index.Text = GetTreeIndex(tableName);
                txt_PartialBLL.Text = GetTreePartialBLL(tableName);
                txt_PartialModel.Text = GetTreePartialModel(tableName);
            }
            else
            {
                txt_Controller.Text = GetController(tableName);
                txt_Index.Text = GetIndex(tableName);
                txt_PartialBLL.Text = GetPartialBLL(tableName);
                txt_PartialModel.Text = GetPartialModel(tableName);
            }

            //获取IBLL
            tbIBLL.Text = GetIBLL(tableName);

            //获取Model
            txt_Create.Text = GetCreate(tableName);
            //获取Model
            txt_Edit.Text = GetEdit(tableName);

            //默认值
            txt_CreateParent.Text = "//只有当启用父表时且开启可在子表中编辑父表时会生成";
            txt_EditParent.Text = "//只有当启用父表时且开启可在子表中编辑父表时会生成";

            //没有启用连表编辑
            if (cb_EnableParent.Checked)
            {

                //启用在字表中编辑父表信息，对应的视图
                if (cb_MulView.Checked)
                {
                    //获取Model
                    txt_CreateParent.Text = GetCreateParent(txt_TableName1.Text);
                    //获取Model
                    txt_EditParent.Text = GetEditParent(txt_TableName1.Text);
                }
            }


        }



        

        

      

        

        

        

        

        




        

        #region TreePartialModel
      
        #endregion


        //获取前缀
        public string GetLeftStr(string tableName)
        {
            //生成代码
            string nameSpace = "";
            if (lb_Tables.Text.IndexOf("_") > 0)
            {
                nameSpace = tableName.Substring(0, tableName.IndexOf("_"));
            }
            else
            {
                nameSpace = "Sys";
            }
            return nameSpace;
        }
        //textbox全选
        private void anyTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == '\x1')
            {
                ((TextBox)sender).SelectAll();
                e.Handled = true;
            }
        }

        //处理注解
        public string SetValid(CompleteField field)//0不能为空
        {
            string validStr = "";
            string xtype = SqlHelper.GetType(field.xType);
            if (xtype == "string")
            {
                //不能为空
                if (field.isNullAble == "0" && field.name.ToLower() != "id")//一般ID为主键
                {
                    validStr = validStr + "        [NotNullExpression]\r\n";
                }
                validStr = validStr + "        [MaxWordsExpression(" + field.length + ")]\r\n";
            }
            else if (xtype == "int" && field.isNullAble == "0")//冲突处理，不能自定义注解
            {
                validStr = validStr + "        [Required(ErrorMessage= \"{0}只能填写数字\")]\r\n";
            }
            else if (field.xType == "int")
            {
                validStr = validStr + "        [IsNumberExpression]\r\n";
            }
            return validStr;
        }



        private void btn_EditSQLCon_Click(object sender, EventArgs e)
        {
            string strXmlPath = "Config.xml";
            XmlHelper.SetXmlFileValue(strXmlPath, "DataBase", txt_SQL.Text);
            MessageBox.Show("修改成功，重新载入");
            init();
        }

        private void txt_ModelName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                OutputCode();
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //路径设置
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtfilepath.Text = fbd.SelectedPath + "\\";
                string strXmlPath = "Config.xml";
                XmlHelper.SetXmlFileValue(strXmlPath, "Path", txtfilepath.Text);

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string tableName = lb_Tables.Text;
            string pathName = tableName.IndexOf("_") > 0 ? tableName.Split('_')[1] : tableName;
            //创建对应的HTML目录
            if (!Directory.Exists(txtfilepath.Text + pathName))
            {
                Directory.CreateDirectory(txtfilepath.Text + pathName);
            }

            //导出Index
            using (FileStream aFile = new FileStream(txtfilepath.Text + pathName + "\\Index.cshtml", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8))
                {
                    sw.WriteLine(txt_Index.Text);
                }
            }
            //导出Create
            using (FileStream aFile = new FileStream(txtfilepath.Text + pathName + "\\Create.cshtml", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8))
                {
                    sw.WriteLine(txt_Create.Text);
                }
            }
            //导出Edit
            using (FileStream aFile = new FileStream(txtfilepath.Text + pathName + "\\Edit.cshtml", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8))
                {
                    sw.WriteLine(txt_Edit.Text);
                }
            }
            if (cb_EnableParent.Checked && cb_MulView.Checked)
            {
                //导出CreateParent
                using (FileStream aFile = new FileStream(txtfilepath.Text + pathName + "\\CreateParent.cshtml", FileMode.OpenOrCreate))
                {
                    using (StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8))
                    {
                        sw.WriteLine(txt_CreateParent.Text);
                    }
                }
                //导出EditParent
                using (FileStream aFile = new FileStream(txtfilepath.Text + pathName + "\\EditParent.cshtml", FileMode.OpenOrCreate))
                {
                    using (StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8))
                    {
                        sw.WriteLine(txt_EditParent.Text);
                    }
                }
            }

            //导出IBLL
            using (FileStream aFile = new FileStream(txtfilepath.Text + "\\I" + tableName + "BLL.cs", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8))
                {
                    sw.WriteLine(tbIBLL.Text);
                }
            }

            //导出BLL
            using (FileStream aFile = new FileStream(txtfilepath.Text + "\\" + tableName + "BLL.cs", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8))
                {
                    sw.WriteLine(txt_PartialBLL.Text);
                }
            }
            //导出Model
            using (FileStream aFile = new FileStream(txtfilepath.Text + "\\" + tableName + "Model.cs", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8))
                {
                    sw.WriteLine(txt_PartialModel.Text);
                }
            }
            //导出Controller
            using (FileStream aFile = new FileStream(txtfilepath.Text + pathName + "Controller.cs", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8))
                {
                    sw.WriteLine(txt_Controller.Text);
                }
            }

            string path = txtfilepath.Text.Replace("\\\\", "\\");
            System.Diagnostics.Process.Start("explorer.exe", path);

        }

        private void txt_TableName1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txt_TableName2_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txt_TableName3_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (txt_TableKey1.Text == "" || !cb_EnableParent.Checked)
            {
                MessageBox.Show("信息不完整,无法生成！");
            }
            else
            {
                OutputCode();
            }

        }

        private void txt_TableName1_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt_TableName1.Text) && txt_TableName1.Text.IndexOf("_") > 0)
            {
                string leftStr = GetLeftStr(txt_TableName1.Text);
                txt_TableKey1.Text = txt_TableName1.Text.Replace(leftStr + "_", "") + "Id";
            }
        }

    }
}
