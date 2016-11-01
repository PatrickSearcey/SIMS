<%@ Page Title="" Language="C#" MasterPageFile="~/SIMS.Master" AutoEventWireup="true" CodeBehind="SIMSWSCHome.aspx.cs" Inherits="SIMS2017.SIMSWSCHome" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Control/OfficeSelector.ascx" TagName="OfficeSelector" TagPrefix="uc" %>

<asp:Content ID="Content0" ContentPlaceHolderID="head" Runat="Server">
    <link href="styles/default.css" rel="stylesheet" />
    <style type="text/css">
        div.RadGrid .rgAltRow {
            background: white;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="osHome">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="osHome" />
                    <telerik:AjaxUpdatedControl ControlID="rgSites" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlOfficeName" />
                    <telerik:AjaxUpdatedControl ControlID="pnlFieldTrip" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgSites">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgSites" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <telerik:RadPageLayout runat="server" ID="rplTop">
        <Rows>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn CssClass="jumbotron">
                        <h2>Welcome to the <asp:Literal ID="ltlWSCName" runat="server" /></h2>
                        <img src="images/HomepageTitle.png" alt="SIMS" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn HiddenMd="true" HiddenSm="true" HiddenXs="true">
                        <uc:OfficeSelector id="osHome" runat="server" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="Server">
    <telerik:RadPageLayout runat="server" ID="rplContent">
        <Rows>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
                        <h3><asp:Literal ID="ltlOfficeName" runat="server" /></h3>

                        <asp:Panel ID="pnlFieldTrip" runat="server">
                            <h4><asp:Literal ID="ltlFieldTrip" runat="server" /></h4>
                            <b><asp:HyperLink ID="hlMapTrip" runat="server" Text="Map Trip" /> | <asp:HyperLink ID="hlRealtimeGraphs" runat="server" Text="View Realtime Graphs for Field Trip" Target="_blank" /></b>
                        </asp:Panel>
                        
                        <b>Note: real-time sites have light blue backgrounds</b> | 
                        <asp:HyperLink ID="lbFAQ" runat="server" Text="FAQ for Master Station List" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
                        <telerik:RadGrid ID="rgSites" runat="server"
                            OnNeedDataSource="rgSites_NeedDataSource"
                            OnPreRender="rgSites_PreRender"
                            OnItemDataBound="rgSites_ItemDataBound">
                            <ClientSettings EnableAlternatingItems="false" />
                            <MasterTableView AutoGenerateColumns="false" AllowFilteringByColumn="true" AllowSorting="true">
                                <Columns>
                                    <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="site_no" HeaderText="Site Number" FilterControlWidth="100px">
                                        <ItemTemplate>
                                            <b><a href='<%# String.Format("{0}StationInfo.asp?site_id={1}", Eval("SIMSClassicURL"), Eval("site_id")) %>'><%# Eval("site_no") %></a></b>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="station_nm" AllowSorting="true" AllowFiltering="true" HeaderText="Station Name" />
                                    <telerik:GridTemplateColumn AllowSorting="false" AllowFiltering="false" HeaderText="Email Request">
                                        <ItemTemplate>
                                            <a href='<%# String.Format("{0}NWISOpsRequest.aspx?office_id={1}&wsc_id={2}&site_id={3}", Eval("SIMSURL"), Eval("office_id"), Eval("wsc_id"), Eval("site_id")) %>'>Email</a>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn AllowSorting="false" AllowFiltering="false" HeaderText="NWIS Web">
                                        <ItemTemplate>
                                            <a href='<%# String.Format("http://waterdata.usgs.gov/nwis/nwisman/?site_no={0}&agency_cd={1}", Eval("site_no"), Eval("agency_cd")) %>' target="_blank">Go!</a>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="SiteType" AllowFiltering="true" AllowSorting="true" HeaderText="Site Type" FilterControlWidth="40px" />
                                    <telerik:GridBoundColumn DataField="Active" AllowFiltering="true" AllowSorting="true" HeaderText="Active" FilterControlWidth="80px" />
                                    <telerik:GridBoundColumn DataField="TelFlag" UniqueName="TelFlag" Display="false" />
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
