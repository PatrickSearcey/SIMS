<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="AuditPeriod.aspx.cs" Inherits="RMS.AuditPeriod" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/audit.css" rel="stylesheet" />
    <script type="text/javascript">
    </script>
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
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriod" LoadingPanelID="ralp" />
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
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriod" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbCreateAudit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlNotice" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAuditPeriod" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSetupAuditPeriod" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">

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
                <p class="AuditList">1. Enter the date range of the new audit period:</p>
                <div class="AuditList">
                    <telerik:RadDatePicker ID="rdpBeginDt" runat="server" Skin="Bootstrap" /> - to - <telerik:RadDatePicker ID="rdpEndDt" runat="server" Skin="Bootstrap" />
                </div>
                <p class="AuditList">2. Select at least one record from the list below for which to audit:</p>
                <div class="Records">
                    <div class="RecordList">
                        <telerik:RadListBox RenderMode="Lightweight" ID="rlbRecords" runat="server" CheckBoxes="true" ShowCheckAll="true" Width="400px" Height="200px" 
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
                        <telerik:RadButton ID="rbSubmitRecords" runat="server" AutoPostBack="true" Text="Audit these records" Skin="Bootstrap" OnCommand="rbSubmitRecords_Command" />
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
                        Height="300px" SelectionMode="Single" Width="300px" AutoPostBack="true" />
                    <telerik:RadTextBox ID="rtbSANAL" runat="server" Height="300px" Width="700px" ReadOnly="true" Skin="Bootstrap" />
                </div>
                <h4>Audit the Period</h4>
                <div class="AuditList">
                    <table cellpadding="5" width="100%">
                        <tr>
                            <td><b>Type of Audit:</b></td>
                            <td><telerik:RadDropDownList ID="rddlAuditType" runat="server" Skin="Bootstrap" DataTextField="description" DataValueField="audit_type_id" /></td>
                        </tr>
                        <tr>
                            <td><b>Audit Results:</b></td>
                            <td><telerik:RadDropDownList ID="rddlAuditResults" runat="server" Skin="Bootstrap" DataTextField="description" DataValueField="audit_result_id" /></td>
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
                    </table>
                </div>
                <telerik:RadButton ID="rbCreateAudit" runat="server" Skin="Bootstrap" Text="Create Audit" OnCommand="CreateAudit" AutoPostBack="true" />
                <telerik:RadButton ID="rbStartOver" runat="server" Skin="Bootstrap" Text="Start Over" OnCommand="StartOver" AutoPostBack="true" />
            </asp:Panel>

        </asp:Panel>

    </div>
</asp:Content>
