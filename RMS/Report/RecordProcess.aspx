<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="RecordProcess.aspx.cs" Inherits="RMS.Report.RecordProcess" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/recordprocess.css" rel="stylesheet" />
    <script type="text/javascript">
        function OpenPopup(_URL) {
            open(_URL, 'Popup', 'toolbar=yes, menubar=no, width=840, height=500, scrollbars=yes');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgAnalyzeMyRecords">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgAnalyzeMyRecords" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgAnalyzeRecords">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgAnalyzeRecords" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgApproveMyRecords">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgApproveMyRecords" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgApproveRecords">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgApproveRecords" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgAuditsDue">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgAuditsDue" LoadingPanelID="ralp" />
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
                <telerik:RadTab Text="Records Ready For Analyzing" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="Records Ready for Approving" SelectedCssClass="selectedTab" />
                <telerik:RadTab Text="Audits Due" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp1" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <p style="font-weight:bold; padding: 0 5px 0 5px;">
                    <img src="../images/lock.png" alt="lock" /> Record is currently locked
                    <img src="../images/save_icon.gif" alt="save" /> Updates pending 
                    &nbsp;&nbsp;&nbsp;&nbsp;Rows with light yellow backgrounds are currently in a status of "Reanalyze"<br />
                    Click on the Record-Type to analyze the record. Click on the station number to navigate to the Station Information page. 
                </p>
                <h4>&nbsp;&nbsp;&nbsp;Records Assigned To Me</h4>
                <telerik:RadGrid ID="rgAnalyzeMyRecords" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" 
                    GridLines="None" ShowStatusBar="true" PageSize="50"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="false"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgAnalyzeMyRecords_NeedDataSource"
                    OnItemDataBound="rgAnalyzeMyRecords_ItemDataBound"
                    OnPreRender="rgAnalyzeMyRecords_PreRender">
                    <MasterTableView DataKeyNames="rms_record_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" Name="Records" AllowFilteringByColumn="true">
                        <Columns>
                            <telerik:GridTemplateColumn AllowFiltering="false" AllowSorting="false" UniqueName="LockIcon" HeaderText="Lck" HeaderStyle-Width="20px">
                                <ItemTemplate>
                                    <asp:Image ID="imgLockIcon" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="site_no" HeaderText="Site Number" FilterControlWidth="90px">
                                <ItemTemplate>
                                    <b><a href='<%# String.Format("{0}StationInfo.aspx?site_id={1}", Eval("SIMSURL"), Eval("site_id")) %>'><%# Eval("site_no") %></a></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="station_nm" HeaderText="Station Name" UniqueName="station_nm" HeaderStyle-Width="500px" SortExpression="station_nm" FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="category_no" HeaderText="Cat." UniqueName="category_no" SortExpression="category_no" AllowFiltering="false" HeaderStyle-Width="20px" />
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="type_ds" HeaderText="Type Cd" FilterControlWidth="60px">
                                <ItemTemplate>
                                    <b><asp:HyperLink ID="hlRecordType" runat="server" Target="_blank"><%# Eval("type_cd") %></asp:HyperLink></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="analyzer_uid" HeaderText="Assigned To" UniqueName="analyzer_uid" FilterControlWidth="80px" SortExpression="analyzer_uid" />
                            <telerik:GridBoundColumn DataField="LastAnalyzedDate" UniqueName="LastAnalyzedDate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Analyzed Through" SortExpression="LastAnalyzedDate" AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="DaysSinceAging" UniqueName="DaysSinceAging" HeaderText="Days Since Last Approved" SortExpression="DaysSinceAging" 
                                AllowFiltering="false" HeaderStyle-Width="140px" />
                            <telerik:GridBoundColumn DataField="lock_type" UniqueName="lock_type" Display="false" />
                            <telerik:GridBoundColumn DataField="lock_dt" UniqueName="lock_dt" Display="false" />
                            <telerik:GridBoundColumn DataField="lock_uid" UniqueName="lock_uid" Display="false" />
                            <telerik:GridBoundColumn DataField="reanalyze_status" UniqueName="reanalyze_status" Display="false" />
                        </Columns>
                    </MasterTableView>
                    <ClientSettings EnableAlternatingItems="false"></ClientSettings>
                </telerik:RadGrid>
                <h4>&nbsp;&nbsp;&nbsp;All Other Records</h4>
                <telerik:RadGrid ID="rgAnalyzeRecords" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" 
                    GridLines="None" ShowStatusBar="true" PageSize="50"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="false"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgAnalyzeRecords_NeedDataSource"
                    OnItemDataBound="rgAnalyzeRecords_ItemDataBound"
                    OnPreRender="rgAnalyzeRecords_PreRender">
                    <MasterTableView DataKeyNames="rms_record_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" Name="Records" AllowFilteringByColumn="true">
                        <Columns>
                            <telerik:GridTemplateColumn AllowFiltering="false" AllowSorting="false" UniqueName="LockIcon" HeaderText="Lck" HeaderStyle-Width="20px">
                                <ItemTemplate>
                                    <asp:Image ID="imgLockIcon" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="site_no" HeaderText="Site Number" FilterControlWidth="90px">
                                <ItemTemplate>
                                    <b><a href='<%# String.Format("{0}StationInfo.aspx?site_id={1}", Eval("SIMSURL"), Eval("site_id")) %>'><%# Eval("site_no") %></a></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="station_nm" HeaderText="Station Name" UniqueName="station_nm" HeaderStyle-Width="500px" SortExpression="station_nm" FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="category_no" HeaderText="Cat." UniqueName="category_no" SortExpression="category_no" AllowFiltering="false" HeaderStyle-Width="20px" />
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="type_ds" HeaderText="Type Cd" FilterControlWidth="60px">
                                <ItemTemplate>
                                    <b><asp:HyperLink ID="hlRecordType" runat="server" Target="_blank"><%# Eval("type_cd") %></asp:HyperLink></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="analyzer_uid" HeaderText="Assigned To" UniqueName="analyzer_uid" FilterControlWidth="80px" SortExpression="analyzer_uid" />
                            <telerik:GridBoundColumn DataField="LastAnalyzedDate" UniqueName="LastAnalyzedDate" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Analyzed Through" SortExpression="LastAnalyzedDate" AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="DaysSinceAging" UniqueName="DaysSinceAging" HeaderText="Days Since Last Approved in AQ" SortExpression="DaysSinceAging" 
                                AllowFiltering="false" HeaderStyle-Width="140px" />
                            <telerik:GridBoundColumn DataField="lock_type" UniqueName="lock_type" Display="false" />
                            <telerik:GridBoundColumn DataField="lock_dt" UniqueName="lock_dt" Display="false" />
                            <telerik:GridBoundColumn DataField="lock_uid" UniqueName="lock_uid" Display="false" />
                            <telerik:GridBoundColumn DataField="reanalyze_status" UniqueName="reanalyze_status" Display="false" />
                        </Columns>
                    </MasterTableView>
                    <ClientSettings EnableAlternatingItems="false"></ClientSettings>
                </telerik:RadGrid>
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="rpv1">
                <p style="font-weight:bold; padding: 0 5px 0 5px;">
                    <img src="../images/lock.png" alt="lock" /> Record is currently locked
                    <img src="../images/save_icon.gif" alt="save" /> Updates pending <br />
                    Click on the Record-Type to approve the record. Click on the station number to navigate to the Station Information page. 
                </p>
                <h4>&nbsp;&nbsp;&nbsp;Records Assigned To Me</h4>
                <telerik:RadGrid ID="rgApproveMyRecords" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" 
                    GridLines="None" ShowStatusBar="true" PageSize="50"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="false"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgApproveMyRecords_NeedDataSource"
                    OnItemDataBound="rgApproveMyRecords_ItemDataBound"
                    OnPreRender="rgApproveMyRecords_PreRender">
                    <MasterTableView DataKeyNames="rms_record_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" 
                        Name="Records" AllowFilteringByColumn="true">
                        <Columns>
                            <telerik:GridTemplateColumn AllowFiltering="false" AllowSorting="false" UniqueName="LockIcon" HeaderText="Lck" HeaderStyle-Width="20px">
                                <ItemTemplate>
                                    <asp:Image ID="imgLockIcon" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="site_no" HeaderText="Site Number" FilterControlWidth="90px">
                                <ItemTemplate>
                                    <b><a href='<%# String.Format("{0}StationInfo.aspx?site_id={1}", Eval("SIMSURL"), Eval("site_id")) %>'><%# Eval("site_no") %></a></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="station_nm" HeaderText="Station Name" UniqueName="station_nm" HeaderStyle-Width="500px" SortExpression="station_nm" 
                                FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="category_no" HeaderText="Cat." UniqueName="category_no" SortExpression="category_no" AllowFiltering="false" HeaderStyle-Width="20px" />
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="type_ds" HeaderText="Type Cd" FilterControlWidth="60px">
                                <ItemTemplate>
                                    <b><asp:HyperLink ID="hlRecordType" runat="server" Target="_blank"><%# Eval("type_cd") %></asp:HyperLink></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="approver_uid" HeaderText="Assigned To" UniqueName="approver_uid" FilterControlWidth="80px" SortExpression="approver_uid" />
                            <telerik:GridBoundColumn DataField="approved_by" HeaderText="Prev. Appr. By" UniqueName="approved_by" SortExpression="approved_by" HeaderStyle-Width="40px" 
                                AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="period" AllowSorting="true" SortExpression="period" HeaderText="Period" AllowFiltering="false" HeaderStyle-Width="180px" />
                            <telerik:GridBoundColumn DataField="DaysSinceAging" UniqueName="DaysSinceAging" HeaderText="Days Since Last Approved in AQ" SortExpression="DaysSinceAging" 
                                AllowFiltering="false" HeaderStyle-Width="140px" />
                            <telerik:GridBoundColumn DataField="lock_type" UniqueName="lock_type" Display="false" />
                            <telerik:GridBoundColumn DataField="lock_dt" UniqueName="lock_dt" Display="false" />
                            <telerik:GridBoundColumn DataField="lock_uid" UniqueName="lock_uid" Display="false" />
                            <telerik:GridBoundColumn DataField="period_id" UniqueName="period_id" Display="false" />
                        </Columns>
                    </MasterTableView>
                    <ClientSettings EnableAlternatingItems="false"></ClientSettings>
                </telerik:RadGrid>
                <h4>&nbsp;&nbsp;&nbsp;All Other Records</h4>
                <telerik:RadGrid ID="rgApproveRecords" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" 
                    GridLines="None" ShowStatusBar="true" PageSize="50"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="false"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgApproveRecords_NeedDataSource"
                    OnItemDataBound="rgApproveRecords_ItemDataBound"
                    OnPreRender="rgApproveRecords_PreRender">
                    <MasterTableView DataKeyNames="rms_record_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" 
                        Name="Records" AllowFilteringByColumn="true">
                        <Columns>
                            <telerik:GridTemplateColumn AllowFiltering="false" AllowSorting="false" UniqueName="LockIcon" HeaderText="Lck" HeaderStyle-Width="20px">
                                <ItemTemplate>
                                    <asp:Image ID="imgLockIcon" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="site_no" HeaderText="Site Number" FilterControlWidth="90px">
                                <ItemTemplate>
                                    <b><a href='<%# String.Format("{0}StationInfo.aspx?site_id={1}", Eval("SIMSURL"), Eval("site_id")) %>'><%# Eval("site_no") %></a></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="station_nm" HeaderText="Station Name" UniqueName="station_nm" HeaderStyle-Width="500px" SortExpression="station_nm" 
                                FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="category_no" HeaderText="Cat." UniqueName="category_no" SortExpression="category_no" AllowFiltering="false" HeaderStyle-Width="20px" />
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="type_ds" HeaderText="Type Cd" FilterControlWidth="60px">
                                <ItemTemplate>
                                    <b><asp:HyperLink ID="hlRecordType" runat="server" Target="_blank"><%# Eval("type_cd") %></asp:HyperLink></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="approver_uid" HeaderText="Assigned To" UniqueName="approver_uid" FilterControlWidth="80px" SortExpression="approver_uid" />
                            <telerik:GridBoundColumn DataField="approved_by" HeaderText="Prev. Appr. By" UniqueName="approved_by" SortExpression="approved_by" HeaderStyle-Width="40px"  
                                AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="period" AllowSorting="true" SortExpression="period" HeaderText="Period" AllowFiltering="false" HeaderStyle-Width="180px" />
                            <telerik:GridBoundColumn DataField="DaysSinceAging" UniqueName="DaysSinceAging" HeaderText="Days Since Last Approved in AQ" SortExpression="DaysSinceAging" 
                                AllowFiltering="false" HeaderStyle-Width="140px" />
                            <telerik:GridBoundColumn DataField="lock_type" UniqueName="lock_type" Display="false" />
                            <telerik:GridBoundColumn DataField="lock_dt" UniqueName="lock_dt" Display="false" />
                            <telerik:GridBoundColumn DataField="lock_uid" UniqueName="lock_uid" Display="false" />
                            <telerik:GridBoundColumn DataField="period_id" UniqueName="period_id" Display="false" />
                        </Columns>
                    </MasterTableView>
                    <ClientSettings EnableAlternatingItems="false"></ClientSettings>
                </telerik:RadGrid>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpv2">
                <p style="font-weight:bold; padding: 0 5px 0 5px;">Click the column headings to sort by that column.  To view audit details and download documents, click on 
                    the Last Audit Period link.  Records that have been audited within the past 9 months show Months Since Last Audit in <span style="color:#196F3D;">dark green font</span>. Those audited between 9 - 12 
                    months ago are in <span style="color:#7DCEA6;">light green font</span>, those between 12 - 15 months are in <span style="color:#CA6F1E;">orange font</span>, and greater than 15 months in <span style="color:#E33813;">red font</span>.
                </p>
                <telerik:RadGrid ID="rgAuditsDue" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" 
                    GridLines="None" ShowStatusBar="true" PageSize="50"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="true"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgAuditsDue_NeedDataSource"
                    OnItemDataBound="rgAuditsDue_ItemDataBound"
                    OnPreRender="rgAuditsDue_PreRender">
                    <PagerStyle Mode="NumericPages" />
                    <MasterTableView DataKeyNames="rms_record_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" 
                        Name="Records" AllowFilteringByColumn="true">
                        <Columns>
                            <telerik:GridBoundColumn DataField="office_cd" HeaderText="Office" UniqueName="office_cd" SortExpression="office_cd" HeaderStyle-Width="90px" FilterControlWidth="70px" />
                            <telerik:GridBoundColumn DataField="site_no" HeaderText="Site Number" UniqueName="site_no" SortExpression="site_no" HeaderStyle-Width="150px" FilterControlWidth="100px" />
                            <telerik:GridBoundColumn DataField="station_nm" HeaderText="Station Name" UniqueName="station_nm" HeaderStyle-Width="500px" SortExpression="station_nm" FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="type_cd" UniqueName="type_cd" HeaderText="Record Type" SortExpression="type_cd" FilterControlWidth="100px" />
                            <telerik:GridBoundColumn DataField="auditor_uid" UniqueName="auditor_uid" HeaderText="Assigned Auditor" SortExpression="auditor_uid" FilterControlWidth="100px" />
                            <telerik:GridBoundColumn DataField="last_approved_period" UniqueName="last_approved_period" HeaderText="Last Approved Period" SortExpression="approved_dt" AllowFiltering="false" />
                            <telerik:GridTemplateColumn DataField="audit_end_dt" UniqueName="last_audit_period" HeaderText="Last Audit Period" SortExpression="audit_end_dt" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlAuditPeriod" runat="server"></asp:HyperLink>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="months_since_last" UniqueName="months_since_last" HeaderText="Months Since Last Audit" SortExpression="months_since_last" FilterControlWidth="60px">
                                <ItemTemplate>
                                    <asp:Literal ID="ltlMonthsSinceLast" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
</asp:Content>
