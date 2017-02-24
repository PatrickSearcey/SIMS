<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="RegisterSite.aspx.cs" Inherits="SIMS2017.Admin.RegisterSite" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/admin.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlEnterSite" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlConfirmSite" />
                    <telerik:AjaxUpdatedControl ControlID="pnlError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbAddSite">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlFinal" />
                    <telerik:AjaxUpdatedControl ControlID="pnlConfirmSite" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbCancel2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlConfirmSite" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlEnterSite" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbAddSite2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlFinal" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlEnterSite" /> 
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
            <h4>You do not have the necessary permission to access Administration Tasks for this WSC. Please contact your WSC SIMS Admin if you require access.</h4>
        </asp:Panel>

        <asp:Panel ID="pnlHasAccess" runat="server">
            <asp:Panel ID="pnlError" runat="server" CssClass="pnlNotice">
                <h3>Error!</h3>
                <asp:Literal ID="ltlError" runat="server" />
            </asp:Panel>

            <asp:Panel ID="pnlEnterSite" runat="server">
                <b>Enter the 8 or 15 digit site number:</b> <telerik:RadTextBox ID="rtbSiteNo" runat="server" MaxLength="15" Width="250px" Skin="Bootstrap" /><br />
                <b>Agency Code:</b> <telerik:RadTextBox ID="rtbAgencyCode" runat="server" Text="USGS" Skin="Bootstrap" Width="100px" /><br />
                <b>Assign this site to an office:</b> <telerik:RadDropDownList ID="rddlOffice" runat="server" Width="300px" Skin="Bootstrap" DataValueField="office_id" DataTextField="office_nm" />
                <telerik:RadButton ID="rbSubmit" runat="server" Text="Submit" OnCommand="SubmitEvent" CommandArgument="ConfirmSite" AutoPostBack="true" Skin="Bootstrap" />
                <telerik:RadButton ID="rbCancel1" runat="server" Text="Cancel" OnCommand="CancelEvent" CommandArgument="Cancel" AutoPostBack="true" Skin="Bootstrap" />
            </asp:Panel>

            <asp:Panel ID="pnlConfirmSite" runat="server">
                <table width="700" cellpadding="10px">
                    <tr>
                        <td><b>Adding site number:</b></td>
                        <td><b><asp:Literal ID="ltlSiteNo" runat="server" /></b></td>
                    </tr>
                    <tr>
                        <td><b>For office:</b></td>
                        <td><b><asp:Literal ID="ltlOffice" runat="server" /></b></td>
                    </tr>
                    <tr>
                        <td><b>Short site name:</b></td>
                        <td><b><asp:Literal ID="ltlSiteName" runat="server" /></b></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <b>Please modify the site name to the published, full site name.</b><br />
                            <telerik:RadTextBox ID="rtbSiteName" runat="server" Skin="Bootstrap" Width="500px" />
                            <asp:Image ID="imgSiteNameHelp" runat="server" ImageUrl="~/images/tooltip.png" AlternateText="tooltip" />
                            <telerik:RadToolTip runat="server" ID="rrt1" RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                                Height="50px" TargetControlID="imgSiteNameHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                Refer to the following example:<br />
                                <b><i>Salt Fk Red Rv nr Wellington, TX</i></b> is the short name.<br />
                                <b><i>Salt Fork Red River near Wellington, TX</i></b> is the full name.
                            </telerik:RadToolTip>
                        </td>
                    </tr>
                </table>
                <br />
                <telerik:RadButton ID="rbAddSite" runat="server" Text="Add" OnCommand="SubmitEvent" CommandArgument="AddSite" AutoPostBack="true" Skin="Bootstrap" />
                <telerik:RadButton ID="rbCancel2" runat="server" Text="Cancel" OnCommand="CancelEvent" CommandArgument="CancelAdd" AutoPostBack="true" Skin="Bootstrap" />
            </asp:Panel>

            <asp:Panel ID="pnlFinal" runat="server">
                <p>The site was successfully registered in SIMS!  You may now visit its <asp:HyperLink ID="hlStationInfo" runat="server" Text="Station Information" />
                    page to manage site-related data. 
                </p>
                <telerik:RadButton ID="rbAddSite2" runat="server" Text="Return to add another site" OnCommand="SubmitEvent" CommandArgument="StartOver" AutoPostBack="true" Skin="Bootstrap" />
            </asp:Panel>
        </asp:Panel>
    </div>
</asp:Content>
