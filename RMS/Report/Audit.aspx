<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="Audit.aspx.cs" Inherits="RMS.Report.Audit" ValidateRequest="false" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/audit.css" rel="stylesheet" />
    <script type="text/javascript">
        function OpenPopup(_URL) {
            open(_URL, 'Popup', 'toolbar=yes, menubar=no, width=840, height=500, scrollbars=yes');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgAudits">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgAudits" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgAuditByRecord">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgAuditByRecord" LoadingPanelID="ralp" />
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
        <telerik:RadTabStrip runat="server" ID="rts1" Orientation="HorizontalTop" SelectedIndex="0" MultiPageID="rmp1" Skin="Bootstrap">
            <Tabs>
                <telerik:RadTab Text="Audits By Record" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="Audits By Date" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp1" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <p style="font-weight:bold; padding: 0 5px 0 5px;">To view audits for the record, click on the arrow in the far left column of the row.  To view audit details and download documents, click on 
                    the <img src="../images/viewdoc.png" alt="view audit" /> icon under the View column.  Clicking the Edit link in the far right column will take you to the modify audit period page.</p>
                <telerik:RadGrid ID="rgAuditByRecord" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" 
                    GridLines="None" ShowStatusBar="true" PageSize="50"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="true"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgAuditByRecord_NeedDataSource" OnDetailTableDataBind="rgAuditByRecord_DetailTableDataBind"
                    OnItemDataBound="rgAuditByRecord_ItemDataBound"
                    OnPreRender="rgAuditByRecord_PreRender">
                    <PagerStyle Mode="NumericPages" />
                    <MasterTableView DataKeyNames="rms_record_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" 
                        Name="Records" AllowFilteringByColumn="true">
                        <DetailTables>
                            <telerik:GridTableView DataKeyNames="rms_record_id, rms_audit_id" Width="100%" runat="server" CommandItemDisplay="None"
                                Name="Audits" AllowFilteringByColumn="false">
                                <ParentTableRelation>
                                    <telerik:GridRelationFields DetailKeyField="rms_record_id" MasterKeyField="rms_record_id" />
                                </ParentTableRelation>
                                <Columns>
                                    <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn" EditImageUrl="../images/viewdoc.png" HeaderText="View">
                                        <HeaderStyle Width="10px" />
                                        <ItemStyle CssClass="MyImageButton" />
                                    </telerik:GridEditCommandColumn>
                                    <telerik:GridBoundColumn DataField="date_range" UniqueName="date_range" HeaderText="Date Range" SortExpression="audit_beg_dt" HeaderStyle-Width="150px"  />
                                    <telerik:GridBoundColumn DataField="audit_by" HeaderText="Audit By" UniqueName="audit_by" SortExpression="audit_by" HeaderStyle-Width="80px" />
                                    <telerik:GridBoundColumn DataField="audit_type" UniqueName="audit_type" HeaderText="Audit Type" SortExpression="audit_type" HeaderStyle-Width="100px" />
                                    <telerik:GridBoundColumn DataField="audit_results" UniqueName="audit_results" HeaderText="Audit Results" SortExpression="audit_results" HeaderStyle-Width="200px" />
                                    <telerik:GridTemplateColumn HeaderText="Included Analysis Period Notes" UniqueName="analysis_notes_va" AllowSorting="false" HeaderStyle-Width="100px">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hlAnalysisNotes" runat="server" Text="View" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="edit_audit" HeaderText="Edit Audit Period" HeaderStyle-Width="40px">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hlEditAudit" runat="server" Text="Edit" />&nbsp;
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                                <EditFormSettings EditFormType="Template">
                                    <FormTemplate>
                                        <div style="padding:5px;background-color:#f0eac8;">
                                            <table id="tableForm1" cellspacing="5" cellpadding="5" width="100%" border="0" rules="none" style="border-collapse: collapse; background: #f0eac8;">
                                                <tr>
                                                    <td colspan="3">
                                                        <h4>Audit Period Details</h4>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="200" valign="top">
                                                        <asp:Image ID="imgTypeHelp" runat="server" ImageUrl="~/Images/tooltip.png" /> <label>Audit Type:</label>
                                                    </td>
                                                    <td>
                                                        <asp:Literal ID="ltlAuditType" runat="server" />
                                                    </td>
                                                    <td rowspan="6" valign="top">
                                                        <div class="DocBox">
                                                            <h5>Currently Uploaded Documents for this Audit</h5>
                                                            <div style="padding: 10px 0 10px 20px;">
                                                                <telerik:RadListView ID="rlvAuditDocs" runat="server" ItemPlaceholderID="DocsHolder" Skin="Bootstrap">
                                                                    <LayoutTemplate>
                                                                        <asp:Panel ID="DocsHolder" runat="server">
                                                                        </asp:Panel>
                                                                    </LayoutTemplate>
                                                                    <ItemTemplate>
                                                                        <a href='<%# String.Format("{0}Handler/DocHandler.ashx?task=get&ID={1}", System.Configuration.ConfigurationManager.AppSettings["SIMSURL"], Eval("rms_audit_document_id")) %>' target="_blank"><%# Eval("document_nm") %></a>
                                                                    </ItemTemplate>
                                                                    <ItemSeparatorTemplate>
                                                                        <br />
                                                                    </ItemSeparatorTemplate>
                                                                    <EmptyDataTemplate>
                                                                        <i>No audit documents have been uploaded.</i>
                                                                    </EmptyDataTemplate>
                                                                </telerik:RadListView>
                                                            </div>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top">
                                                        <asp:Image ID="imgResultsHelp" runat="server" ImageUrl="~/Images/tooltip.png" /> <label>Audit Results:</label>
                                                    </td>
                                                    <td>
                                                        <asp:Literal ID="ltlAuditResults" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top">
                                                        <asp:Image ID="imgReasonHelp" runat="server" ImageUrl="~/Images/tooltip.png" /> <label>Audit Reason:</label>
                                                    </td>
                                                    <td>
                                                        <asp:Literal ID="ltlReason" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top">
                                                        <asp:Image ID="imgDataHelp" runat="server" ImageUrl="~/Images/tooltip.png" /> <label>Data Audited:</label>
                                                    </td>
                                                    <td>
                                                        <asp:Literal ID="ltlData" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top">
                                                        <asp:Image ID="imgFindingsHelp" runat="server" ImageUrl="~/Images/tooltip.png" /> <label>Audit Findings:</label>
                                                    </td>
                                                    <td>
                                                        <asp:Literal ID="ltlFindings" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" colspan="2">
                                                        <telerik:RadButton ID="rbClose" Text="Close" runat="server" CausesValidation="false" CommandName="Cancel" Skin="Bootstrap" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <telerik:RadToolTip runat="server" ID="rtt3" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                            Height="50px" TargetControlID="imgTypeHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                            The type of audit.  
                                        </telerik:RadToolTip>
                                        <telerik:RadToolTip runat="server" ID="rrt5" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                            Height="50px" TargetControlID="imgResultsHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                            The audit results.
                                        </telerik:RadToolTip>
                                        <telerik:RadToolTip runat="server" ID="rtt6" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                            Height="50px" TargetControlID="imgReasonHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                            The audit reason
                                        </telerik:RadToolTip>
                                        <telerik:RadToolTip runat="server" ID="rtt7" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                            Height="50px" TargetControlID="imgDataHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                            The data audited.
                                        </telerik:RadToolTip>
                                        <telerik:RadToolTip runat="server" ID="rtt8" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                            height="50px" TargetControlID="imgFindingsHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                            The audit findings.
                                        </telerik:RadToolTip>
                                    </FormTemplate>
                                </EditFormSettings>
                            </telerik:GridTableView>
                        </DetailTables>
                        <Columns>
                            <telerik:GridBoundColumn DataField="site_no" HeaderText="Site Number" UniqueName="site_no" SortExpression="site_no" HeaderStyle-Width="150px" FilterControlWidth="100px" />
                            <telerik:GridBoundColumn DataField="station_nm" HeaderText="Station Name" UniqueName="station_nm" HeaderStyle-Width="500px" SortExpression="station_nm" FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="type_ds" UniqueName="type_ds" HeaderText="Record Type" SortExpression="type_ds" FilterControlWidth="200px" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="rpv1">
                <p style="font-weight:bold;padding: 0 5px 0 5px;">To view records included in the audit, click on the arrow in the far left column of the row.  To view audit details and download documents, click on 
                    the <img src="../images/viewdoc.png" alt="view audit" /> icon under the View column.  Clicking the Edit link in the far right column will take you to the modify audit period page.</p>
                <telerik:RadGrid ID="rgAudits" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" 
                    GridLines="None" ShowStatusBar="true" PageSize="50"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="true"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgAudits_NeedDataSource" OnDetailTableDataBind="rgAudits_DetailTableDataBind"
                    OnItemDataBound="rgAudits_ItemDataBound"
                    OnPreRender="rgAudits_PreRender">
                    <PagerStyle Mode="NumericPages" />
                    <MasterTableView DataKeyNames="rms_audit_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" 
                        Name="Audits" AllowFilteringByColumn="true">
                        <DetailTables>
                            <telerik:GridTableView DataKeyNames="rms_audit_record_id" Width="100%" runat="server" CommandItemDisplay="None"
                                Name="Records">
                                <ParentTableRelation>
                                    <telerik:GridRelationFields DetailKeyField="rms_audit_id" MasterKeyField="rms_audit_id" />
                                </ParentTableRelation>
                                <Columns>
                                    <telerik:GridBoundColumn DataField="site_no" HeaderText="Site Number" UniqueName="site_no" SortExpression="site_no" 
                                        AllowFiltering="false" HeaderStyle-Width="150" />
                                    <telerik:GridBoundColumn DataField="station_nm" HeaderText="Station Name" UniqueName="station_nm" HeaderStyle-Width="500" 
                                        SortExpression="station_nm" AllowFiltering="false" />
                                    <telerik:GridBoundColumn DataField="type_ds" UniqueName="type_ds" HeaderText="Record Type" SortExpression="type_ds" AllowFiltering="false" />
                                    <telerik:GridTemplateColumn HeaderText="Included Analysis Period Notes" UniqueName="analysis_notes_va" AllowSorting="false" AllowFiltering="false">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hlAnalysisNotes" runat="server" Text="View" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                                <NoRecordsTemplate>
                                    <div style="background-color:#d7adad;padding:5px;font-weight:bold;">No records were audited.</div>
                                </NoRecordsTemplate>
                            </telerik:GridTableView>
                        </DetailTables>
                        <Columns>
                            <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn" EditImageUrl="../images/viewdoc.png" HeaderText="View">
                                <HeaderStyle Width="10px" />
                                <ItemStyle CssClass="MyImageButton" />
                            </telerik:GridEditCommandColumn>
                            <telerik:GridBoundColumn DataField="date_range" UniqueName="date_range" HeaderText="Date Range" SortExpression="audit_beg_dt" FilterControlWidth="70px" HeaderStyle-Width="100px"  />
                            <telerik:GridBoundColumn DataField="audit_by" HeaderText="Audit By" UniqueName="audit_by" SortExpression="audit_by" FilterControlWidth="60px" HeaderStyle-Width="70px" />
                            <telerik:GridBoundColumn DataField="audit_type" UniqueName="audit_type" HeaderText="Audit Type" SortExpression="audit_type" FilterControlWidth="70px" HeaderStyle-Width="100px" />
                            <telerik:GridBoundColumn DataField="audit_results" UniqueName="audit_results" HeaderText="Audit Results" SortExpression="audit_results" FilterControlWidth="70px" HeaderStyle-Width="100px" />
                            <telerik:GridTemplateColumn UniqueName="edit_audit" HeaderText="Edit Audit Period" HeaderStyle-Width="40px" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlEditAudit" runat="server" Text="Edit" />&nbsp;
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <EditFormSettings EditFormType="Template">
                            <FormTemplate>
                                <div style="padding:5px;background-color:#f0eac8;">
                                    <table id="tableForm1" cellspacing="5" cellpadding="5" width="100%" border="0" rules="none" style="border-collapse: collapse; background: #f0eac8;">
                                        <tr>
                                            <td colspan="3">
                                                <h4>Audit Period Details</h4>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="200" valign="top">
                                                <asp:Image ID="imgTypeHelp" runat="server" ImageUrl="~/Images/tooltip.png" /> <label>Audit Type:</label>
                                            </td>
                                            <td>
                                                <asp:Literal ID="ltlAuditType" runat="server" />
                                            </td>
                                            <td rowspan="6" valign="top">
                                                <div class="DocBox">
                                                    <h5>Currently Uploaded Documents for this Audit</h5>
                                                    <div style="padding: 10px 0 10px 20px;">
                                                        <telerik:RadListView ID="rlvAuditDocs" runat="server" ItemPlaceholderID="DocsHolder" Skin="Bootstrap">
                                                            <LayoutTemplate>
                                                                <asp:Panel ID="DocsHolder" runat="server">
                                                                </asp:Panel>
                                                            </LayoutTemplate>
                                                            <ItemTemplate>
                                                                <a href='<%# String.Format("{0}Handler/DocHandler.ashx?task=get&ID={1}", System.Configuration.ConfigurationManager.AppSettings["SIMSURL"], Eval("rms_audit_document_id")) %>' target="_blank"><%# Eval("document_nm") %></a>
                                                            </ItemTemplate>
                                                            <ItemSeparatorTemplate>
                                                                <br />
                                                            </ItemSeparatorTemplate>
                                                            <EmptyDataTemplate>
                                                                <i>No audit documents have been uploaded.</i>
                                                            </EmptyDataTemplate>
                                                        </telerik:RadListView>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Image ID="imgResultsHelp" runat="server" ImageUrl="~/Images/tooltip.png" /> <label>Audit Results:</label>
                                            </td>
                                            <td>
                                                <asp:Literal ID="ltlAuditResults" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Image ID="imgReasonHelp" runat="server" ImageUrl="~/Images/tooltip.png" /> <label>Audit Reason:</label>
                                            </td>
                                            <td>
                                                <asp:Literal ID="ltlReason" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Image ID="imgDataHelp" runat="server" ImageUrl="~/Images/tooltip.png" /> <label>Data Audited:</label>
                                            </td>
                                            <td>
                                                <asp:Literal ID="ltlData" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Image ID="imgFindingsHelp" runat="server" ImageUrl="~/Images/tooltip.png" /> <label>Audit Findings:</label>
                                            </td>
                                            <td>
                                                <asp:Literal ID="ltlFindings" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2">
                                                <telerik:RadButton ID="rbClose" Text="Close" runat="server" CausesValidation="false" CommandName="Cancel" Skin="Bootstrap" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <telerik:RadToolTip runat="server" ID="rtt3" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                    Height="50px" TargetControlID="imgTypeHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    The type of audit. 
                                </telerik:RadToolTip>
                                <telerik:RadToolTip runat="server" ID="rrt5" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                    Height="50px" TargetControlID="imgResultsHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    The audit results.
                                </telerik:RadToolTip>
                                <telerik:RadToolTip runat="server" ID="rtt6" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                    Height="50px" TargetControlID="imgReasonHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    The audit reason.
                                </telerik:RadToolTip>
                                <telerik:RadToolTip runat="server" ID="rtt7" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                    Height="50px" TargetControlID="imgDataHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    The data audited.
                                </telerik:RadToolTip>
                                <telerik:RadToolTip runat="server" ID="rtt8" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                    height="50px" TargetControlID="imgFindingsHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    The audit findings.
                                </telerik:RadToolTip>
                            </FormTemplate>
                        </EditFormSettings>
                    </MasterTableView>
                </telerik:RadGrid>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
</asp:Content>
