<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="RecordProcess.aspx.cs" Inherits="RMS.Report.RecordProcess" %>
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
                    <img src="../images/save_icon.png" alt="save" /> Updates pending <br />
                    Click on the Record-Type to analyze the record. Click on the station number to navigate to the Station Information page. 
                </p>
                <telerik:RadGrid ID="rgAnalyzeRecords" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" 
                    GridLines="None" ShowStatusBar="true" PageSize="50"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="true"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgAnalyzeRecords_NeedDataSource" OnDetailTableDataBind="rgAnalyzeRecords_DetailTableDataBind"
                    OnItemDataBound="rgAnalyzeRecords_ItemDataBound"
                    OnPreRender="rgAnalyzeRecords_PreRender">
                    <PagerStyle Mode="NumericPages" />
                    <MasterTableView DataKeyNames="rms_record_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" 
                        Name="Records" AllowFilteringByColumn="true">
                        <Columns>
                            <telerik:GridBoundColumn DataField="site_no" HeaderText="Site Number" UniqueName="site_no" SortExpression="site_no" HeaderStyle-Width="150px" FilterControlWidth="100px" />
                            <telerik:GridBoundColumn DataField="station_nm" HeaderText="Staiton Name" UniqueName="station_nm" HeaderStyle-Width="500px" SortExpression="station_nm" FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="type_ds" UniqueName="type_ds" HeaderText="Record Type" SortExpression="type_ds" FilterControlWidth="200px" />
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
                    AllowPaging="true"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgApproveRecords_NeedDataSource" OnDetailTableDataBind="rgApproveRecords_DetailTableDataBind"
                    OnItemDataBound="rgApproveRecords_ItemDataBound"
                    OnPreRender="rgApproveRecords_PreRender">
                    <PagerStyle Mode="NumericPages" />
                    <MasterTableView DataKeyNames="rms_audit_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" 
                        Name="Audits" AllowFilteringByColumn="true">
                        <Columns>
                            <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn" EditImageUrl="images/viewdoc.png" HeaderText="View">
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
                    </MasterTableView>
                </telerik:RadGrid>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
</asp:Content>
