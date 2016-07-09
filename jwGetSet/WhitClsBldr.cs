using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace WhitClassBuilder
{
    public class BuildClass
    {
        public string cDisclaimer = "/*WhitClassBuilder-247Coding.com\n© 2016, WebMeToo.com and its affiliates.\nWhitClsBldr.dll is currently free"+
            " for public and private use without restriction as long as this comment/disclaimer section is left fully in tact on output strings.\n"+
            "Website: http://247coding.com/wcb\nLibrary\n"+
            "Disclaimer: By using this library or any related examples, means that you fully agree that"+
            "you are doing so at your own risk.\n"+
            "You also release any WhitClassBuilder library affiliations from any and all negative result created from it's use.\nThis library comes with no guarantees.\n*/\n ";
        public string CapString(string inPrivate)
        {

            string capChar = inPrivate[0].ToString().ToUpper();
            return capChar + inPrivate.Remove(0, 1);

        }

        public string GenClass(string inVarString, bool wrapInRegion, bool wrapInClass, string inClassName, bool genDataReader,string inReaderName, string inSqlConnString, bool publicDR, string inSQLDatabase, string inSQLTable, string inSqlType)
        {
             string gspriv = "private %type %var;";
            string gspub = "public %type %cvar\n{\nget{return %var;}\nset {%var=value;}\n}\n";
            string[] tbSplit = inVarString.Replace("/","").Replace("&#47;","").Split('\n');
            string rvalue = "";
            string dValue="";
            for (int i = 0; i < tbSplit.Length; i++)
            {
               
                string[] lsplit = tbSplit[i].Split(' ');
                if (lsplit.Length > 1)
                {
                    string typ = lsplit[0].Trim();
                    string var = lsplit[1].Trim();
                    string note = "";
                    if (lsplit.Length > 2)
                    {
                        note = lsplit[2];
                    }
                    string cv = CapString(var);// cupp + var.Remove(0, 1);
                    string ntt = "";
                    if (note != "")
                    {
                        ntt = "//" + note + "\n";
                    }
                    rvalue += ntt + gspriv.Replace("%type", lsplit[0]).Replace("%var", var) + "\n" + gspub.Replace("%type", typ).Replace("%cvar", cv).Replace("%var", var) + "\n";
                }
                
            }
            if (wrapInRegion)
            {
                rvalue = "#region Properties " + DateTime.Now.ToString() + "\n" + rvalue + "\n#endregion\n";
            }
            if (wrapInClass)
            {
                if (inClassName == "")
                {
                    inClassName = "MyClass";
                }

                rvalue = "public class " + inClassName + "\n{\npublic " + inClassName + "()\n{\n}//abowClassList\n" + rvalue + "}//classOver\n";
            }
            if (genDataReader)
            {
                // dValue = GenerateDR(inClassName, inReaderName, inVarString, inSqlConnString, publicDR);
                if (inSqlType.ToLower() == "mssql")
                {
                    rvalue = rvalue.Replace("//abowClassList", "//abowClassList\n" + GenClassReader(inClassName, inReaderName, inVarString, inSqlConnString, publicDR, inSQLDatabase, inSQLTable));
                }
                else if (inSqlType.ToLower() == "mysql")
                {
                    rvalue = rvalue.Replace("//abowClassList", "//abowClassList\n" + GenMySqlClassReader(inClassName, inReaderName, inVarString, inSqlConnString, publicDR, inSQLDatabase, inSQLTable));
                
                }

            }           
            return rvalue;
        }
    
        private string GenClassReader(string oClass, string drName, string inClassData, string inSQLConn, bool publicDR, string inDBName, string inTableName)
        {
            string obj = "cObj";
            string dr = drName;
            if (dr == "")
            {
                dr = "dReader";
            }
            if (oClass == "")
            {
                oClass = "MyClass";
            }
            string rValue = cDisclaimer + "//-->Insure that you have using statement reference to SQL Client for class-->\n//using System.Data.SqlClient;\n"+
            "public List<" + oClass + "> Get" + oClass + "()\n{\n"+
            "List<" + oClass + "> cList=new List<" + oClass + ">();\n"+
            "string sqConStrg=\"" + inSQLConn + "\";" +
                "SqlConnection dbC=new SqlConnection(sqConStrg);\n" +
                "string sQ = \"select * from " + inDBName + ".dbo." + inTableName + "\";\n"+
                "dbC.Open();\n"+
                "SqlCommand sC = new SqlCommand(sQ,dbC);\n" +
                "SqlDataReader " + dr + "=sC.ExecuteReader();\n"+
            "while (" + dr + ".Read())\n{\n" + oClass + " "+ obj +" = new " + oClass + "();\n";

            string endObj = "\ncList.Add("+ obj +");\n}\n" + dr + ".Dispose();\n sC.Dispose();"+
            "try{\ndbC.Close();\n}\ncatch{}\ndbC.Dispose();\nreturn cList;\n}\n";
            string[] tbsp = inClassData.Split('\n');

            for (int i = 0; i < tbsp.Length; i++)
            {
                string[] rsplt = tbsp[i].Split(' ');
                if (rsplt.Length > 1)
                {

                    string ctyp = rsplt[0].ToLower().Trim();
                    string ccol = rsplt[1].Trim();
                    if (publicDR)
                    {
                        ccol = CapString(ccol);
                    }
                    if (ctyp == "bool")
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=Convert.ToBoolean(" + dr + "[\"" + ccol + "\"].ToString());\n}\ncatch{}\n";
                    }
                    else if (ctyp == "datetime")
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=Convert.ToDateTime(" + dr + "[\"" + ccol + "\"].ToString());\n}\ncatch{}\n";
                    }
                    else if (ctyp == "int")
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=Convert.ToInt32(" + dr + "[\"" + ccol + "\"].ToString());\n}\ncatch{}\n";
                    }
                    else
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=" + dr + "[\"" + ccol + "\"].ToString();\n}\ncatch{}\n";
                    }
                }

            }
            return rValue + endObj;
        }
        private string GenMySqlClassReader(string oClass, string drName, string inClassData, string inSQLConn, bool publicDR, string inDBName, string inTableName)
        {
            string obj = "cObj";
            string dr = drName;
            if (dr == "")
            {
                dr = "dReader";
            }
            if (oClass=="")
            { 
                oClass = "MyClass"; 
            }

            string rValue =cDisclaimer +  "//-->Insure that you have using statement reference to SQL Client for class-->\n//using MySql.Data;\n//using MySql.Data.MySqlClient;\n" +
            "public List<" + oClass + "> Get" + oClass + "()\n{\n" +
            "List<" + oClass + "> cList=new List<" + oClass + ">();\n" +
            "string sqConStrg=\"" + inSQLConn + "\";\n" +
                "MySqlConnection dbC=new MySqlConnection(sqConStrg);\n" +
                "string sQ = \"select * from " + inDBName + ".dbo." + inTableName + "\";\n" +
                "dbC.Open();\n" +
                "MySqlCommand sC = new MySqlCommand(sQ,dbC);\n" +
                "MySqlDataReader " + dr + "=sC.ExecuteReader();\n" +
            "while (" + dr + ".Read())\n{\n" + oClass + " " + obj + " = new " + oClass + "();\n";

            string endObj = "\ncList.Add(" + obj + ");\n}\n" + dr + ".Dispose();\n sC.Dispose();" +
            "try{\ndbC.Close();\n}\ncatch{}\ndbC.Dispose();\nreturn cList;\n}\n";
            string[] tbsp = inClassData.Split('\n');

            for (int i = 0; i < tbsp.Length; i++)
            {
                string[] rsplt = tbsp[i].Split(' ');
                if (rsplt.Length > 1)
                {

                    string ctyp = rsplt[0].ToLower().Trim();
                    string ccol = rsplt[1].Trim();
                    if (publicDR)
                    {
                        ccol = CapString(ccol);
                    }
                    if (ctyp == "bool")
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=Convert.ToBoolean(" + dr + "[\"" + ccol + "\"].ToString());\n}\ncatch{}\n";
                    }
                    else if (ctyp == "datetime")
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=Convert.ToDateTime(" + dr + "[\"" + ccol + "\"].ToString());\n}\ncatch{}\n";
                    }
                    else if (ctyp == "int")
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=Convert.ToInt32(" + dr + "[\"" + ccol + "\"].ToString());\n}\ncatch{}\n";
                    }
                    else
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=" + dr + "[\"" + ccol + "\"].ToString();\n}\ncatch{}\n";
                    }
                }

            }
            return rValue + endObj;
        }

        private string GenaerateDR(string objectName, string drName, string inClassData, string inSQLQuery, bool publicDR)
        {
            string rValue = "";
            string obj = objectName;
            string dr = drName; 
            string[] tbsp = inClassData.Split('\n');// tbIn.Text.Split('\n');

            for (int i = 0; i < tbsp.Length; i++)
            {
                string[] rsplt = tbsp[i].Split(' ');
                if (rsplt.Length > 1)
                {

                    string ctyp = rsplt[0].ToLower().Trim();
                    string ccol = rsplt[1].Trim();
                    if (publicDR)
                    {
                        ccol = CapString(ccol);
                    }
                    if (ctyp == "bool")
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=Convert.ToBoolean(" + dr + "[\"" + rsplt[1] + "\"].ToString());\n}\ncatch{}\n";
                    }
                    else if (ctyp == "datetime")
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=Convert.ToDateTime(" + dr + "[\"" + rsplt[1] + "\"].ToString());\n}\ncatch{}\n";
                    }
                    else if (ctyp == "int")
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=Convert.ToInt32(" + dr + "[\"" + rsplt[1] + "\"].ToString());\n}\ncatch{}\n";
                    }
                    else
                    {
                        rValue += "try\n{\n\t" + obj + "." + ccol + "=" + dr + "[\"" + rsplt[1] + "\"].ToString();\n}\ncatch{}\n";
                    }
                }

            }
            return rValue;
        }

        public bool ValidSql(string inSqlConnection)
        {
            bool cR = false;
            SqlConnection dbc = new SqlConnection();
            dbc.ConnectionString = inSqlConnection;
            try
            {
                dbc.Open();
                if (dbc.State == System.Data.ConnectionState.Open)
                {
                    cR = true;
                }
            }
            catch
            {
            }
            try
            {
                if (dbc.State == System.Data.ConnectionState.Open)
                {
                    try
                    {
                        dbc.Close();
                    }
                    catch { }
                
                }
            }
            catch { }
            dbc.Dispose();
            return cR;
        }
        public bool ValidMySql(string inMySqlConnection)
        {
            bool rV = false;
            try
            {
                MySql.Data.MySqlClient.MySqlConnection msConn = new MySqlConnection();
                msConn.ConnectionString = inMySqlConnection;
                msConn.Open();
                if (msConn.State == System.Data.ConnectionState.Open)
                {
                    rV = true;
                    msConn.Close();
                    msConn.Dispose();
                }
               
            }
            catch { }
            return rV;
        }
    }
    
    public class dbColumn
    {
        public static string GetDataType(string dtype)
        {
            string rType = dtype;
            if (dtype.ToLower().StartsWith("varch") || dtype.ToLower().StartsWith("nvar") || dtype.ToLower().StartsWith("text") || dtype.ToLower().StartsWith("tinytext"))
            {
                rType = "string";
            }
            else if (dtype.ToLower() == "bit")
            {
                rType = "bool";
            }
            else if (dtype.ToLower() == "date" || dtype.ToLower() == "datetime" || dtype.ToLower() == "datetime1" || dtype.ToLower() == "datetime2")
            {
                rType = "DateTime";
            }
            else if (dtype.ToLower() == "smallint" || dtype.ToLower() == "int")
            {
                rType = "int";
            }
            else
            {
                rType = dtype;
            }
            return rType;
        }
        public dbColumn()
        {
        }
        
        public static List<dbColumn> MySqlColumnList(string inConnString, string inDatabaseName, string inTableName)
        {
            List<dbColumn> colList = new List<dbColumn>();
            try
            {
                string msQuery = "SELECT `COLUMN_NAME`,`DATA_TYPE`,`CHARACTER_MAXIMUM_LENGTH`,`IS_NULLABLE` FROM `INFORMATION_SCHEMA`.`COLUMNS` WHERE `TABLE_SCHEMA`=@dbn AND `TABLE_NAME`=@tbn;";
                MySqlConnection dbc = new MySqlConnection();
                dbc.ConnectionString = inConnString;
                dbc.Open();
                MySqlCommand cmd = new MySqlCommand(msQuery, dbc);
                cmd.Parameters.AddWithValue("@dbn", inDatabaseName.Replace("'", ""));
                cmd.Parameters.AddWithValue("@tbn", inTableName.Replace("'", ""));

                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    dbColumn dcol = new dbColumn();

                    try
                    {
                        string cnCheck = dr["COLUMN_NAME"].ToString();
                        string csrt = cnCheck[0].ToString().ToLower();
                        dcol.ColumnName = csrt + cnCheck.Remove(0, 1); //lowercase the first char..
                    }
                    catch
                    { }
                    try
                    {
                        dcol.dataType = GetDataType(dr["DATA_TYPE"].ToString());
                    }
                    catch
                    { }
                    try
                    {
                        dcol.MaxChars = Convert.ToInt32(dr["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    }
                    catch
                    { }
                    try
                    {
                        if (dr["IS_NULLABLE"].ToString().ToLower() == "yes")
                        {
                            dcol.isNullable = true;
                        }
                        else
                        {
                            dcol.isNullable = false;
                        }

                    }
                    catch
                    { }
                    try
                    {
                        string mlgth = dr["CHARACTER_MAXIMUM_LENGTH"].ToString();
                        if (mlgth != "")
                        {
                            dcol.MaxChars = Convert.ToInt32(mlgth);
                        }
                        else
                        {
                            dcol.maxChars = -1;
                        }
                    }
                    catch { }
                    colList.Add(dcol);
                }
            }
            catch(Exception exc)
            {
                dbColumn erc = new dbColumn();
                erc.columnName = exc.ToString();
                erc.DataType = "SQL_ERROR";
                colList.Add(erc);
            }

            return colList;
        }
       
        public static List<dbColumn> MsSqlColumnList(string inConnString, string inDatabaseName, string inTableName)
        {
            List<dbColumn> colList = new List<dbColumn>();
            string errs = "";
            if (inDatabaseName != "" && inTableName != "")
            {
                try
                {
                    SqlConnection dbc = new SqlConnection();
                    dbc.ConnectionString = inConnString;
                    dbc.Open();
                    string dbn = inDatabaseName.Trim().Replace("'", "") + ".INFORMATION_SCHEMA.COLUMNS";
                    string tbn = inTableName.Trim().Replace("'", "");
                    string sqq = "select [COLUMN_NAME],[IS_NULLABLE],[DATA_TYPE],[CHARACTER_MAXIMUM_LENGTH],[ORDINAL_POSITION] FROM " + dbn + " WHERE TABLE_NAME = '" + tbn + "' ORDER by ORDINAL_POSITION ASC";
                    SqlCommand cmd = new SqlCommand(sqq, dbc);
                    cmd.Parameters.AddWithValue("@tbn", inTableName.Replace("'", ""));
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        dbColumn dcol = new dbColumn();
                        try
                        {
                            string cnCheck = dr["COLUMN_NAME"].ToString();
                            string csrt = cnCheck[0].ToString().ToLower();
                            dcol.ColumnName = csrt + cnCheck.Remove(0, 1); //lowercase the first char..
                            string inul = dr["IS_NULLABLE"].ToString();
                            if (inul.ToUpper() == "YES")
                            {
                                dcol.IsNullable = true;
                            }
                            else
                            {
                                dcol.IsNullable = false;
                            }
                            dcol.dataType = GetDataType(dr["DATA_TYPE"].ToString());

                            string mlgth = dr["CHARACTER_MAXIMUM_LENGTH"].ToString();
                            if (mlgth != "")
                            {
                                dcol.MaxChars = Convert.ToInt32(mlgth);
                            }
                            else
                            {
                                dcol.maxChars = -1;
                            }
                            dcol.OrdinalPosition = Convert.ToInt32(dr["ORDINAL_POSITION"]);
                            colList.Add(dcol);
                        }
                        catch (Exception exc)
                        {
                            dbColumn erc = new dbColumn();
                            erc.columnName = exc.ToString();
                            erc.DataType = "SQL_ERROR";
                            colList.Add(erc);
                        }
                    }
                    dr.Dispose();
                    cmd.Dispose();
                    if (dbc.State == System.Data.ConnectionState.Open)
                    {
                        try
                        {
                            dbc.Close();
                        }
                        catch { }
                    }
                    dbc.Dispose();
                }
                catch (Exception ex)
                {
                    dbColumn erc = new dbColumn();
                    erc.columnName = ex.ToString();
                    erc.DataType = "SQL_ERROR";
                    colList.Add(erc);
                }
            }
            return colList;
        }


        #region GetSets
        private string columnName;
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }
        private bool isNullable;
        public bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
        }
        private string dataType;
        public string DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }
        private int maxChars;
        public int MaxChars
        {
            get { return maxChars; }
            set { maxChars = value; }
        }
        private int ordinalPosition;
        public int OrdinalPosition
        {
            get { return ordinalPosition; }
            set { ordinalPosition = value; }
        }
        private string colErrors;
        public string ColErrors
        {
            get { return colErrors; }
            set { colErrors = value; }
        }
        #endregion
    }

   
}
class WebSite
{
    string Name = "247Coding.com";
}