<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="Tasks.aspx.cs" Inherits="SIMS2017.Admin.Tasks" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/admin.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <asp:Panel ID="pnlNoAccess" runat="server">
            <h4>You do not have the necessary permission to access Administration Tasks for this WSC. Please contact your WSC SIMS Admin if you require access.</h4>
        </asp:Panel>

        <asp:Panel ID="pnlHasAccess" runat="server">
            <table width="100%">
                <tr>
                    <td valign="top">
                        <h3>General Tasks</h3>
                        <div class="GeneralTasks">
                            <asp:HyperLink ID="hlRegisterSite" runat="server" Text="Register Site in SIMS" /><br />
                            <asp:HyperLink ID="hlManageOffices" runat="server" Text="Manage Offices" /><br />
                            <asp:HyperLink ID="hlManagePersonnel" runat="server" Text="Manage Personnel" /><br />
                            <asp:HyperLink ID="hlManageFieldTrips" runat="server" Text="Manage Field Trips" />
                        </div>
                        <h3>Endangered Gages</h3>
                        <div class="GeneralTasks">
                            <asp:HyperLink ID="hlEndangeredGages" runat="server" Text="List of Endangered Gages" />
                        </div>
                    </td>
                    <td>
                        <h3>RMS Specific Tasks</h3>
                        <div class="RMSTasks">
                            <asp:HyperLink ID="hlManageRecords" runat="server" Text="Create/Modify Record Configurations" /><br />
                            <asp:HyperLink ID="hlManageRecordTypes" runat="server" Text="Manage Record-Types" /><br />
                            <h5>Manage Record Periods</h5>
                            <div style="padding: 0 0 0 20px">
                                <asp:HyperLink ID="hlPeriodDates" runat="server" Text="Record Period Dates" /><br />
                                <asp:HyperLink ID="hlPeriodStatus" runat="server" Text="Record Period Status" /><br />
                                <asp:HyperLink ID="hlSANAL" runat="server" Text="Station Analysis Notes" /><br />
                                <asp:HyperLink ID="hlUnlock" runat="server" Text="Unlock Record Period" />
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>
