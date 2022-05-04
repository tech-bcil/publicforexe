<%@ Page Title="" Language="C#" MasterPageFile="~/WebPages/MobiVUEMaster.master" AutoEventWireup="true" CodeFile="Reconcilation.aspx.cs" Inherits="WebPages_Reconcilation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../css/CalendarControl.css" type="text/css" rel="Stylesheet" />

    <script language="javascript" src="../js/CalendarControl.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function ClearFields() {
            document.getElementById('<%= lblErrorMsg.ClientID%>').value = "";
            document.getElementById('<%= ddlSite.ClientID%>').value = "";
           document.getElementById('<%= ddlFloor.ClientID%>').value = "";
            document.getElementById('<%= ddlStore.ClientID%>').value = "";
            document.getElementById('<%= txtRecId.ClientID%>').value = "";
            document.getElementById('<%= txtFromDate.ClientID%>').value = "";
            document.getElementById('<%= txtToDate.ClientID%>').value = "";
        }
        function ShowErrMsg(msg) {
            document.getElementById('<%= lblErrorMsg.ClientID%>').innerHTML = msg.toString();
        }
        function ShowUnAuthorisedMsg() {
            document.getElementById('<%= lblErrorMsg.ClientID%>').innerHTML = 'Please Note : You are not authorised to execute this operation!';
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="clear">
    </div>
    <div id="wrapper">
        <div id="pageTitle">
            ASSET RECONCILIATION
        </div>
    </div>
    <div id="wrapper1">
        <table id="Table1" runat="server" cellspacing="10" style="width: 100%" align="center">
            <tr>
                <td colspan="4">
                    <table id="Table2" runat="server" style="width: 100%;" cellspacing="10" align="center">                      
                        
                        <tr>                                                  
                            <td style="text-align: left" colspan="3">
                                  <asp:Button ID="btnStart" runat="server"  Text="Initiate ReConcilation"  CssClass="button"
                                            TabIndex="26" Width="150px" ToolTip="Start ReConcile" style="margin-bottom:20px;"
                                       onclick="btnStart_Click"   />
                                 <asp:Button ID="btnStop" runat="server"  Text="Stop ReConcilation"  CssClass="button"
                                            TabIndex="26" Width="150px" ToolTip="Stop ReConcile" style="margin-bottom:20px;"
                                     onclick="btnStop_Click"     />                              
                            </td>
                        </tr>
                        <tr>
                             <td style="text-align: left">
                                        <asp:Label ID="Label2" runat="server" Text="Site Location :" CssClass="label"></asp:Label>
                                  </td>
                                  <td style="text-align: left">
                                  <asp:DropDownList runat="server" ID="ddlSite" CssClass="dropdownlist" Width="200" AutoPostBack="true" OnSelectedIndexChanged="ddlSite_SelectedIndexChanged" >
                                        </asp:DropDownList>
                                    </td>
                                  
                                    <td style="text-align: left">
                                        <asp:Label ID="Label4" runat="server" Text="Floor :" CssClass="label"></asp:Label>
                                        </td>
                                  <td style="text-align: left">
                                         <asp:DropDownList runat="server" ID="ddlFloor" CssClass="dropdownlist"  Width="200"  AutoPostBack="true"  ToolTip="Select Floor" OnSelectedIndexChanged="ddlFloor_SelectedIndexChanged"></asp:DropDownList>
                                    </td>
                            </tr>
                         <tr>
                             <td style="text-align: left">
                                        <asp:Label ID="Label1" runat="server" Text="Store :" CssClass="label"></asp:Label>
                                 </td>
                                  <td style="text-align: left">
                                         <asp:DropDownList runat="server" ID="ddlStore" CssClass="dropdownlist"  Width="200" OnSelectedIndexChanged="ddlStore_SelectedIndexChanged" ToolTip="Select Store"></asp:DropDownList>
                                    </td>                           
                       
                        </tr>
                         <tr Visible="false">
                             <td style="text-align: left">
                                 <asp:Label ID="Label3" runat="server" Text="Reconcilation Id :" CssClass="label"></asp:Label>
                                 </td>
                                  <td style="text-align: left">
                                 <asp:TextBox runat="server" ID="txtRecId" Width="200" ToolTip="Enter Reconcilation ID"></asp:TextBox>
                             </td>
                             <td style="text-align: left">
                                 <asp:Label ID="Label5" runat="server" Text="From Date :" CssClass="label"></asp:Label>
                                 </td>
                                  <td style="text-align: left">
                                 <asp:TextBox ID="txtFromDate" runat="server" autocomplete="off" CssClass="textbox" MaxLength="50" 
                                     ToolTip="Enter From Date" Width="200px" onfocus="showCalendarControl(this);"></asp:TextBox>
                            </td>
                             <td style="text-align: left">
                                 <asp:Label ID="Label6" runat="server" Text="To Date :" CssClass="label"></asp:Label>
                                 </td>
                                  <td style="text-align: left">
                                 <asp:TextBox ID="txtToDate" runat="server" autocomplete="off" CssClass="textbox" MaxLength="50" 
                                     ToolTip="Enter To Date" Width="200px" onfocus="showCalendarControl(this);"></asp:TextBox>
                            </td>
                             </tr>
                        <tr>
                             <td style="text-align: left">
                             <asp:Button ID="btnSearch" runat="server"  Text="Search"  CssClass="button" OnClick="btnSearch_Click"
                                             Width="150px" ToolTip="Search" />  
                                </td>
                             <td style="text-align: left">
                            <asp:Button ID="btnClear" runat="server"  Text="Clear"  CssClass="button"  OnClientClick="ClearFields();"
                                  onclick="btnClear_Click"           Width="150px" ToolTip="Search" />  
                                </td>
                        </tr>
                        <tr>
                            <td style="text-align: left" colspan="6">
                                <table id="tblAssets" runat="server" cellspacing="10" width="100%" 
                                    align="center">
                                    <tr style="border: 2px double #006600;">
                                        <td style="vertical-align: top">
                                            <asp:GridView ID="gvCodes" runat="server" AllowPaging="True" OnPageIndexChanging="gvCodes_PageIndexChanging"
                                                ShowFooter="false" CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                                PageSize="50" >
                                             
                                                <PagerStyle CssClass="pgr"></PagerStyle>
                                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                            </asp:GridView>
                                            <asp:GridView ID="gvCodesFacilities" runat="server" AllowPaging="True" OnPageIndexChanging="gvCodesFacilities_PageIndexChanging"
                                                ShowFooter="false" CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                                PageSize="50" >
                                             
                                                <PagerStyle CssClass="pgr"></PagerStyle>
                                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                            </asp:GridView>
                                        </td>                                   
                                    </tr>                                    
                                    
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <asp:Label ID="lblErrorMsg" Font-Bold="true" CssClass="ErrorLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                                        <td colspan="2" align="left">
                                            <asp:ImageButton ID="btnExport" runat="server"  Enabled="true" ToolTip="Export assets list into excel file"
                                                ImageUrl="~/images/Excel-icon (2).png" CausesValidation="false" 
                                             onclick="btnExport_Click"   />
                                        </td>
                                    </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>


