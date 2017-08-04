<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="PubStatus.aspx.cs" Inherits="RMS.Report.PubStatus" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/pubstatus.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgTSStatus">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgTSStatus" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgNTSStatus">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgNTSStatus" LoadingPanelID="ralp" />
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
                <telerik:RadTab Text="Time-Series Record Status" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="Non-Time-Series Record Status" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp1" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <p style="font-weight:bold; padding: 0 5px 0 5px;"></p>
                <telerik:RadGrid ID="rgTSStatus" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" RenderMode="Lightweight"
                    GridLines="None" ShowStatusBar="true"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="false"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgTSStatus_NeedDataSource"
                    OnItemDataBound="rgTSStatus_ItemDataBound"
                    OnPreRender="rgTSStatus_PreRender">
                    <MasterTableView DataKeyNames="rms_record_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                        <Columns>
                            <telerik:GridBoundColumn DataField="office_cd" HeaderText="Office" UniqueName="office_cd" SortExpression="office_cd" HeaderStyle-Width="60px" FilterControlWidth="40px" />
                            <telerik:GridBoundColumn DataField="site_no" HeaderText="Site Number" UniqueName="site_no" SortExpression="site_no" HeaderStyle-Width="150px" FilterControlWidth="100px" />
                            <telerik:GridBoundColumn DataField="station_full_nm" HeaderText="Station Name" UniqueName="station_full_nm" HeaderStyle-Width="500px" SortExpression="station_nm" FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="parm_cd" UniqueName="parm_cd" HeaderText="Parm Cd" SortExpression="parm_cd" FilterControlWidth="50px" />
                            <telerik:GridBoundColumn DataField="type_cd" UniqueName="type_cd" HeaderText="Record Type" SortExpression="type_cd" FilterControlWidth="100px" />
                            <telerik:GridBoundColumn DataField="last_aging_dt" UniqueName="last_aging_dt" HeaderText="Last Approved DV" DataFormatString="{0:MM/dd/yyyy}" SortExpression="last_aging_dt" FilterControlWidth="80px" />
                            <telerik:GridBoundColumn DataField="Last_peak_entered" UniqueName="Last_peak_entered" HeaderText="Last Peak Entered" DataFormatString="{0:MM/dd/yyyy}" SortExpression="Last_peak_entered" FilterControlWidth="80px" />
                            <telerik:GridBoundColumn DataField="Last_manu_approved" UniqueName="Last_manu_approved" HeaderText="MANU Approved Date" DataFormatString="{0:MM/dd/yyyy}" SortExpression="Last_manu_approved" FilterControlWidth="80px" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="rpv1">
                <p style="font-weight:bold;padding: 0 5px 0 5px;"></p>
                <telerik:RadGrid ID="rgNTSStatus" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" RenderMode="Lightweight" 
                    GridLines="None" ShowStatusBar="true"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="false"
                    AllowAutomaticDeletes="true" OnNeedDataSource="rgNTSStatus_NeedDataSource"
                    OnItemDataBound="rgNTSStatus_ItemDataBound"
                    OnPreRender="rgNTSStatus_PreRender">
                    <MasterTableView DataKeyNames="rms_record_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                        <Columns>
                            <telerik:GridBoundColumn DataField="office_cd" HeaderText="Office" UniqueName="office_cd" SortExpression="office_cd" HeaderStyle-Width="60px" FilterControlWidth="40px" />
                            <telerik:GridBoundColumn DataField="site_no" HeaderText="Site Number" UniqueName="site_no" SortExpression="site_no" HeaderStyle-Width="150px" FilterControlWidth="100px" />
                            <telerik:GridBoundColumn DataField="station_full_nm" HeaderText="Station Name" UniqueName="station_full_nm" HeaderStyle-Width="500px" SortExpression="station_nm" FilterControlWidth="200px"/>
                            <telerik:GridBoundColumn DataField="parm_cd" UniqueName="parm_cd" HeaderText="Parm Cd" SortExpression="parm_cd" FilterControlWidth="50px" />
                            <telerik:GridBoundColumn DataField="type_cd" UniqueName="type_cd" HeaderText="Record Type" SortExpression="type_cd" FilterControlWidth="100px" />
                            <telerik:GridBoundColumn DataField="last_aging_dt" UniqueName="last_aging_dt" HeaderText="Last Approved DV" DataFormatString="{0:MM/dd/yyyy}" SortExpression="last_aging_dt" FilterControlWidth="80px" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
</asp:Content>
