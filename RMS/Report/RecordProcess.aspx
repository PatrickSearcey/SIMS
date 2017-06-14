﻿<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="RecordProcess.aspx.cs" Inherits="RMS.Report.RecordProcess" %>
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
                <telerik:RadTab Text="Records Ready For Analyzing" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="Records Ready for Approving" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp1" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <p style="font-weight:bold; padding: 0 5px 0 5px;">
                    <img src="../images/lock.png" alt="lock" /> Record is currently locked
                    <img src="../images/save_icon.gif" alt="save" /> Updates pending <br />
                    Click on the Record-Type to analyze the record. Click on the station number to navigate to the Station Information page. 
                </p>
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
                            <telerik:GridTemplateColumn AllowFiltering="false" AllowSorting="false" UniqueName="LockIcon" HeaderText="Lck">
                                <ItemTemplate>
                                    <asp:Image ID="imgLockIcon" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="site_no" HeaderText="Site Number" FilterControlWidth="90px">
                                <ItemTemplate>
                                    <b><a href='<%# String.Format("{0}StationInfo.aspx?site_id={1}", Eval("SIMS2017URL"), Eval("site_id")) %>'><%# Eval("site_no") %></a></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="station_nm" HeaderText="Staiton Name" UniqueName="station_nm" HeaderStyle-Width="500px" SortExpression="station_nm" FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="category_no" HeaderText="Category" UniqueName="category_no" SortExpression="category_no" AllowFiltering="false" />
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="type_ds" HeaderText="Record-Type" FilterControlWidth="90px">
                                <ItemTemplate>
                                    <b><asp:HyperLink ID="hlRecordType" runat="server" Target="_blank"><%# Eval("type_ds") %></asp:HyperLink></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="analyzer_uid" HeaderText="Assigned To" UniqueName="analyzer_uid" FilterControlWidth="90px" SortExpression="analyzer_uid" />
                            <telerik:GridTemplateColumn AllowSorting="true" SortExpression="status_va" HeaderText="Reanalyze Status" AllowFiltering="false">
                                <ItemTemplate>
                                    <%# Eval("reanalyze_status") %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="analyzed_dt" UniqueName="analyzed_dt" HeaderText="Analyzed Through" SortExpression="analzyed_dt" AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="aq_approved_dt" UniqueName="aq_approved_dt" HeaderText="Days Since Last Approved in Aquarius" SortExpression="aq_approved_dt" 
                                AllowFiltering="false" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="rpv1">
                <p style="font-weight:bold; padding: 0 5px 0 5px;">
                    <img src="../images/lock.png" alt="lock" /> Record is currently locked
                    <img src="../images/save_icon.png" alt="save" /> Updates pending <br />
                    Click on the Record-Type to approve the record. Click on the station number to navigate to the Station Information page. 
                </p>
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
                            <telerik:GridTemplateColumn AllowFiltering="false" AllowSorting="false" UniqueName="LockIcon" HeaderText="Lck">
                                <ItemTemplate>
                                    <asp:Image ID="imgLockIcon" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="site_no" HeaderText="Site Number" FilterControlWidth="90px">
                                <ItemTemplate>
                                    <b><a href='<%# String.Format("{0}StationInfo.aspx?site_id={1}", Eval("SIMS2017URL"), Eval("site_id")) %>'><%# Eval("site_no") %></a></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="station_nm" HeaderText="Staiton Name" UniqueName="station_nm" HeaderStyle-Width="400px" SortExpression="station_nm" 
                                FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="category_no" HeaderText="Category" UniqueName="category_no" SortExpression="category_no" AllowFiltering="false" />
                            <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="type_ds" HeaderText="Record-Type" FilterControlWidth="90px">
                                <ItemTemplate>
                                    <b><asp:HyperLink ID="hlRecordType" runat="server" Target="_blank"><%# Eval("type_ds") %></asp:HyperLink></b>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="approver_uid" HeaderText="Assigned To" UniqueName="approver_uid" FilterControlWidth="90px" SortExpression="approver_uid" />
                            <telerik:GridBoundColumn DataField="prev_approved_by" HeaderText="Previously Approved By" UniqueName="prev_approved_by" SortExpression="prev_approved_by" 
                                AllowFiltering="false" />
                            <telerik:GridTemplateColumn AllowSorting="true" SortExpression="period" HeaderText="Period" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:Literal ID="ltlPeriod" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="aq_approved_dt" UniqueName="aq_approved_dt" HeaderText="Days Since Last Approved in Aquarius" SortExpression="aq_approved_dt" 
                                AllowFiltering="false" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
</asp:Content>
