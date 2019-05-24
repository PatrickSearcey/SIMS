<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="Audit.aspx.cs" Inherits="RMS.Task.Audit" ValidateRequest="false" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/audit.css" rel="stylesheet" />
    <script type="text/javascript">
    </script>
    <style type="text/css">
        #DivReadOnlyEditor .reToolbarWrapper
        {
            display:none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rcbOffice">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rcbFieldTrip" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rlbRecords" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rcbFieldTrip">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rlbRecords" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSubmitRecords">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAuditPeriod" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriodForMultiples" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNotice" />
                    <telerik:AjaxUpdatedControl ControlID="hfAuditType" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rlbViewRecords">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rtbSANAL" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbStartOver">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAuditPeriod" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriodForMultiples" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriod" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNotice" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbDone">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlUploadDocs" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriodForMultiples" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriod" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbCreateEditAudit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlUploadDocs" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlError" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNotice" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAuditPeriod" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ppnlSetupAuditPeriodForMultiples" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlUploadDocs" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbReturnToSingle">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriodForMultiples" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriod" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbMultiple">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriod" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriodForMultiples" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSubmitRecord">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAuditPeriod" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriod" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNotice" />
                    <telerik:AjaxUpdatedControl ControlID="hfAuditType" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />

    <div class="topLinks">
        <b>Links of interest:</b> <a href="https://water.usgs.gov/osw/RevisionsGuidance.html" target="_blank">WMA Revisions Policy</a>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <asp:HiddenField ID="hfAuditType" runat="server" />

        <asp:Panel ID="pnlNoAccess" runat="server">
            <h3>NO ACCESS</h3>
            <p>You do not have the necessary permission to create audit periods for this WSC.</p>
            <p>Please contact your WSC-level admin if you believe you received this notice in error.</p>
        </asp:Panel>

        <asp:Panel ID="pnlNotice" runat="server" CssClass="pnlNotice" Visible="false">
            <h4>NOTICE</h4>
            <p><asp:Literal ID="ltlNotice" runat="server" /></p>
        </asp:Panel>

        <asp:Panel ID="pnlHasAccess" runat="server">

            <asp:Panel ID="pnlSetupAuditPeriod" runat="server">
                <p class="AuditList">Select the approved periods for this record to audit, <b>OR</b> enter the date range of the audit period:</p>
                <div class="Records">
                    <div class="RecordList">
                        <telerik:RadListBox ID="rlbRecordPeriods" runat="server" CheckBoxes="true" ShowCheckAll="true" Width="400px" Height="200px" 
                            Skin="Bootstrap" DataValueField="period_id" DataTextField="PeriodDates" />
                    </div>
                    <div class="DateRange">
                        <h5>OR enter a begin and end date:</h5>
                        <telerik:RadDatePicker ID="rdpBeginDt1" runat="server" Skin="Bootstrap" DateInput-EmptyMessage="begin date" /> - to - <telerik:RadDatePicker ID="rdpEndDt1" runat="server" Skin="Bootstrap" DateInput-EmptyMessage="end date" />
                    </div> 
                    <div class="RecordSubmit">
                        <br /><br /><br /><br />
                        <telerik:RadButton ID="rbSubmitRecord" runat="server" AutoPostBack="true" Text="Continue with audit" Skin="Bootstrap" OnCommand="rbSubmitRecords_Command" CommandArgument="single" />
                        <p><b><img src="../images/arrowbullet.png" alt="arrow" style="float:left;padding: 2px 3px 0 0;" /> Click <asp:LinkButton ID="lbMultiple" runat="server" OnCommand="lbMultiple_Command" Text="here" /> to select multiple records to audit.</b></p>
                    </div>
                </div>

            </asp:Panel>

            <asp:Panel ID="pnlSetupAuditPeriodForMultiples" runat="server">
                <p class="AuditList">1. Enter the date range of the audit period:</p>
                <div class="AuditList">
                    <telerik:RadDatePicker ID="rdpBeginDt2" runat="server" Skin="Bootstrap" DateInput-EmptyMessage="begin date" /> - to - <telerik:RadDatePicker ID="rdpEndDt2" runat="server" Skin="Bootstrap" DateInput-EmptyMessage="end date" />
                </div>
                <p class="AuditList">2. Select multiple records from the list below for which to audit:</p>
                <div class="Records">
                    <div class="RecordList">
                        <telerik:RadListBox ID="rlbRecords" runat="server" CheckBoxes="true" ShowCheckAll="true" Width="400px" Height="200px" 
                            Skin="Bootstrap" DataValueField="rms_record_id" DataTextField="record_nm" />
                    </div>
                    <div class="RecordFilters">
                        <h5>Filter the Record List:</h5>
                        <b>By Office</b><br />
                        <telerik:RadDropDownList RenderMode="Lightweight" ID="rddlOffice" runat="server" Skin="Bootstrap" Width="300px" AutoPostBack="true" 
                            OnSelectedIndexChanged="FilterRecordList" DataValueField="office_id" DataTextField="office_nm" /><br /><br />
                        <b>By Field Trip</b><br />
                        <telerik:RadDropDownList RenderMode="Lightweight" ID="rddlFieldTrip" runat="server" Skin="Bootstrap" Width="300px" AutoPostBack="true" 
                            OnSelectedIndexChanged="FilterRecordList" DataValueField="trip_id" DataTextField="trip_nm" />
                    </div> 
                    <div class="RecordSubmit">
                        <telerik:RadButton ID="rbSubmitRecords" runat="server" AutoPostBack="true" Text="Audit these records" Skin="Bootstrap" OnCommand="rbSubmitRecords_Command" CommandArgument="multiple" />
                        <telerik:RadButton ID="rbReturnToSingle" runat="server" AutoPostBack="true" Text="Return to audit single record" Skin="Bootstrap" OnCommand="rbReturnToSingle_Command" />
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlAuditPeriod" runat="server">
                <h4>Audit Period Details</h4>
                <div class="AuditList">
                    <b>Date Range:</b> <asp:Literal ID="ltlAuditDateRange" runat="server" /><br />
                    <b>Auditor:</b> <asp:Literal ID="ltlAuditBy" runat="server" /><br />
                    <b>Records Included in Audit:</b><br />
                    <p>Select a record from the list to view the station analyses that relate to the audit date range.</p>
                    <telerik:RadListBox RenderMode="Lightweight" ID="rlbViewRecords" runat="server" Skin="Bootstrap" OnSelectedIndexChanged="rlbViewRecords_SelectedIndexChanged" 
                        Height="80px" SelectionMode="Single" Width="1000px" AutoPostBack="true" DataValueField="rms_record_id" DataTextField="record_nm" /><br />
                    <div id="DivReadOnlyEditor">
                        <telerik:RadEditor ID="reSANAL" runat="server" Height="300px" Width="1000px" EditModes="Preview" Skin="Bootstrap">
                            <Tools>
                                <telerik:EditorToolGroup />
                            </Tools>
                        </telerik:RadEditor>
                    </div>
                </div>
                <h4>Audit the Period</h4>
                <div class="AuditList">
                    <table cellpadding="5" width="100%">
                        <tr>
                            <td width="200px"><b>Type of Audit:</b></td>
                            <td><telerik:RadDropDownList ID="rddlAuditType" runat="server" Skin="Bootstrap" DataTextField="description" DataValueField="audit_type_id" Width="600px" /></td>
                        </tr>
                        <tr>
                            <td><b>Audit Results:</b></td>
                            <td><telerik:RadDropDownList ID="rddlAuditResults" runat="server" Skin="Bootstrap" DataTextField="description" DataValueField="audit_results_id" Width="600px" /></td>
                        </tr>
                        <tr>
                            <td><b>Reason for Audit:</b></td>
                            <td><telerik:RadTextBox ID="rtbAuditReason" runat="server" Skin="Bootstrap" Width="600px" Height="200px" TextMode="MultiLine" /></td>
                        </tr>
                        <tr>
                            <td><b>Data Audited:</b></td>
                            <td><telerik:RadTextBox ID="rtbDataAudited" runat="server" Skin="Bootstrap" Width="600px" Height="200px" TextMode="MultiLine" /></td>
                        </tr>
                        <tr>
                            <td><b>Description of Audit Findings:</b></td>
                            <td><telerik:RadTextBox ID="rtbAuditFindings" runat="server" Skin="Bootstrap" Width="600px" Height="200px" TextMode="MultiLine" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <b>After clicking the Create/Edit Audit button, you will be given the option to upload related documents for this audit.</b>
                            </td>
                        </tr>
                    </table>
                </div>
                <hr />
                <telerik:RadButton ID="rbCreateEditAudit" runat="server" Skin="Bootstrap" OnCommand="CreateEditAudit" AutoPostBack="true" />
                <telerik:RadButton ID="rbStartOver" runat="server" Skin="Bootstrap" Text="Start Over" OnCommand="StartOver" AutoPostBack="true" />
                
                <asp:Panel ID="pnlError" runat="server" CssClass="pnlNotice" Visible="false">
                    <h4>Error</h4>
                    <p><asp:Literal ID="ltlError" runat="server" /></p>
                </asp:Panel>
            </asp:Panel>

            <asp:Panel ID="pnlUploadDocs" runat="server">
                <div class="pnlNotice">
                    <h4><asp:Literal ID="ltlConfirm" runat="server" /></h4>
                    <p>Use the form below to upload documents pertaining to this audit.  File types accepted are .TXT, .PDF, .XLSX, .DOCX, .GIF, .JPG, and .PNG.  
                        If you wish to view all audit periods for your WSC, visit the <a href="../Report/Audit.aspx">Audit Report</a>.</p>
                    <p style="font-weight:bold;"><asp:Literal ID="ltlDone" runat="server" /></p>
                </div>
                <br />
                <table width="1000" class="DocBoxes" cellpadding="10">
                    <tr>
                        <td valign="top" width="500">
                            <h5>Upload Related Documents</h5>
                            <table>
                                <tr>
                                    <td align="right" nowrap><b>Select Document</b></td>
                                    <td valign="bottom">
                                        <telerik:RadAsyncUpload runat="server" ID="rauAuditDoc" TemporaryFolder="~/Doc/Temp/" AllowedFileExtensions="pdf,docx,xlsx,txt,gif,jpg,png" 
                                            MaxFileInputsCount="1" MaxFileSize="10000000" DisableChunkUpload="true" MultipleFileSelection="Disabled" Skin="Bootstrap" 
                                            PostbackTriggers="rbSubmit" Localization-Select="Browse" ToolTip="File types accepted are .TXT, .PDF, .XLSX, .DOCX, .GIF, .JPG, and .PNG" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right"><b>Document Title</b></td>
                                    <td><telerik:RadTextBox runat="server" ID="rtbName" Skin="Bootstrap" Width="300px" /></td>
                                </tr>
                                <tr>
                                    <td align="right" valign="top"><b>Description</b></td>
                                    <td><telerik:RadTextBox runat="server" ID="rtbDescription" TextMode="MultiLine" Skin="Bootstrap" Width="300px" Height="50px" /></td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td><telerik:RadButton ID="rbSubmit" runat="server" OnCommand="UploadDocument" CommandName="UploadDoc" Text="Upload Document" Skin="Bootstrap" />
                                        <telerik:RadButton ID="rbDone" runat="server" OnCommand="StartOver" AutoPostBack="true" Text="Done" Skin="Bootstrap" /><br />
                                        <asp:Literal ID="ltlAlert" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top" width="500">
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
                </table>
            </asp:Panel>
        </asp:Panel>

    </div>
</asp:Content>
