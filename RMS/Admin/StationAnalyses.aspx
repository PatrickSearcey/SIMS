<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="StationAnalyses.aspx.cs" Inherits="RMS.Admin.StationAnalyses" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/admin.css" rel="stylesheet" />
    <script type="text/javascript">
        function OnClientLoad(editor, args) {
            editor.setSize("1000px", "400px");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rddlRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rgSANAL" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rddlRecords">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgSANAL" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgSANAL">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgSANAL" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <br />
    <div class="mainContent">
        <asp:Panel ID="pnlNoAccess" runat="server">
            <h4>You do not have the necessary permission to access Administration Tasks for this WSC. Please contact your WSC SIMS Admin if you require access.</h4>
        </asp:Panel>

        <asp:Panel ID="pnlHasAccess" runat="server">
            <asp:Panel ID="pnlNotice" runat="server" CssClass="pnlNotes">
                <h4><asp:Literal ID="ltlNoticeHeading" runat="server" /></h4>
                <asp:Literal ID="ltlNotice" runat="server" />
            </asp:Panel>

            <div class="Filters">
                <p>
                    Enter a site number and agency code to search for available records.  Then select the record to modify the analysis periods.
                </p>
                <table width="700">
                    <tr>
                        <td><telerik:RadTextBox ID="rtbSiteNo" runat="server" Skin="Bootstrap" Width="200px" /></td>
                        <td><telerik:RadTextBox ID="rtbAgencyCd" runat="server" Skin="Bootstrap" Width="80px" Text="USGS" /></td>
                        <td><telerik:RadButton ID="rbSubmit" runat="server" OnCommand="UpdateRecordList" CommandArgument="Update" Text="Find Records" AutoPostBack="true" Skin="Bootstrap" /></td>
                        <td><asp:Literal ID="ltlError" runat="server" /></td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <telerik:RadDropDownList ID="rddlRecords" runat="server" DataValueField="rms_record_id" DataTextField="type_ds" Skin="Bootstrap" Width="400px" 
                                DropDownHeight="100px" OnSelectedIndexChanged="UpdateSANALGrid" AutoPostBack="true" />
                         </td>
                    </tr>
                </table>
                <p>NOTE: This page currently does not allow the changing of period status, or deleting/merging of periods. 
                    Please contact <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W Help SIMS</a> for these changes.</p>
            </div>

            <h3><asp:Literal ID="ltlResultsHeading" runat="server" /></h3>
            <telerik:RadGrid ID="rgSANAL" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                Skin="Bootstrap" GridLines="None" ShowStatusBar="true" 
                AllowSorting="true" 
                AllowMultiRowSelection="false" 
                AllowFiltering="false"
                AllowPaging="false"
                OnNeedDataSource="rgSANAL_NeedDataSource"
                OnItemDataBound="rgSANAL_ItemDataBound"
                OnUpdateCommand="rgSANAL_UpdateCommand">
                <MasterTableView DataKeyNames="period_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" 
                    Name="Periods" AllowFilteringByColumn="false">
                    <Columns>
                        <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn1" HeaderText="Edit">
                            <HeaderStyle Width="20px" />
                            <ItemStyle CssClass="MyImageButton" />
                        </telerik:GridEditCommandColumn>
                        <telerik:GridBoundColumn DataField="period_beg_dt" UniqueName="period_beg_dt" HeaderText="Begin Date" DataFormatString="{0:MM/dd/yyyy}"  />
                        <telerik:GridBoundColumn DataField="period_end_dt" UniqueName="period_end_dt" HeaderText="End Date" DataFormatString="{0:MM/dd/yyyy}" />
                        <telerik:GridBoundColumn DataField="status_va" UniqueName="status_va" HeaderText="Status" />
                    </Columns>

                    <EditFormSettings EditFormType="Template">
                        <FormTemplate>
                            <div style="padding:5px;background-color:#cfe3db;">
                                <table id="tableForm1" cellspacing="5" cellpadding="5" width="600" border="0" rules="none" style="border-collapse: collapse; background: #cfe3db;">
                                    <tr>
                                        <td colspan="2">
                                            <h4>Edit the Analysis Period Details</h4>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:120px;">
                                            <label>Period ID:</label>
                                        </td>
                                        <td>
                                            <%# Eval("period_id") %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Station:</label>
                                        </td>
                                        <td>
                                            <%# Eval("site_no") %> <%# Eval("station_full_nm") %> (<%# Eval("type_ds") %>)
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Begin Date:</label> <%# String.Format("{0:MM/dd/yyyy}", Eval("period_beg_dt")) %>
                                        </td>
                                        <td>
                                            <label>End Date:</label> <%# String.Format("{0:MM/dd/yyyy}", Eval("period_end_dt")) %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Status:</label> <%# Eval("status_va") %>
                                        </td>
                                        <td>
                                            <label>Role of person setting status:</label> <%# Eval("status_set_by_role_va") %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Analyzed By:</label>
                                            <telerik:RadDropDownList ID="rddlAnalyzedBy" runat="server" DataValueField="user_id" DataTextField="user_id" Skin="Bootstrap" />
                                        </td>
                                        <td>
                                            <label>Analyzed Date:</label>
                                            <telerik:RadDatePicker ID="rdpAnalyzedDt" runat="server" Skin="Bootstrap" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Approved By:</label>
                                            <telerik:RadDropDownList ID="rddlApprovedBy" runat="server" DataValueField="user_id" DataTextField="user_id" Skin="Bootstrap" />
                                        </td>
                                        <td>
                                            <label>Approved Date:</label>
                                            <telerik:RadDatePicker ID="rdpApprovedDt" runat="server" Skin="Bootstrap" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <label>Analysis Notes:</label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <telerik:RadEditor ID="reSANAL" runat="server" Skin="Bootstrap" OnClientLoad="OnClientLoad" Width="100%" Height="300px">
                                                <Tools>
                                                    <telerik:EditorToolGroup>
                                                    <telerik:EditorTool Name="InsertLink" Text="Insert Link Dialog" />
                                                    <telerik:EditorTool Name="Bold" Text="Bold" Visible="true" /> 
                                                    </telerik:EditorToolGroup>
                                                </Tools>
                                            </telerik:RadEditor>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="2">
                                            <telerik:RadButton ID="btnUpdate" Text="Update" runat="server" CommandName="Update" />
                                            <telerik:RadButton ID="btnCancel" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </FormTemplate>
                    </EditFormSettings>
                    <EditItemStyle BackColor="#cfe3db" />
                </MasterTableView>
            </telerik:RadGrid>
        </asp:Panel>
    </div>
</asp:Content>
