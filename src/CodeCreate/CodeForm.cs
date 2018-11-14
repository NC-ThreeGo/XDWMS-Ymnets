using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCreate.Code;
using System.Data.SqlClient;
using CommonData.Util.String;
using CommonData.Model.Core;

namespace CodeCreate
{
    public partial class CodeForm : Form
    {
        public CodeForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CodeForm_Load(object sender, EventArgs e)
        {
            
        }


        /// <summary>
        /// 选择登录方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbSQLServer_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSQLServer.Checked)
            {
                this.txtUserName.Enabled = true;
                this.txtPassword.Enabled = true;
            }
            else 
            {
                this.txtUserName.Enabled = false;
                this.txtPassword.Enabled = false;
            }
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlServerName.SelectedItem==null)
                {
                    MessageBox.Show("请选择服务器");
                    return;
                }
                DataTable table = Core.Core.GetDataBase(ddlServerName.SelectedItem.ToString(), txtUserName.Text, txtPassword.Text);
                if (table != null && table.Rows.Count > 0)
                {
                    MessageBox.Show("数据库服务器连接成功");
                    ddlDataBaseName.Items.Clear();
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        ddlDataBaseName.Items.Add(table.Rows[i]["name"].ToString());
                    }
                }
                else
                {
                    MessageBox.Show("数据库服务器连接失败");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("数据库服务器连接失败");
                Console.WriteLine(ee.Message);
            }
        }

        /// <summary>
        /// 选择数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlDataBaseName_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetTable();
        }

        private void GetTable()
        {
            try
            {
                DataTable table = Core.Core.GetTable(ddlServerName.SelectedItem.ToString(), ddlDataBaseName.SelectedItem.ToString(), txtUserName.Text, txtPassword.Text);
                ddlTableNames.Items.Clear();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    ddlTableNames.Items.Add(table.Rows[i]["name"].ToString());
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("数据表查询连接发生异常");
                Console.WriteLine(ee.Message);
            }
        }

        /// <summary>
        /// 生成代码
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            string servername = ddlServerName.SelectedItem.ToString();
            string tableName = ddlTableNames.SelectedItem.ToString();
            string connection = null;
            SqlConnection con = null;
            if (rbWindows.Checked)
            {
                connection = "server=" + servername + ";database=" + ddlDataBaseName.SelectedItem.ToString() + ";Integrated Security=true";
            }
            else
            {
                connection = "server=" + servername + ";database=" + ddlDataBaseName.SelectedItem.ToString() + "; uid=" + txtUserName.Text + ";pwd=" + txtPassword.Text;
            }

            try
            {
                con = new SqlConnection(connection);
                con.Open();
                StringBuilder sb = new StringBuilder("select ");
                sb.Append("syscolumns.name as ColName ,");
                sb.Append("systypes.name as ColTypeName ,");
                sb.Append("syscolumns.length,");
                sb.Append("sys.extended_properties.value as Mark ,");
                sb.Append("IsAuto=case ");
                sb.Append("when ( (SELECT COLUMNPROPERTY( OBJECT_ID('" + tableName + "'),'syscolumns.name','IsIdentity')) =1) ");
                sb.Append("then 'true' else 'false' end,");
                sb.Append("AllowNull=case ");
                sb.Append("when (syscolumns.isnullable=0) then 'false' ");
                sb.Append("else 'true' end,");
                sb.Append("IsPK = Case ");
                sb.Append(" when exists ( select 1 from sysobjects inner join sysindexes on sysindexes.name = sysobjects.name inner join sysindexkeys on sysindexes.id = sysindexkeys.id and sysindexes.indid = sysindexkeys.indid where xtype='PK' and parent_obj = syscolumns.id and sysindexkeys.colid = syscolumns.colid )  ");
                sb.Append(" then 'true' else 'false' end ,");
                sb.Append(" IsIdentity = Case syscolumns.status when 128 then 1 else 0 end from syscolumns inner join systypes on ( syscolumns.xtype = systypes.xtype and systypes.name <>'_default_' and systypes.name<>'sysname' ) left outer join sys.extended_properties on ( sys.extended_properties.major_id=syscolumns.id and minor_id=syscolumns.colid ) where syscolumns.id = (select id from sysobjects where name='"+tableName+"')  ");
                sb.Append(" order by syscolumns.colid");
                SqlCommand command = new SqlCommand(sb.ToString(), con);
                SqlDataReader reader = command.ExecuteReader();
                ddlTableNames.Items.Clear();
                StringBuilder sbCode = new StringBuilder("");
                sbCode.Append("using System;\n");
                sbCode.Append("using System.Collections.Generic;\n");
                sbCode.Append("using System.Linq;\n");
                sbCode.Append("using System.Text;\n");
                sbCode.Append("using CommonData.Entity;\n");
                sbCode.Append("using CommonData.Model.Core;\n");
                sbCode.Append("\n");
                sbCode.Append("namespace Entity\n");
                sbCode.Append("{\n");

                sbCode.AppendFormat("\t[Serializable]\n");
                sbCode.AppendFormat("\t[TableAttribute(DBName = \"\", Name = \"{0}\", PrimaryKeyName = \"@PrimaryKeyName\", IsInternal = false)]\n", tableName);
                sbCode.AppendFormat("\tpublic class {0}:BaseEntity\n", tableName.FirstToUpper(tableName));
                sbCode.Append("\t{\n");
                sbCode.AppendFormat("\t\tpublic {0}()\n", tableName.FirstToUpper(tableName));
                sbCode.Append("\t\t{\n");
                sbCode.Append("\t\t}\n\n");
                string pkName="Id";
                while (reader.Read())
                {
                    if(reader["IsPK"].ToString()=="true")
                    {
                        pkName=reader["ColName"].ToString();
                    }
                    sbCode.AppendFormat("\t\tprivate {0} {1};\n", GetType(reader["ColTypeName"].ToString()), reader["ColName"].ToString().FirstToLower(reader["ColName"].ToString()));
                    sbCode.AppendFormat("\t\t[ColumnAttribute(Name = \"{0}\", IsPrimaryKey = {1}, AutoIncrement = {2}, DataType = DataType.{3}, CanNull = {4})]\n", reader["ColName"].ToString(), reader["IsPK"].ToString(), reader["IsAuto"].ToString(), GetDataType(reader["ColTypeName"].ToString()), reader["AllowNull"].ToString());
                    sbCode.AppendFormat("\t\tpublic {0} {1}\n", GetType(reader["ColTypeName"].ToString()), reader["ColName"].ToString().FirstToUpper(reader["ColName"].ToString()));
                    sbCode.Append("\t\t{\n");
                    sbCode.Append("\t\t\tget { return " + "".FirstToLower(reader["ColName"].ToString()) + "; }\n");
                    sbCode.Append("\t\t\tset { " + "".FirstToLower(reader["ColName"].ToString()) + " = value; }\n");
                    sbCode.Append("\t\t}\n\n");
                }
                sbCode.Append("\t}\n");
                sbCode.Append("}\n");
                sbCode.Replace("@PrimaryKeyName", pkName);
                rtxtCode.Text = sbCode.ToString();
            }
            catch
            {
                MessageBox.Show("连接失败");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
            GetTable();
        }


        private string GetType(string name)
        {
            string value = "string";
            switch (name)
            { 
                case "int":
                    value = "int";
                    break;
                case "bigint":
                    value = "long";
                    break;
                case "bit":
                    value = "int";
                    break;
                case "char":
                    value = "string";
                    break;
                case "date":
                    value = "DateTime";
                    break;
                case "datetime":
                    value = "DateTime";
                    break;
                case "datetime2":
                    value = "DateTime";
                    break;
                case "float":
                    value = "double";
                    break;
                case "money":
                    value = "double";
                    break;
                case "nchar":
                    value = "string";
                    break;
                case "ntext":
                    value = "string";
                    break;
                case "nvarchar":
                    value = "string";
                    break;
                case "varchar":
                    value = "string";
                    break;
                case "text":
                    value = "string";
                    break;
                
            }
            return value;
        }

        public string GetDataType(string name)
        {
            string value = "Int";
            switch (name.ToLower())
            {
                case "int":
                    value = DataType.Int.ToString();
                    break;
                case "bigint":
                    value = DataType.Bigint.ToString();
                    break;
                case "bit":
                    value = DataType.Bit.ToString();
                    break;
                case "char":
                    value = DataType.Char.ToString();
                    break;
                case "date":
                    value = DataType.Datetime.ToString();
                    break;
                case "datetime":
                    value = DataType.Datetime.ToString();
                    break;
                case "datetime2":
                    value = DataType.Datetime.ToString();
                    break;
                case "float":
                    value = DataType.Float.ToString();
                    break;
                case "money":
                    value = DataType.Money.ToString();
                    break;
                case "nchar":
                    value = DataType.Nchar.ToString();
                    break;
                case "ntext":
                    value = DataType.Ntext.ToString();
                    break;
                case "nvarchar":
                    value = DataType.Nvarchar.ToString();
                    break;
                case "varchar":
                    value = DataType.Varchar.ToString();
                    break;
                case "text":
                    value = DataType.Text.ToString();
                    break;
            }
            return value;
        }


        /// <summary>
        /// 刷新获取数据库服务名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

    }
}
