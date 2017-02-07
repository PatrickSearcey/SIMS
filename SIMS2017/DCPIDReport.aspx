<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="DCPIDReport.aspx.cs" Inherits="SIMS2017.DCPIDReport" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>
<%@ Register Src="~/Control/OfficeSelector.ascx" TagName="OfficeSelector" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/stationinfo.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="osDCPReport">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="osDCPReport" />
                    <telerik:AjaxUpdatedControl ControlID="rgDCPIDs" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlOfficeName" />
                    <telerik:AjaxUpdatedControl ControlID="pnlFieldTrip" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="lbNotAssignedToggle" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgDCPIDs">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgDCPIDs" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbNotAssignedToggle">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgDCPIDs" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="lbNotAssignedToggle" />
                    <telerik:AjaxUpdatedControl ControlID="ltlOfficeName" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="phDCPReport" runat="server" />
    <div style="margin-top:-40px"><uc:OfficeSelector id="osDCPReport" runat="server" /></div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">

    <div class="mainContent">
        <h3><asp:Literal ID="ltlOfficeName" runat="server" /></h3>

        <asp:Panel ID="pnlFieldTrip" runat="server">
            <h4 style="border-bottom:none !important;"><asp:Literal ID="ltlFieldTrip" runat="server" /></h4>
        </asp:Panel>
      
        <div style="width:100%;text-align:right;font-weight:bold;margin-top:-30px;margin-bottom:5px;">
            <asp:LinkButton ID="lbNotAssignedToggle" runat="server" OnCommand="ChangeView" CommandName="NotAssigned" />
        </div>

        <div>
            <telerik:RadGrid ID="rgDCPIDs" runat="server"
                OnNeedDataSource="rgDCPIDs_NeedDataSource"
                OnPreRender="rgDCPIDs_PreRender">
                <ClientSettings EnableAlternatingItems="false" />
                <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="false" AllowFilteringByColumn="true" AllowSorting="true">
                    <Columns>
                        <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="site_no" HeaderText="Site Number" FilterControlWidth="100px">
                            <ItemTemplate>
                                <b><a href='<%# String.Format("StationInfo.aspx?site_id={0}", Eval("site_id")) %>'><%# Eval("site_no") %></a></b>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="station_nm"  HeaderText="Station Name" FilterControlWidth="200px" HeaderStyle-Width="350px" />
                        <telerik:GridBoundColumn DataField="DCPID" FilterControlWidth="100px" HeaderText="DCPID" />
                        <telerik:GridBoundColumn DataField="channel" FilterControlWidth="50px" HeaderText="Prim./ Rndm. Channel" />
                        <telerik:GridBoundColumn DataField="baud" FilterControlWidth="50px" HeaderText="Prim./ Rndm. Baud" />
                        <telerik:GridBoundColumn DataField="elevation" FilterControlWidth="50px" HeaderText="Satellite Azimuth/ Elevation" />
                        <telerik:GridBoundColumn DataField="transmission" FilterControlWidth="50px" HeaderText="Trans. Time/ Interval/ Window" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
    </div>
</asp:Content>
