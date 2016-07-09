using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using WhitClassBuilder;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;

public partial class _Default : System.Web.UI.Page
{
    public static string msSqlConnString = ConfigurationManager.AppSettings["msSqlConn"].ToString();
    public static string mySqlConnString = ConfigurationManager.AppSettings["mySqlConn"].ToString();
    public static string appversion=ConfigurationManager.AppSettings["appversion"].ToString();
    public static int dfltDbType = Convert.ToInt32(ConfigurationManager.AppSettings["defaultDBTypeIndex"].ToString());
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblVersion.Text = appversion;
            CheckType();
        }
    }
   
    private void CheckType()
     {
         ddSqlType.SelectedIndex = dfltDbType;
         if (dfltDbType == 0)
         {
             tbSqConn.Text = msSqlConnString;
         }
         else if (dfltDbType == 1)
         {
             tbSqConn.Text = mySqlConnString;

         }
     }
        protected void Button1_Click(object sender, EventArgs e)
        {
            BuildClass bCls = new BuildClass();
            tbOut.Text = bCls.GenClass(tbIn.Text, cbWrap.Checked, cbClass.Checked, tbClassName.Text, cbGenDR.Checked, tbReader.Text, tbSqConn.Text, cbIsPublic.Checked, tbDbDatabase.Text, tbDbTable.Text,ddSqlType.SelectedItem.Value);
        }

        protected void ibSQL_Click(object sender, ImageClickEventArgs e)
        {
            pnlSQL.Visible = !pnlSQL.Visible;
        }
        
        protected void btnLoadSQL_Click(object sender, EventArgs e)
        {
                BuildClass bC = new BuildClass();
                List<dbColumn> gsTable;
                if (ddSqlType.SelectedIndex == 0)
                {
                    gsTable =dbColumn.MsSqlColumnList(tbSqConn.Text, tbDbDatabase.Text, tbDbTable.Text);
                }
                else if (ddSqlType.SelectedIndex == 1)
                {
                    gsTable = dbColumn.MySqlColumnList(tbSqConn.Text, tbDbDatabase.Text, tbDbTable.Text);

                }
                else
                {
                    gsTable = dbColumn.MsSqlColumnList(tbSqConn.Text, tbDbDatabase.Text, tbDbTable.Text);
                }
                lblColCnt.Text = gsTable.Count.ToString() + " DB Columns";
                string bldStrg = "";
                foreach (dbColumn dbc in gsTable)
                {
                    bldStrg += dbc.DataType + " " + dbc.ColumnName + " MaxChars:" + dbc.MaxChars + ",Nullable:" + dbc.IsNullable + "\n";
                }
                tbIn.Text = bldStrg;
        }

        protected void btnSqlChk_Click(object sender, EventArgs e)
        {
            BuildClass bc = new BuildClass();
            try
            {
                if (ddSqlType.SelectedIndex == 0)
                {
                    if (bc.ValidSql(tbSqConn.Text))
                    {
                        lblDConnChk.Text = "SQL OK";
                    }
                    else
                    {
                        lblDConnChk.Text = "Failed";
                    }
                }
                else if (ddSqlType.SelectedIndex==1)
                {
                    if (bc.ValidMySql(tbSqConn.Text))
                    {
                        lblDConnChk.Text = "OK";
                    }
                    else
                    {
                        lblDConnChk.Text = "Failed";
                    }
                }
            }
            catch(Exception sqx)
            {
                lblDConnChk.Text = sqx.ToString();
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            pnlSQL.Visible = !pnlSQL.Visible;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect(Request.ServerVariables["SCRIPT_NAME"].ToString());
        }

        protected void cbClass_CheckedChanged(object sender, EventArgs e)
        {
            tbClassName.Visible = cbClass.Checked;
            pnlClassDetails.Visible = cbClass.Checked;
        }

        protected void cbGenDR_CheckedChanged(object sender, EventArgs e)
        {
            bool vs = cbGenDR.Checked;
            if (cbGenDR.Checked && tbClassName.Text == "")
            {
                tbClassName.Text = "MyClass";
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddSqlType.SelectedIndex == 0)
            {
                tbSqConn.Text = msSqlConnString;
            }
            else
            {
                tbSqConn.Text = mySqlConnString;

            }
        }
        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            bool v = !lblOp1.Visible;
            lblOp1.Visible = v;
            lblOp2.Visible = v;
            tbRefName.Visible = v;
            tbReader.Visible = v;

            
        }
    
}//classover


