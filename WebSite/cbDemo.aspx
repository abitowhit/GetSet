<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="cbDemo.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="SiteStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <asp:UpdatePanel ID="UpdatePanel1" runat="server" AssociatedUpdatePanelID="UpdatePanel1"><ContentTemplate>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server"><ProgressTemplate>
            &nbsp;<img src="Graphics/GreenArrowWait.gif" alt="Please Wait.." class="_PanelRounded" style="background-color:white"/>Processing...
                                                                </ProgressTemplate></asp:UpdateProgress>
    <asp:Panel ID="Panel1" runat="server" CssClass="_PanelRounded" Style="font-size:small;background-color:#96A995;float:left;vertical-align:central" ForeColor="#333333">
        &nbsp;<asp:Label ID="lblHeader" runat="server" Text="247Coding.com - Class Builder" Style="color:#B3C5B3;font-size:x-large"></asp:Label>
        
        <asp:Panel ID="pnlSQL" runat="server" BackColor="#D4E2D4" CssClass="_PanelRounded" Visible="False" Style="float:left" Width="90%">
            <asp:Button ID="Button2" runat="server" CssClass="_PanelRounded" Text="X" style="float:right" OnClick="Button2_Click" ToolTip="Close SQL piece"/>
            <span class="auto-style1">Convert your Database table into class properties</span><br />
            <em><br /> </em>Connection String (Example Provided)<br /> <asp:DropDownList ID="ddSqlType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" ToolTip="Select which type of database you are connecting to">
                <asp:ListItem Selected="True" Value="mssql">Microsoft SQL</asp:ListItem>
                <asp:ListItem Value="mysql">MySQL</asp:ListItem>
            </asp:DropDownList>
            <asp:Label ID="lblSQLMsg" runat="server" style="color: #FF0000"></asp:Label>
            <br />
            <asp:TextBox ID="tbSqConn" runat="server" Width="98%" CssClass="_PanelRounded" ToolTip="Enter a connection string to connect to your database">Data Source=localhost\sqlx01;Initial Catalog=cbexample;Persist Security Info=True;User ID=TstUserReadOnly;Password=247CodeReadOnly</asp:TextBox>
            <br />
            <asp:Button ID="btnSqlChk" runat="server" CssClass="_PanelRounded" OnClick="btnSqlChk_Click" Text="Test Connection" ToolTip="Click to see if your connection string can connect." />
            <asp:Label ID="lblDConnChk" runat="server"></asp:Label>
            <br />
            <table bgcolor="White" class="_PanelRounded" width="80%">
                <tr>
                    <td>Database Name</td>
                    <td>Table Name</td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="tbDbDatabase" runat="server" Width="100%" BackColor="GhostWhite" CssClass="_PanelRounded" ToolTip="Enter the database or schema you are connecting to">cbexample</asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="tbDbTable" runat="server" Width="100%" BackColor="GhostWhite" CssClass="_PanelRounded" ToolTip="Enter the database table you wish to return properties from">example_table</asp:TextBox>
                    </td>
                </tr>
            </table>
            <asp:Label ID="lblColCnt" runat="server"></asp:Label>
            <br />
            <asp:Button ID="btnLoadSQL" runat="server" CssClass="_PanelRounded" Text="Load Columns" OnClick="btnLoadSQL_Click" ToolTip="If your connection, database and table are correct, your columns will be loaded below." />
        </asp:Panel>

        <asp:Panel ID="pnlText" runat="server" BackColor="#D4E2D4" CssClass="_PanelRounded" Style="float:left" Width="90%">
            <asp:ImageButton ID="ibSQL" runat="server" ImageUrl="~/Graphics/SQL.gif" OnClick="ibSQL_Click" style="float:left" ToolTip="Create Classs/Properties from a SQL table" />
            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Graphics/refreshdb.gif" OnClick="ImageButton1_Click" ToolTip="Refresh" />
            <asp:Label ID="lblTitle" runat="server" Text="Enter property list, one per line (variableType variablePrivateName)"></asp:Label>
            <asp:TextBox ID="tbIn" runat="server" BackColor="GhostWhite" CssClass="_PanelRounded" Height="124px" TextMode="MultiLine" Width="80%" ToolTip="Enter properties here. i.e. type&lt;space&gt;name. One property per line."></asp:TextBox>
            <br />
            <asp:Button ID="Button1" runat="server" CssClass="_PanelRounded" OnClick="Button1_Click" Text="Process" ToolTip="Click to process class properties. (get-sets)" />
            &nbsp;<asp:CheckBox ID="cbClass" runat="server" AutoPostBack="True" CssClass="_PanelRounded" OnCheckedChanged="cbClass_CheckedChanged" Text="Include Class" ToolTip="Wraps your properties in a class object in result" />
            <asp:CheckBox ID="cbWrap" runat="server" Checked="True" CssClass="_PanelRounded" Text="Wrap in region" ToolTip="Wraps your property get-sets in a region" />
            <asp:Panel ID="pnlClassDetails" runat="server" BackColor="White" CssClass="_PanelRounded" Visible="False">
                Class Options<br />
                <asp:TextBox ID="tbClassName" runat="server" CssClass="_PanelRounded" Visible="False" Width="17%" ToolTip="This is what your new class will be called">ClassName</asp:TextBox>
                <asp:CheckBox ID="cbGenDR" runat="server" CssClass="_PanelRounded" OnCheckedChanged="cbGenDR_CheckedChanged" Text="Generate DB List Class" Checked="True" ToolTip="If checked, a list of class objects will be created within your new class, built from the sql columns." />
                <asp:CheckBox ID="cbIsPublic" runat="server" Checked="True" Text="Public DataReader?" ToolTip="Capitalize first char of returned reader object (sets public property in class object)" Visible="False" />
                <br />
                <asp:LinkButton ID="LinkButton1" runat="server" ForeColor="#999999" OnClick="LinkButton1_Click">More Options..</asp:LinkButton>
                &nbsp;<asp:Label ID="lblOp1" runat="server" Text="Object Name" Visible="False"></asp:Label>
                <asp:TextBox ID="tbRefName" runat="server" CssClass="_PanelRounded" ToolTip="Object name you will call reference" Visible="False" Width="17%">obj0</asp:TextBox>
                <asp:Label ID="lblOp2" runat="server" Text="Data Reader Name" Visible="False"></asp:Label>
                <asp:TextBox ID="tbReader" runat="server" CssClass="_PanelRounded" ToolTip="Your data reader name. Only needed if you need to change the reader name." Visible="False" Width="17%">dr0</asp:TextBox>
            </asp:Panel>
        </asp:Panel>
        <asp:TextBox ID="tbOut" runat="server" CssClass="_PanelRounded" Height="103px" TextMode="MultiLine" Width="90%" ToolTip="Results"></asp:TextBox>
        <asp:TextBox ID="tbDR" runat="server" CssClass="_PanelRounded" Height="103px" TextMode="MultiLine" Visible="False" Width="90%"></asp:TextBox>
        <br /><a href="http://247Coding.com" target="247coding" style="float:right">247Coding.com</a>
        <asp:Label ID="lblVersion" runat="server"></asp:Label>
        <br />
    </asp:Panel>
        </ContentTemplate></asp:UpdatePanel>
    
</asp:Content>

