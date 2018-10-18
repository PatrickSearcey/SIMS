<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecordEdit.aspx.cs" Inherits="SIMS2017.Modal.RecordEdit" ValidateRequest="false" %>

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
                <telerik:RadCheckBoxList ID="rcblOptions" runat="server" OnSelectedIndexChanged="CreateNewRecord" Direction="Horizontal" Skin="Bootstrap"
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
                        <td><asp:Literal ID="ltlTimeSeriesLabel" runat="server"><b>Time-Series ID</b></asp:Literal></td>
                        <td>
                            <asp:HiddenField ID="hfEditIDs" runat="server" />
                            <asp:Panel ID="pnlAssignedIDs" runat="server">
                                <asp:Literal ID="ltlAssignedIDs" runat="server" /> <asp:LinkButton ID="lbEditIDs" runat="server" Text="edit" OnCommand="EditIDs" />
                            </asp:Panel>
                            <asp:Panel ID="pnlEditIDs" runat="server">
                                <telerik:RadCheckBoxList ID="rcblIDs" runat="server" DataBindings-DataValueField="iv_ts_id" DataBindings-DataTextField="dd_ts_ds" Skin="Bootstrap" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td width="30%"><b>Record-Type</b></td>
                        <td><telerik:RadDropDownList ID="rddlRecordTypes" runat="server" DataValueField="record_type_id" DataTextField="type_ds" Width="300px" Skin="Bootstrap" /></td>
                    </tr>
                    <tr>
                        <td><asp:Literal ID="ltlCatNumberLabel" runat="server"><b>Category Number</b></asp:Literal></td>
                        <td>
                            <telerik:RadDropDownList ID="rddlCatNumber" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CategoryNumberChanged" Width="80px" Skin="Bootstrap">
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
                        <td><b>Operator</b> <telerik:RadDropDownList ID="rddlOperator" runat="server" Width="120px" DataValueField="user_id" DataTextField="user_id" Skin="Bootstrap" /></td>
                        <td>
                            <b>Analyst</b> <telerik:RadDropDownList ID="rddlAnalyzer" runat="server" Width="120px" DataValueField="user_id" DataTextField="user_id" Skin="Bootstrap" />
                            <b>Approver</b> <telerik:RadDropDownList ID="rddlApprover" runat="server" Width="120px" DataValueField="user_id" DataTextField="user_id" Skin="Bootstrap"  />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <b>Auditor</b> <telerik:RadDropDownList ID="rddlAuditor" runat="server" Width="120px" DataValueField="user_id" DataTextField="user_id" Skin="Bootstrap" />
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
                        <td><b>Responsible Office</b></td>
                        <td>
                            <telerik:RadDropDownList ID="rddlResponsibleOffice" runat="server" DataValueField="office_id" DataTextField="office_nm"  Width="350px" Skin="Bootstrap" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <telerik:RadCheckBox ID="rcbThreatenedGage" runat="server" Text="Record Endangered (show/don't show on endangered gage website)" AutoPostBack="true" Skin="Bootstrap" 
                                OnCheckedChanged="rcbThreatenedGage_CheckedChanged" />
                            <asp:Panel ID="pnlThreatenedGage" runat="server">
                                <div class="SubPanel">
                                    <p><b>Enter display comment</b><br /><span style="font-style:italic;font-size:9pt;">Include the date to be discontinued (or was discontinued or rescued), 
                                        as well as the reason for the change in status. Please note, all remarks will be visible to the public.</span></p>
                                    <asp:TextBox ID="tbRemarks" runat="server" TextMode="MultiLine" Height="50px" Width="557px" />
                                    <table width="100%">
                                        <tr>
                                            <td><b>Enter years of record</b></td>
                                            <td><telerik:RadNumericTextBox ID="rntbYearsOfRecord" runat="server" Width="50px" NumberFormat-DecimalDigits="0" Skin="Bootstrap" /></td>
                                            <td><b>Choose status</b></td>
                                            <td>
                                                <telerik:RadComboBox ID="rcbStatus" runat="server" Skin="Bootstrap">
                                                    <Items>
                                                        <telerik:RadComboBoxItem Value="" Text="" />
                                                        <telerik:RadComboBoxItem Value="Threatened" Text="Endangered" />
                                                        <telerik:RadComboBoxItem Value="Rescued" Text="Rescued" />
                                                        <telerik:RadComboBoxItem Value="Discontinued" Text="Discontinued" />
                                                    </Items>
                                                </telerik:RadComboBox>
                                                <asp:Image runat="server" ID="imgToolTip_Status" ImageUrl="~/images/QuestionMark.png" AlternateText="question mark" Width="15px" Height="15px" />
                                                <telerik:RadToolTip RenderMode="Lightweight" runat="server" ID="rttStatus" TargetControlID="imgToolTip_Status" IsClientID="false"
                                                    ShowEvent="OnMouseOver" HideEvent="Default" Position="MiddleRight" RelativeTo="Element" AutoCloseDelay="9000"
                                                    Width="300px" Height="150px" Skin="Bootstrap" IgnoreAltAttribute="true">
                                                    <ul>
                                                        <li>Endangered - Station may be discontinued or converted to a reduced level of service.</li>
                                                        <li>Discontinued - Recently discontinued stations due to funding shortfall</li>
                                                        <li>Rescued - Stations temporarily discontinued or converted to a reduced level of service, now back in full operation</li>
                                                    </ul>
                                                </telerik:RadToolTip>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3"><b>If Rescued/Discontinued - Enter sunset date of this message</b></td>
                                            <td><telerik:RadDatePicker ID="rdpSunsetDt" runat="server" /></td>
                                        </tr>
                                    </table>
                                </div>
                            </asp:Panel>
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
