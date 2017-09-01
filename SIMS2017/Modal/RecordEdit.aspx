<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecordEdit.aspx.cs" Inherits="SIMS2017.Modal.RecordEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Record</title>
    <link href="../styles/recordedit.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="rsm2" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadFormDecorator ID="rfd2" runat="server" DecoratedControls="all" Skin="Bootstrap" RenderMode="Lightweight" />
    <telerik:RadStyleSheetManager ID="rssm2" runat="server" CdnSettings-TelerikCdn="Enabled" />
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function CloseModal() {
            //create the argument that will be returned to the parent page
            var oArg = new Object();
            oArg.type = "record";
            //get a reference to the current RadWindow
            var oWnd = GetRadWindow();
            //Close the RadWindow and send the argument to the parent page
            oWnd.close(oArg);
        }
    </script>
    <div>
        <telerik:RadAjaxPanel ID="rap" runat="server">
            <h2><asp:Literal ID="ltlTitle" runat="server" /></h2>
            <hr />
            <h3><asp:Literal ID="ltlRecordType" runat="server" /><br /><asp:Literal ID="ltlSite" runat="server" /></h3>
            <asp:Panel ID="pnlNewRecord" runat="server" CssClass="RecordPanel">
                <p><asp:Literal ID="ltlTopNote" runat="server" /></p>
                <telerik:RadCheckBoxList ID="rcblOptions" runat="server" OnSelectedIndexChanged="CreateNewRecord" Direction="Horizontal"
                    DataBindings-DataValueField="option" DataBindings-DataTextField="description" AutoPostBack="true" />
                <p><asp:Literal ID="ltlBottomNote" runat="server" /></p>
            </asp:Panel>
            <asp:Panel ID="pnlRecordNotUsed" runat="server" Visible="false" CssClass="RecordPanel">
                This record has been set to <b>inactive</b>.<br />
                If you wish to reactivate the record, check the box: 
                <asp:CheckBox ID="cbReactivate" runat="server" OnCheckedChanged="cbReactivate_CheckedChanged" AutoPostBack="true" Width="20px" Height="20px" />
            </asp:Panel>
            <asp:Panel ID="pnlEditRecord" runat="server" CssClass="RecordPanel">
                <table width="95%">
                    <tr>
                        <td colspan="2"><p><asp:Literal ID="ltlRecordTS" runat="server" /></p></td>
                    </tr>
                    <tr>
                        <td><asp:Literal ID="ltlTimeSeriesLabel" runat="server" Text="Time-Series ID" /></td>
                        <td>
                            <asp:HiddenField ID="hfEditIDs" runat="server" />
                            <asp:Panel ID="pnlAssignedIDs" runat="server">
                                <asp:Literal ID="ltlAssignedIDs" runat="server" /> <asp:LinkButton ID="lbEditIDs" runat="server" Text="edit" OnCommand="EditIDs" />
                            </asp:Panel>
                            <asp:Panel ID="pnlEditIDs" runat="server">
                                <telerik:RadCheckBoxList ID="rcblIDs" runat="server" DataBindings-DataValueField="iv_ts_id" DataBindings-DataTextField="dd_ts_ds" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td width="30%">Record-Type</td>
                        <td><telerik:RadDropDownList ID="rddlRecordTypes" runat="server" DataValueField="record_type_id" DataTextField="type_ds" Width="300px" /></td>
                    </tr>
                    <tr>
                        <td><asp:Literal ID="ltlCatNumberLabel" runat="server" Text="Category Number" /></td>
                        <td>
                            <telerik:RadDropDownList ID="rddlCatNumber" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CategoryNumberChanged" Width="80px">
                                <Items>
                                    <telerik:DropDownListItem Value="1" Text="1" />
                                    <telerik:DropDownListItem Value="2" Text="2" />
                                    <telerik:DropDownListItem Value="3" Text="3" />
                                </Items>
                            </telerik:RadDropDownList>
                            <telerik:RadTextBox ID="rtbCatReason" runat="server" Width="300px" />
                        </td>
                    </tr>
                    <tr>
                        <td>Operator <telerik:RadDropDownList ID="rddlOperator" runat="server" Width="120px" DataValueField="user_id" DataTextField="user_id" /></td>
                        <td>
                            Analyst <telerik:RadDropDownList ID="rddlAnalyzer" runat="server" Width="120px" DataValueField="user_id" DataTextField="user_id" />
                            Approver <telerik:RadDropDownList ID="rddlApprover" runat="server" Width="120px" DataValueField="user_id" DataTextField="user_id"  />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="SubPanel">
                                <p>Upon approving this record, an email will also be sent to the following address(es): <asp:Literal ID="ltlApproverEmail" runat="server" /></p>
                                <p>This site currently belongs to the following field trips: <asp:Literal ID="ltlFieldTrips" runat="server" /></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>Responsible Office</td>
                        <td>
                            <telerik:RadDropDownList ID="rddlResponsibleOffice" runat="server" DataValueField="office_id" DataTextField="office_nm"  Width="350px" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <telerik:RadCheckBox ID="rcbRecordInactive" runat="server" Text="Record Inactive (record only shows up when editing records through admin pages)" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <telerik:RadButton ID="rbSubmit" runat="server" Text="Submit Changes" OnClick="EditRecord" SingleClick="true" SingleClickText="Submitting..." />
                            <telerik:RadButton ID="rbCancel" runat="server" Text="Cancel" OnClick="Cancel" />
                            <span class="error"><asp:Literal ID="ltlError" runat="server" /></span>
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="pnlOtherRecords" runat="server" CssClass="SubPanel">
                    <h4>Other Records For This Site</h4>
                    <asp:DataGrid ID="dgOtherRecords" runat="server" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundColumn DataField="type_ds" HeaderText="Record-Type" ItemStyle-Width="200px" />
                            <asp:BoundColumn DataField="operator_uid" HeaderText="Operator" ItemStyle-Width="100px" />
                            <asp:BoundColumn DataField="analyzer_uid" HeaderText="Analyst" ItemStyle-Width="100px" />
                            <asp:BoundColumn DataField="approver_uid" HeaderText="Approver" ItemStyle-Width="100px" />
                            <asp:BoundColumn DataField="status" HeaderText="Status" ItemStyle-Width="80px" />
                        </Columns>
                        <HeaderStyle Font-Bold="true" CssClass="Header" />
                    </asp:DataGrid>
                    <br />
                </asp:Panel>
            </asp:Panel>
        </telerik:RadAjaxPanel>
    </div>
    </form>
</body>
</html>
