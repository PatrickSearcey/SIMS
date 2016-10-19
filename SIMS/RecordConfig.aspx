<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/RMS.Master" CodeBehind="RecordConfig.aspx.vb" Inherits="SIMS.RecordConfig" %>
<%@ MasterType  virtualPath="~/RMS.master" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="javascript" type="text/javascript">
    function PopupInst() {
        var insturl = "/SIMSClassic/instructions/showinstructions.asp?itype=alt_office"
        open(insturl, "InstPopup", "toolbar=yes, menubar=no, width=840, height=700, resizable=1, scrollbars=yes")

    }
    // validation for DD checkboxlists
    function ValidateDDListWithUnusedDDs(source, args) {
        var chkUsedDDList = document.getElementById('<%= cblUsedDDs.ClientID %>');
        var chkUsedDDListInputs = chkUsedDDList.getElementsByTagName("input");
        var chkUnusedDDList = document.getElementById('<%= cblUnusedDDs.ClientID %>');
        var chkUnusedDDListInputs = chkUnusedDDList.getElementsByTagName("input");
        for (var i = 0; i < chkUsedDDListInputs.length; i++) {
            if (chkUsedDDListInputs[i].checked) {
                args.IsValid = true;
                return;
            } else {
                for (var n = 0; n < chkUnusedDDListInputs.length; n++) {
                    if (chkUnusedDDListInputs[n].checked) {
                        args.IsValid = true;
                        return;
                    }
                }
            }
        }
        args.IsValid = false;
    }
    // validation for DD checkboxlists when the unused DD checkboxlist is not showing
    function ValidateDDList(source, args) {
        var chkUsedDDList = document.getElementById('<%= cblUsedDDs.ClientID %>');
        var chkUsedDDListInputs = chkUsedDDList.getElementsByTagName("input");
        for (var i = 0; i < chkUsedDDListInputs.length; i++) {
            if (chkUsedDDListInputs[i].checked) {
                args.IsValid = true;
                return;
            }
        }
        args.IsValid = false;
    }
    // validation for DD checkboxlists when the used DD checkboxlist is not showing
    function ValidateDDListWithOnlyUnusedDDs(source, args) {
        var chkUnusedDDList = document.getElementById('<%= cblUnusedDDs.ClientID %>');
        var chkUnusedDDListInputs = chkUnusedDDList.getElementsByTagName("input");
        for (var i = 0; i < chkUnusedDDListInputs.length; i++) {
            if (chkUnusedDDListInputs[i].checked) {
                args.IsValid = true;
                return;
            }
        }
        args.IsValid = false;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
<asp:Panel ID="pnlError" runat="server">
    <p class="SITitleFontSmall">In order to use this interface, a record ID must be passed.</p>
</asp:Panel>
<asp:Panel ID="pnlHasAccess" runat="server">
    <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlRecordNotUsed" runat="server" BackColor="#f2eaab" Visible="false">
                <div style="padding-left:10px;padding-right:10px;margin-top:0px;margin-bottom:0px;">
                    This record has been set to <b>inactive</b>.<br />
                    If you wish to reactivate the record, check the box: 
                    <asp:CheckBox ID="cbReactivate" runat="server" OnCheckedChanged="cbReactivate_CheckedChanged" AutoPostBack="true" Width="20px" Height="20px" />
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlContOrNoncont" runat="server" BackColor="#f2eaab" Visible="false">
                <div style="padding-left:10px;padding-right:10px;margin-top:0px;margin-bottom:0px;">
                    <asp:Panel ID="pnlBoth" runat="server">
                        Choose a classification for this new record:<br /><br /> 
                        Time-Series Record, with DD <asp:CheckBox ID="cbCont" runat="server" OnCheckedChanged="cbCont_CheckedChanged" AutoPostBack="true" Width="20px" Height="20px" />
                        <br /><b>&nbsp; OR &nbsp;</b><br />
                        Non-Time-Series Record, with DD <asp:CheckBox ID="cbNonContDD" runat="server" OnCheckedChanged="cbNonContDD_CheckedChanged" AutoPostBack="true" Width="20px" Height="20px" />
                        <br /><b>&nbsp; OR &nbsp;</b><br />
                        Non-Time-Series Record, no DD <asp:CheckBox ID="cbNoncont" runat="server" OnCheckedChanged="cbNoncont_CheckedChanged" AutoPostBack="true" Width="20px" Height="20px" />
                    </asp:Panel>
                    <asp:Panel ID="pnlOnlyNoncont" runat="server">
                        There are no available DDs registered on NWISWeb for this site, so the record must be non-times-series. If this is correct, please check the box below to continue creating a non-time-series record.<br /><br />
                        Non-Time-Series Record, no DD <asp:CheckBox ID="cbOnlyNoncont" runat="server" OnCheckedChanged="cbOnlyNoncont_CheckedChanged" AutoPostBack="true" Width="20px" Height="20px" /><br /><br />
                        <b>Note:</b> If you are trying to create a times-series record but do not see the DD, you must register the daily value DD in nw_edit, and manually push the data to NWISWeb. Your registered DD will be available for creating a record soon.
                    </asp:Panel>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlUpdateConfirm" runat="server" BackColor="#f2eaab" Visible="false">
                <div style="padding-left:10px;padding-right:10px;margin-top:0px;margin-bottom:0px;">
                    You have successfully submitted updates for the <asp:Label ID="lblRecordTypeAU" runat="server" /> record.<br />
                    Return to the <asp:HyperLink ID="hlStationInfo" runat="server" Text="Station Information page for this site" />, 
                    or choose to configure another record from the list below.<br /><br />
                    <asp:GridView ID="gvOtherRecordsAU" runat="server" AutoGenerateColumns="False" 
                        BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" 
                        CellPadding="4" ForeColor="Black" GridLines="Vertical">
                        <RowStyle BackColor="#F7F7DE" />
                        <Columns>
                            <asp:BoundField DataField="type_ds" HeaderText="Record-Type" />
                            <asp:BoundField DataField="operator_va" HeaderText="Operator" />
                            <asp:BoundField DataField="assigned_checker_uid" HeaderText="Checker" />
                            <asp:BoundField DataField="assigned_reviewer_uid" HeaderText="Reviewer" />
                            <asp:BoundField DataField="rec_status" HeaderText="Status" />
                            <asp:TemplateField ShowHeader="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbSelectRecord" runat="server" CausesValidation="False" 
                                        CommandName='<%# DataBinder.Eval(Container.DataItem,"rms_record_id") %>' 
                                        Text="Configure" OnCommand="ResetForm"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <FooterStyle BackColor="#CCCC99" />
                        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                </div>    
            </asp:Panel>
            <asp:Panel ID="pnlRecordConfigForm" runat="server" BackColor="#f2eaab" Width="850px">
                <div style="padding-left:10px;padding-right:10px;margin-top:0px;margin-bottom:0px;">
                    <p class="SITitleFontSmall" style="margin-top:0px;">Parameters tied to <asp:Label ID="lblRecordType" runat="server" /> at the site:</p>
                    <table width="100%" cellspacing="0" cellpadding="0">
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblContRecord" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td nowrap width="20%"><asp:Label ID="lblDDHelp" runat="server" Visible="false"  /> <asp:Label ID="lblDD" runat="server" Text="ADAPS Data Descriptor" Visible="false" />
                            </td>
                            <td style="text-align:left;"><asp:CheckBoxList ID="cblUsedDDs" runat="server" RepeatDirection="Horizontal" Visible="false" />
                            <asp:CheckBox ID="cbMultiDDs" runat="server" Visible="false" Width="300" Height="20" 
                                AutoPostBack="true" OnCheckedChanged="cbMultiDDs_CheckedChanged"  />
                            <asp:CheckBoxList ID="cblUnusedDDs" runat="server" RepeatDirection="Horizontal" Visible="false" />
                            <asp:CustomValidator runat="server" ID="cvDDList" 
                                ErrorMessage="You must select at least one DD" ></asp:CustomValidator> </td>
                        </tr>
                        <tr>
                            <td nowrap><asp:Label ID="lblRecordTypeHelp" runat="server" /> <asp:Label ID="lblRecordTypeLabel" runat="server" Text="Record-Type" />
                            </td>
                            <td style="text-align:left;"><asp:DropDownList ID="ddlRecordType" runat="server" />
                            <asp:RequiredFieldValidator ID="rfvRecordType" runat="server" 
                                Text="You must choose a record-type" ControlToValidate="ddlRecordType"/></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblCategoryHelp" runat="server" Visible="false"  /> <asp:Label ID="lblCategoryNo" runat="server" Text="Category Number" Visible="false"  />
                            </td>
                            <td style="text-align:left;">
                                <asp:DropDownList ID="ddlCategoryNo" runat="server" Visible="false" AutoPostBack="true" OnSelectedIndexChanged="ddlCategoryNo_SelectedIndexChanged"  />
                                <asp:RequiredFieldValidator ID="rfvCategoryNo" runat="server" 
                                    Text="You must enter a category number" ControlToValidate="ddlCategoryNo"/>
                                <asp:Panel ID="pnlCatReason" runat="server" Visible="false">
                                    <label><asp:Label ID="lblCategoryReason" runat="server" /></label>
                                    <label><asp:Label ID="lblUpdateReason" runat="server" /></label><br />
                                    <asp:DropDownList ID="ddlUpdateReason" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUpdateReason_SelectedIndexChanged" />
                                    <asp:RequiredFieldValidator ID="rfvUpdateReason" runat="server"
                                        Text="You must select a reason" ControlToValidate="ddlUpdateReason" />
                                    <label><asp:Label ID="lblUpdateOtherReason" runat="server" Text="<br />Please specify: " Visible="false" /></label>
                                    <asp:TextBox ID="tbUpdateOtherReason" runat="server" Visible="false" Width="200" />
                                </asp:Panel>
                                <asp:Panel ID="pnlCatReasonNew" runat="server" Visible="false">
                                    <label><asp:Label ID="lblNewReason" runat="server" Text="You must select a reason for choosing this category number:" /></label><br />
                                    <asp:DropDownList ID="ddlNewReason" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlNewReason_SelectedIndexChanged" />
                                    <asp:RequiredFieldValidator ID="rfvNewReason" runat="server"
                                        Text="You must select a reason" ControlToValidate="ddlNewReason" />
                                    <label><asp:Label ID="lblNewOtherReason" runat="server" Text="<br />Please specify: " Visible="false" /></label>
                                    <asp:TextBox ID="tbNewOtherReason" runat="server" Visible="false" Width="200" />
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <table width="700px" cellspacing="0" cellpadding="0">
                        <tr>
                            <td width="15%">Operator</td>
                            <td style="text-align:left;"><asp:DropDownList ID="ddlOperator" runat="server" /></td>
                            <td rowspan="3">
                                <div style="padding-left:10px;">
                                    <asp:Panel ID="pnlFieldTrips" runat="server" BackColor="#e1dac7">
                                        <p style="padding-left:10px;padding-right:10px;margin-top:0px;margin-bottom:0px;">
                                        The site currently belongs to the following field trips:</p>
                                        <asp:BulletedList ID="blFieldTrips" runat="server" BulletStyle="Square" CssClass="fieldtriplist" />
                                    </asp:Panel>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>Checker</td>
                            <td style="text-align:left;"><asp:DropDownList ID="ddlChecker" runat="server" /></td>
                        </tr>
                        <tr>
                            <td>Reviewer</td>
                            <td style="text-align:left;"><asp:DropDownList ID="ddlReviewer" runat="server" /></td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:Label ID="lblReviewerEmail" runat="server" Font-Size="X-Small" ForeColor="DarkRed" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">Responsible Office
                            <asp:Label ID="lblResponsibleOffice" runat="server" Font-Bold="true"  />
                            &laquo; <asp:LinkButton ID="lbResponsibleOffice" runat="server" Font-Size="X-Small" 
                                Text="read more about this" OnClientClick="PopupInst()" /></td>
                        </tr>
                    </table>
                    <hr />
                    <table width="800px" cellspacing="0" cellpadding="0">
                        <tr>
                            <td><asp:CheckBox ID="cbNotPublished" runat="server" Width="20" Height="20" /> 
                                Not Published (record shows up in work, check, review but not in End Of Year Summary)</td>
                        </tr>
                        <tr>
                            <td><asp:CheckBox ID="cbNotUsed" runat="server" Width="20" Height="20" />
                                Record Inactive (record only shows up when editing records through admin pages)</td>
                        </tr>
                    </table>
                    <hr />
                    <table width="100%" cellspacing="0" cellpadding="0" style="background-color:#f1f2e1">
                        <tr>
                            <td style="text-align:center;">
                                <asp:Button ID="btnSubmit" runat="server" OnCommand="btnSubmit_Command"  />
                                <asp:Button ID="btnReset" runat="server" CausesValidation="false" />
                            </td>
                        </tr>
                    </table>
                    <asp:LinkButton ID="lbOtherRecords" runat="server" CausesValidation="false"  
                        Text="<br />View other records tied to this site:" OnClick="Expand_pnlOtherRecords" 
                        BorderStyle="None" Font-Bold="True" Font-Size="105%" ForeColor="#858855" />
                    <asp:Image ID="imgOtherRecords" runat="server" ImageUrl="images/expand.jpg" />
                    <br /><br />
                    <asp:Panel ID="pnlOtherRecords" runat="server">
                        <asp:GridView ID="gvOtherRecords" runat="server" AutoGenerateColumns="False" 
                            BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" 
                            CellPadding="4" ForeColor="Black" GridLines="Vertical">
                            <RowStyle BackColor="#F7F7DE" />
                            <Columns>
                                <asp:BoundField DataField="type_ds" HeaderText="Record-Type" />
                                <asp:BoundField DataField="operator_va" HeaderText="Operator" />
                                <asp:BoundField DataField="assigned_checker_uid" HeaderText="Checker" />
                                <asp:BoundField DataField="assigned_reviewer_uid" HeaderText="Reviewer" />
                                <asp:BoundField DataField="rec_status" HeaderText="Status" />
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbSelectRecord" runat="server" CausesValidation="False" 
                                            CommandName='<%# DataBinder.Eval(Container.DataItem,"rms_record_id") %>' 
                                            Text="Configure" OnCommand="RecordSelected"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                        <asp:Label ID="lblTest" runat="server" />
                    </asp:Panel>
                </div>
            </asp:Panel>
            
            <asp:HiddenField ID="hfRMSSiteID" runat="server" />
            <asp:HiddenField ID="hfTSType" runat="server" />
            <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server"
                TargetControlID="pnlOtherRecords"
                CollapsedSize="0"
                ExpandedSize="100"
                Collapsed="True"
                ExpandControlID="lbOtherRecords"
                CollapseControlID="lbOtherRecords"
                AutoCollapse="False"
                AutoExpand="False"
                ScrollContents="false"
                ImageControlID="imgOtherRecords"
                ExpandedImage="images/collapse.jpg"
                CollapsedImage="images/expand.jpg"
                ExpandDirection="Vertical" />
            <ajaxToolkit:RoundedCornersExtender ID="rce0" runat="server" 
                TargetControlID="pnlRecordNotUsed" 
                BorderColor="#993300" 
                Enabled="True" Radius="15" />
            <ajaxToolkit:RoundedCornersExtender ID="rce1" runat="server" 
                TargetControlID="pnlRecordConfigForm" 
                BorderColor="#993300" 
                Enabled="True" Radius="15" />
            <ajaxToolkit:RoundedCornersExtender ID="rce2" runat="server" 
                TargetControlID="pnlFieldTrips" BorderColor="#cabaab"
                Enabled="true" radius="10" />
            <ajaxToolkit:RoundedCornersExtender ID="rec3" runat="server" 
                TargetControlID="pnlUpdateConfirm" 
                BorderColor="#993300" 
                Enabled="True" Radius="15" />
            <ajaxToolkit:RoundedCornersExtender ID="rec4" runat="server" 
                TargetControlID="pnlContOrNoncont" 
                BorderColor="#993300" 
                Enabled="True" Radius="15" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
</asp:Content>
