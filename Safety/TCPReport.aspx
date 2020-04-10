<%@ Page Title="" Language="C#" MasterPageFile="~/SafetySingleMenu.Master" AutoEventWireup="true" CodeBehind="TCPReport.aspx.cs" Inherits="Safety.TCPReport" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/reports.css" rel="stylesheet" />
    <style type="text/css">
        .RadWindow_Bootstrap {
            z-index: 8000 !important;
        }
        .TCPStatusPanel {
            border:1px solid #e8d0ad; 
            background:#fcfaf6; 
            padding:5px;
            margin-top:10px;
        }
        .imgPadding {
            padding-left:5px;
        }
    </style>
    <script type="text/javascript">
        function openWin(_id, _type) {
            var oWnd = radopen("Modal/TCP.aspx?TCPID=" + _id + "&type=" + _type, "rwTCPReview");
        }

        function OnClientClose(oWnd, args) {
            //get the transferred arguments
            $find("<%= ram.ClientID %>").ajaxRequest("RebindGrids");
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server" OnAjaxRequest="ram_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ram">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgReview" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rgApprove" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rgStatus" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlTCPStatus" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgNWStatus">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgNWStatus" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgNWReview">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgNWReview" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgNWApprove">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgNWApprove" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgNWActiveSitesNoTCP">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgNWActiveSitesNoTCP" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgStatus">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgStatus" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgReview">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgReview" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgApprove">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgApprove" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlTCPStatus" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgActiveSitesNoTCP">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgActiveSitesNoTCP" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbActiveSiteToggleNW1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbActiveSiteToggleNW1" />
                    <telerik:AjaxUpdatedControl ControlID="ltlActiveSiteToggleNW1" />
                    <telerik:AjaxUpdatedControl ControlID="rgNWStatus" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbActiveSiteToggleNW2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbActiveSiteToggleNW2" />
                    <telerik:AjaxUpdatedControl ControlID="ltlActiveSiteToggleNW2" />
                    <telerik:AjaxUpdatedControl ControlID="rgNWReview" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbActiveSiteToggleNW3">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbActiveSiteToggleNW3" />
                    <telerik:AjaxUpdatedControl ControlID="ltlActiveSiteToggleNW3" />
                    <telerik:AjaxUpdatedControl ControlID="rgNWApprove" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbActiveSiteToggleNW4">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbActiveSiteToggleNW4" />
                    <telerik:AjaxUpdatedControl ControlID="ltlActiveSiteToggleNW4" />
                    <telerik:AjaxUpdatedControl ControlID="rgNWSitesNoTCP" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbActiveSiteToggle1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbActiveSiteToggle1" />
                    <telerik:AjaxUpdatedControl ControlID="ltlActiveSiteToggle1" />
                    <telerik:AjaxUpdatedControl ControlID="rgStatus" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbActiveSiteToggle2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbActiveSiteToggle2" />
                    <telerik:AjaxUpdatedControl ControlID="ltlActiveSiteToggle2" />
                    <telerik:AjaxUpdatedControl ControlID="rgReview" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbActiveSiteToggle3">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbActiveSiteToggle3" />
                    <telerik:AjaxUpdatedControl ControlID="ltlActiveSiteToggle3" />
                    <telerik:AjaxUpdatedControl ControlID="rgApprove" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbActiveSiteToggle4">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbActiveSiteToggle4" />
                    <telerik:AjaxUpdatedControl ControlID="ltlActiveSiteToggle4" />
                    <telerik:AjaxUpdatedControl ControlID="rgSitesNoTCP" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap"></telerik:RadAjaxLoadingPanel>
    <telerik:RadWindowManager RenderMode="Lightweight" ID="rwm" ShowContentDuringLoad="false" VisibleStatusbar="false" ReloadOnShow="true" runat="server" EnableShadow="true">
        <Windows>
            <telerik:RadWindow RenderMode="Lightweight" ID="rwTCPReview" runat="server" Behaviors="Close" OnClientClose="OnClientClose" Width="900" Height="900" Modal="true" />
        </Windows>
    </telerik:RadWindowManager>

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <asp:Panel ID="pnlNWReport" runat="server" CssClass="mainContent">
        <p>Use the filters in the column headings to narrow down the results shown, and click on the headings to sort by that column. Click the tabs to view TCPs by status. 
        </p>
        <telerik:RadTabStrip runat="server" ID="rts1" Orientation="HorizontalTop" SelectedIndex="0" MultiPageID="rmp1" Skin="Bootstrap">
            <Tabs>
                <telerik:RadTab Text="Currently Approved TCPs" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="TCPs Requiring Review" SelectedCssClass="selectedTab" />
                <telerik:RadTab Text="TCPs Requiring Approval" SelectedCssClass="selectedTab" />
                <telerik:RadTab Text="Sites with no TCP" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp1" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <div style="padding:10px;">
                    <h3>Currently Approved TCPs</h3>
                    <table cellpadding="5">
                        <tr>
                            <td>
                                <b><asp:Literal ID="ltlActiveSiteToggleNW1" runat="server" Text="Viewing active sites only" /></b>
                            </td>
                            <td>
                                &#10146;
                            </td>
                            <td>
                                <asp:LinkButton ID="lbActiveSiteToggleNW1" runat="server" OnCommand="lbActiveSiteToggle_Command" CommandArgument="viewInactiveNW1" Text="Click to view inactive sites" />
                            </td>
                        </tr>
                    </table>
                    <telerik:RadGrid ID="rgNWStatus" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                        Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="50" OnNeedDataSource="rgNWStatus_NeedDataSource"
                        AllowSorting="true" 
                        AllowMultiRowSelection="false" 
                        AllowFiltering="true"
                        AllowPaging="true"
                        AllowAutomaticDeletes="true" 
                        OnItemDataBound="rgNWStatus_ItemDataBound"
                        OnPreRender="rgNWStatus_PreRender">
                        <GroupingSettings CaseSensitive="false" />
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" />
                        <MasterTableView DataKeyNames="site_no" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                            <Columns>
                                <telerik:GridBoundColumn DataField="site_no" UniqueName="site_no" Display="false" />
                                <telerik:GridBoundColumn DataField="region_cd" UniqueName="region_cd" FilterControlWidth="40px" HeaderText="Region" HeaderStyle-Width="45px" />
                                <telerik:GridBoundColumn DataField="wsc_cd" UniqueName="wsc_cd" FilterControlWidth="40px" HeaderText="WSC" HeaderStyle-Width="45px"  />
                                <telerik:GridBoundColumn DataField="office_cd" UniqueName="office_cd" HeaderText="Office Code" FilterControlWidth="40px" HeaderStyle-Width="45px" />
                                <telerik:GridTemplateColumn DataField="site_no_nm" SortExpression="site_no" UniqueName="site_no_nm" HeaderText="Site" FilterControlWidth="150px" HeaderStyle-Width="400px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlSite" runat="server" Text='<%# Bind("site_no_nm") %>' Target="_blank" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="plan_no" SortExpression="plan_no" UniqueName="plan_no" HeaderText="Plan No." FilterControlWidth="40px" HeaderStyle-Width="60px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlPlan" runat="server" Text='<%# Bind("plan_no") %>' Target="_blank" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="reviewed_by" UniqueName="reviewed_by" HeaderText="Reviewed by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="reviewed_dt" UniqueName="reviewed_dt" HeaderText="Reviewed Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="approved_by" UniqueName="approved_by" HeaderText="Approved by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="approved_dt" UniqueName="approved_dt" HeaderText="Approved Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="site_tp_cd" UniqueName="site_tp_cd" HeaderText="Site Type" AllowFiltering="false" HeaderStyle-Width="25px" />
                                <telerik:GridTemplateColumn DataField="action" UniqueName="action" HeaderText="View TCP" HeaderStyle-Width="50px" AllowFiltering="false">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlAction" runat="server" Text='<%# Bind("action") %>' Target="_blank" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="wsc_id" UniqueName="wsc_id" Display="false" />
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpv1">
                <div style="padding:10px;">
                    <h3>TCPs Requiring Review</h3>
                    <table cellpadding="5">
                        <tr>
                            <td>
                                <b><asp:Literal ID="ltlActiveSiteToggleNW2" runat="server" Text="Viewing active sites only" /></b>
                            </td>
                            <td>
                                &#10146;
                            </td>
                            <td>
                                <asp:LinkButton ID="lbActiveSiteToggleNW2" runat="server" OnCommand="lbActiveSiteToggle_Command" CommandArgument="viewInactiveNW2" Text="Click to view inactive sites" />
                            </td>
                        </tr>
                    </table>
                    <telerik:RadGrid ID="rgNWReview" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                        Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="50" OnNeedDataSource="rgNWReview_NeedDataSource"
                        AllowSorting="true" 
                        AllowMultiRowSelection="false" 
                        AllowFiltering="true"
                        AllowPaging="true"
                        AllowAutomaticDeletes="true" 
                        OnItemDataBound="rgNWReview_ItemDataBound"
                        OnPreRender="rgNWReview_PreRender">
                        <GroupingSettings CaseSensitive="false" />
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" />
                        <MasterTableView DataKeyNames="site_no" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                            <Columns>
                                <telerik:GridBoundColumn DataField="site_no" UniqueName="site_no" Display="false" />
                                <telerik:GridBoundColumn DataField="region_cd" UniqueName="region_cd" FilterControlWidth="40px" HeaderText="Region" HeaderStyle-Width="45px" />
                                <telerik:GridBoundColumn DataField="wsc_cd" UniqueName="wsc_cd" FilterControlWidth="40px" HeaderText="WSC" HeaderStyle-Width="45px"  />
                                <telerik:GridBoundColumn DataField="office_cd" UniqueName="office_cd" HeaderText="Office Code" FilterControlWidth="40px" HeaderStyle-Width="45px" />
                                <telerik:GridTemplateColumn DataField="site_no_nm" SortExpression="site_no" UniqueName="site_no_nm" HeaderText="Site" FilterControlWidth="150px" HeaderStyle-Width="400px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlSite" runat="server" Text='<%# Bind("site_no_nm") %>' Target="_blank" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="plan_no" SortExpression="plan_no" UniqueName="plan_no" HeaderText="Plan No." FilterControlWidth="40px" HeaderStyle-Width="60px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlPlan" runat="server" Text='<%# Bind("plan_no") %>' Target="_blank" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="reviewed_by" UniqueName="reviewed_by" HeaderText="Reviewed by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="reviewed_dt" UniqueName="reviewed_dt" HeaderText="Reviewed Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="approved_by" UniqueName="approved_by" HeaderText="Approved by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="approved_dt" UniqueName="approved_dt" HeaderText="Approved Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="site_tp_cd" UniqueName="site_tp_cd" HeaderText="Site Type" AllowFiltering="false" HeaderStyle-Width="25px" />
                                <telerik:GridTemplateColumn DataField="action" UniqueName="action" HeaderText="View TCP" HeaderStyle-Width="50px" AllowFiltering="false">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlAction" runat="server" Text='<%# Bind("action") %>' Target="_blank" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="wsc_id" UniqueName="wsc_id" Display="false" />
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpv2">
                <div style="padding:10px;">
                    <h3>TCPs Requiring Approval</h3>
                    <table cellpadding="5">
                        <tr>
                            <td>
                                <b><asp:Literal ID="ltlActiveSiteToggleNW3" runat="server" Text="Viewing active sites only" /></b>
                            </td>
                            <td>
                                &#10146;
                            </td>
                            <td>
                                <asp:LinkButton ID="lbActiveSiteToggleNW3" runat="server" OnCommand="lbActiveSiteToggle_Command" CommandArgument="viewInactiveNW3" Text="Click to view inactive sites" />
                            </td>
                        </tr>
                    </table>
                    <telerik:RadGrid ID="rgNWApprove" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                        Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="50" OnNeedDataSource="rgNWApprove_NeedDataSource"
                        AllowSorting="true" 
                        AllowMultiRowSelection="false" 
                        AllowFiltering="true"
                        AllowPaging="true"
                        AllowAutomaticDeletes="true" 
                        OnItemDataBound="rgNWApprove_ItemDataBound"
                        OnPreRender="rgNWApprove_PreRender">
                        <GroupingSettings CaseSensitive="false" />
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" />
                        <MasterTableView DataKeyNames="site_no" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                            <Columns>
                                <telerik:GridBoundColumn DataField="site_no" UniqueName="site_no" Display="false" />
                                <telerik:GridBoundColumn DataField="region_cd" UniqueName="region_cd" FilterControlWidth="40px" HeaderText="Region" HeaderStyle-Width="45px" />
                                <telerik:GridBoundColumn DataField="wsc_cd" UniqueName="wsc_cd" FilterControlWidth="40px" HeaderText="WSC" HeaderStyle-Width="45px"  />
                                <telerik:GridBoundColumn DataField="office_cd" UniqueName="office_cd" HeaderText="Office Code" FilterControlWidth="40px" HeaderStyle-Width="45px" />
                                <telerik:GridTemplateColumn DataField="site_no_nm" SortExpression="site_no" UniqueName="site_no_nm" HeaderText="Site" FilterControlWidth="150px" HeaderStyle-Width="400px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlSite" runat="server" Text='<%# Bind("site_no_nm") %>' Target="_blank" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="plan_no" SortExpression="plan_no" UniqueName="plan_no" HeaderText="Plan No." FilterControlWidth="40px" HeaderStyle-Width="60px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlPlan" runat="server" Text='<%# Bind("plan_no") %>' Target="_blank" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="reviewed_by" UniqueName="reviewed_by" HeaderText="Reviewed by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="reviewed_dt" UniqueName="reviewed_dt" HeaderText="Reviewed Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="approved_by" UniqueName="approved_by" HeaderText="Approved by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="approved_dt" UniqueName="approved_dt" HeaderText="Approved Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="site_tp_cd" UniqueName="site_tp_cd" HeaderText="Site Type" AllowFiltering="false" HeaderStyle-Width="25px" />
                                <telerik:GridTemplateColumn DataField="action" UniqueName="action" HeaderText="View TCP" HeaderStyle-Width="50px" AllowFiltering="false">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlAction" runat="server" Text='<%# Bind("action") %>' Target="_blank" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="wsc_id" UniqueName="wsc_id" Display="false" />
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpv3">
                <div style="padding:10px;">
                    <h3>Sites with no TCP</h3>
                    <table cellpadding="5">
                        <tr>
                            <td>
                                <b><asp:Literal ID="ltlActiveSiteToggleNW4" runat="server" Text="Viewing active sites only" /></b>
                            </td>
                            <td>
                                &#10146;
                            </td>
                            <td>
                                <asp:LinkButton ID="lbActiveSiteToggleNW4" runat="server" OnCommand="lbActiveSiteToggle_Command" CommandArgument="viewInactiveNW4" Text="Click to view inactive sites" />
                            </td>
                        </tr>
                    </table>
                    <telerik:RadGrid ID="rgNWSitesNoTCP" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                        Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="50" OnNeedDataSource="rgNWSitesNoTCP_NeedDataSource"
                        AllowSorting="true" 
                        AllowMultiRowSelection="false" 
                        AllowFiltering="true"
                        AllowPaging="true"
                        AllowAutomaticDeletes="true" 
                        OnItemDataBound="rgNWSitesNoTCP_ItemDataBound"
                        OnPreRender="rgNWSitesNoTCP_PreRender">
                        <GroupingSettings CaseSensitive="false" />
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" />
                        <MasterTableView DataKeyNames="site_no" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                            <Columns>
                                <telerik:GridBoundColumn DataField="site_no" UniqueName="site_no" Display="false" />
                                <telerik:GridBoundColumn DataField="region_cd" UniqueName="region_cd" FilterControlWidth="40px" HeaderText="Region" HeaderStyle-Width="45px" />
                                <telerik:GridBoundColumn DataField="wsc_cd" UniqueName="wsc_cd" FilterControlWidth="40px" HeaderText="WSC" HeaderStyle-Width="45px"  />
                                <telerik:GridBoundColumn DataField="office_cd" UniqueName="office_cd" HeaderText="Office Code" FilterControlWidth="40px" HeaderStyle-Width="45px" />
                                <telerik:GridTemplateColumn DataField="site_no_nm" SortExpression="site_no" UniqueName="site_no_nm" HeaderText="Site" FilterControlWidth="150px" HeaderStyle-Width="400px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlSite" runat="server" Text='<%# Bind("site_no_nm") %>' Target="_blank" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="site_tp_cd" UniqueName="site_tp_cd" HeaderText="Site Type" AllowFiltering="false" HeaderStyle-Width="25px" />
                                <telerik:GridBoundColumn DataField="wsc_id" UniqueName="wsc_id" Display="false" />
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </asp:Panel>

    <asp:Panel ID="pnlWSCReport" runat="server" CssClass="mainContent">
        <table>
            <tr>
                <td>
                    <p style="padding-right:10px;">Use the filters in the column headings to narrow down the results shown, and click on the headings to sort by that column. Click the tabs to view TCPs by status. 
                         If you are not a safety approver for your WSC, you will only see the option to "view" and "review" in the Take Action column of the TCPs Requiring Approval tab.
                    </p>
                </td>
                <td nowrap>
                    <asp:Panel ID="pnlTCPStatus" runat="server" CssClass="TCPStatusPanel">
                        <h4>TCP WSC Status (real time SW only) for past 3 years</h4>
                        <p>Total TCPs: <b><asp:Literal ID="ltlTotalTCPNo" runat="server" /></b> (<asp:Literal ID="ltlTotalSiteTCPNo" runat="server" /> sites with TCPs - a site can have multiple TCPs)<br />
                        Approved TCPs: <b><asp:Literal ID="ltlApprovedTCPNo" runat="server" /></b>
                        <asp:Image ID="imgApprovedTT" runat="server" ImageURL="~/Images/tooltip.png" CssClass="imgPadding" />
                        <telerik:RadToolTip runat="server" ID="rttApproved" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            Height="120px" TargetControlID="imgApprovedTT" IsClientID="false" Animation="Fade" Position="TopRight">
                            This is the number of Real Time SW Site TCPs that have been approved in the past 3 years, regardless of review status. Meaning, the number here does not
                            necessarily match the number of TCPs that show up for the WSC under the Currently Approved TCPs tab.
                        </telerik:RadToolTip></p>
                    </asp:Panel>
                </td>
            </tr>
        </table>
        
        <telerik:RadTabStrip runat="server" ID="rts2" Orientation="HorizontalTop" SelectedIndex="0" MultiPageID="rmp2" Skin="Bootstrap">
            <Tabs>
                <telerik:RadTab Text="Currently Approved TCPs" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="TCPs Requiring Review" SelectedCssClass="selectedTab" />
                <telerik:RadTab Text="TCPs Requiring Approval" SelectedCssClass="selectedTab" />
                <telerik:RadTab Text="Sites with no TCP" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp2" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv4">
                <div style="padding:10px;">
                    <h3>Currently Approved TCPs</h3>
                    <table cellpadding="5">
                        <tr>
                            <td>
                                <b><asp:Literal ID="ltlActiveSiteToggle1" runat="server" Text="Viewing active sites only" /></b>
                            </td>
                            <td>
                                &#10146;
                            </td>
                            <td>
                                <asp:LinkButton ID="lbActiveSiteToggle1" runat="server" OnCommand="lbActiveSiteToggle_Command" CommandArgument="viewInactive1" Text="Click to view inactive sites" />
                            </td>
                        </tr>
                    </table>
                    <telerik:RadGrid ID="rgStatus" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                        Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="25" OnNeedDataSource="rgStatus_NeedDataSource"
                        AllowSorting="true" 
                        AllowMultiRowSelection="false" 
                        AllowFiltering="true"
                        AllowPaging="true"
                        AllowAutomaticDeletes="true" 
                        OnItemDataBound="rgStatus_ItemDataBound"
                        OnPreRender="rgStatus_PreRender">
                        <GroupingSettings CaseSensitive="false" />
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" />
                        <MasterTableView DataKeyNames="site_no" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                            <Columns>
                                <telerik:GridBoundColumn DataField="site_no" UniqueName="site_no" Display="false" />
                                <telerik:GridBoundColumn DataField="office_cd" UniqueName="office_cd" HeaderText="Office Code" FilterControlWidth="40px" HeaderStyle-Width="45px" />
                                <telerik:GridTemplateColumn DataField="site_no_nm" SortExpression="site_no" UniqueName="site_no_nm" HeaderText="Site" FilterControlWidth="150px" HeaderStyle-Width="400px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlSite" runat="server" Text='<%# Bind("site_no_nm") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="plan_no" SortExpression="plan_no" UniqueName="plan_no" HeaderText="Plan No." FilterControlWidth="40px" HeaderStyle-Width="60px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlPlan" runat="server" Text='<%# Bind("plan_no") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="reviewed_by" UniqueName="reviewed_by" HeaderText="Reviewed by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="reviewed_dt" UniqueName="reviewed_dt" HeaderText="Reviewed Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="approved_by" UniqueName="approved_by" HeaderText="Approved by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="approved_dt" UniqueName="approved_dt" HeaderText="Approved Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="reviewer_comments" UniqueName="reviewer_comments" HeaderText="Reviewer Comments" AllowFiltering="false" />
                                <telerik:GridBoundColumn DataField="site_tp_cd" UniqueName="site_tp_cd" HeaderText="Site Type" AllowFiltering="false" HeaderStyle-Width="25px" />
                                <telerik:GridTemplateColumn DataField="action" UniqueName="action" HeaderText="Edit TCP" HeaderStyle-Width="50px" AllowFiltering="false" SortExpression="action">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlAction" runat="server" Text='<%# Bind("action") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpv5">
                <div style="padding:10px;">
                    <h3>TCPs Requiring Review</h3>
                    <table cellpadding="5">
                        <tr>
                            <td>
                                <b><asp:Literal ID="ltlActiveSiteToggle2" runat="server" Text="Viewing active sites only" /></b>
                            </td>
                            <td>
                                &#10146;
                            </td>
                            <td>
                                <asp:LinkButton ID="lbActiveSiteToggle2" runat="server" OnCommand="lbActiveSiteToggle_Command" CommandArgument="viewInactive2" Text="Click to view inactive sites" />
                            </td>
                        </tr>
                    </table>
                    <telerik:RadGrid ID="rgReview" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                        Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="25" OnNeedDataSource="rgReview_NeedDataSource"
                        AllowSorting="true" 
                        AllowMultiRowSelection="false" 
                        AllowFiltering="true"
                        AllowPaging="true"
                        AllowAutomaticDeletes="true" 
                        OnItemDataBound="rgReview_ItemDataBound"
                        OnPreRender="rgReview_PreRender">
                        <GroupingSettings CaseSensitive="false" />
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" />
                        <MasterTableView DataKeyNames="site_no" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                            <Columns>
                                <telerik:GridBoundColumn DataField="site_no" UniqueName="site_no" Display="false" />
                                <telerik:GridBoundColumn DataField="office_cd" UniqueName="office_cd" HeaderText="Office Code" FilterControlWidth="40px" HeaderStyle-Width="45px" />
                                <telerik:GridTemplateColumn DataField="site_no_nm" SortExpression="site_no" UniqueName="site_no_nm" HeaderText="Site" FilterControlWidth="150px" HeaderStyle-Width="400px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlSite" runat="server" Text='<%# Bind("site_no_nm") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="plan_no" SortExpression="plan_no" UniqueName="plan_no" HeaderText="Plan No. (Edit)" FilterControlWidth="40px" HeaderStyle-Width="60px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlPlan" runat="server" Text='<%# Bind("plan_no") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="reviewed_by" UniqueName="reviewed_by" HeaderText="Reviewed by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="reviewed_dt" UniqueName="reviewed_dt" HeaderText="Reviewed Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="approved_by" UniqueName="approved_by" HeaderText="Approved by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="approved_dt" UniqueName="approved_dt" HeaderText="Approved Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="site_tp_cd" UniqueName="site_tp_cd" HeaderText="Site Type" AllowFiltering="false" HeaderStyle-Width="25px" />
                                <telerik:GridTemplateColumn DataField="action" UniqueName="action" HeaderText="Review TCP w/out Edit" HeaderStyle-Width="50px" AllowFiltering="false">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbAction" runat="server" Text='<%# Bind("action") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpv6">
                <div style="padding:10px;">
                    <h3>TCPs Requiring Approval</h3>
                    <table cellpadding="5">
                        <tr>
                            <td>
                                <b><asp:Literal ID="ltlActiveSiteToggle3" runat="server" Text="Viewing active sites only" /></b>
                            </td>
                            <td>
                                &#10146;
                            </td>
                            <td>
                                <asp:LinkButton ID="lbActiveSiteToggle3" runat="server" OnCommand="lbActiveSiteToggle_Command" CommandArgument="viewInactive3" Text="Click to view inactive sites" />
                            </td>
                        </tr>
                    </table>
                    <telerik:RadGrid ID="rgApprove" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                        Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="25" OnNeedDataSource="rgApprove_NeedDataSource"
                        AllowSorting="true" 
                        AllowMultiRowSelection="false" 
                        AllowFiltering="true"
                        AllowPaging="true"
                        AllowAutomaticDeletes="true" 
                        OnItemDataBound="rgApprove_ItemDataBound"
                        OnPreRender="rgApprove_PreRender">
                        <GroupingSettings CaseSensitive="false" />
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" />
                        <MasterTableView DataKeyNames="site_no" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                            <Columns>
                                <telerik:GridBoundColumn DataField="site_no" UniqueName="site_no" Display="false" />
                                <telerik:GridBoundColumn DataField="office_cd" UniqueName="office_cd" HeaderText="Office Code" FilterControlWidth="40px" HeaderStyle-Width="45px" />
                                <telerik:GridTemplateColumn DataField="site_no_nm" SortExpression="site_no" UniqueName="site_no_nm" HeaderText="Site" FilterControlWidth="150px" HeaderStyle-Width="400px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlSite" runat="server" Text='<%# Bind("site_no_nm") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="plan_no" SortExpression="plan_no" UniqueName="plan_no" HeaderText="Plan No." FilterControlWidth="40px" HeaderStyle-Width="60px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlPlan" runat="server" Text='<%# Bind("plan_no") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="reviewed_by" UniqueName="reviewed_by" HeaderText="Reviewed by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="reviewed_dt" UniqueName="reviewed_dt" HeaderText="Reviewed Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="approved_by" UniqueName="approved_by" HeaderText="Approved by" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                                <telerik:GridBoundColumn DataField="approved_dt" UniqueName="approved_dt" HeaderText="Approved Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                                <telerik:GridBoundColumn DataField="reviewer_comments" UniqueName="reviewer_comments" HeaderText="Reviewer Comments" AllowFiltering="false" />
                                <telerik:GridBoundColumn DataField="site_tp_cd" UniqueName="site_tp_cd" HeaderText="Site Type" AllowFiltering="false" HeaderStyle-Width="25px" />
                                <telerik:GridTemplateColumn DataField="action" UniqueName="action" HeaderText="Approve TCP" HeaderStyle-Width="50px" AllowFiltering="false">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbAction" runat="server" Text='<%# Bind("action") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpv7">
                <div style="padding:10px;">
                    <h3>Sites with no TCP</h3>
                    <table cellpadding="5">
                        <tr>
                            <td>
                                <b><asp:Literal ID="ltlActiveSiteToggle4" runat="server" Text="Viewing active sites only" /></b>
                            </td>
                            <td>
                                &#10146;
                            </td>
                            <td>
                                <asp:LinkButton ID="lbActiveSiteToggle4" runat="server" OnCommand="lbActiveSiteToggle_Command" CommandArgument="viewInactive4" Text="Click to view inactive sites" />
                            </td>
                        </tr>
                    </table>
                    <telerik:RadGrid ID="rgSitesNoTCP" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                        Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="25" OnNeedDataSource="rgSitesNoTCP_NeedDataSource"
                        AllowSorting="true" 
                        AllowMultiRowSelection="false" 
                        AllowFiltering="true"
                        AllowPaging="true"
                        AllowAutomaticDeletes="true" 
                        OnItemDataBound="rgSitesNoTCP_ItemDataBound"
                        OnPreRender="rgSitesNoTCP_PreRender">
                        <GroupingSettings CaseSensitive="false" />
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" />
                        <MasterTableView DataKeyNames="site_no" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                            <Columns>
                                <telerik:GridBoundColumn DataField="site_no" UniqueName="site_no" Display="false" />
                                <telerik:GridBoundColumn DataField="office_cd" UniqueName="office_cd" HeaderText="Office Code" FilterControlWidth="40px" HeaderStyle-Width="45px" />
                                <telerik:GridTemplateColumn DataField="site_no_nm" SortExpression="site_no" UniqueName="site_no_nm" HeaderText="Site" FilterControlWidth="150px" HeaderStyle-Width="400px">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlSite" runat="server" Text='<%# Bind("site_no_nm") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="site_tp_cd" UniqueName="site_tp_cd" HeaderText="Site Type" AllowFiltering="false" HeaderStyle-Width="35px" />
                                <telerik:GridTemplateColumn DataField="action" UniqueName="action" HeaderText="Create TCP" HeaderStyle-Width="50px" AllowFiltering="false">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlAction" runat="server" Text='<%# Bind("action") %>' />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
        
    </asp:Panel>
</asp:Content>
