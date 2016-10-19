<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSite.Master" CodeBehind="SiteRegister.aspx.vb" Inherits="SIMS.SiteRegister" %>
<%@ MasterType  virtualPath="~/SIMSSite.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Web20">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxPanel ID="rap" runat="server" LoadingPanelID="ralp">
    <asp:Panel ID="pnlHasAccess" runat="server">
        <div style="border:none;height:40px;">
            <div style="float:left;">
                <asp:Label ID="lblError" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblSuccess" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Green"></asp:Label>
            </div>
        </div>
        <asp:Panel ID="pnlEnterSite" runat="server">
            <table cellpadding="5">
                <tr>
                    <td><b>Enter the 8 or 15 digit site number:</b></td>
                    <td><asp:TextBox ID="tbSiteNo" runat="server" MaxLength="15" Width="120px" /></td>
                </tr>
                <tr>
                    <td><b>Agency code:</b></td>
                    <td><asp:TextBox ID="tbAgencyCd" runat="server" Text="USGS" MaxLength="5" Width="50px" /></td>
                </tr>
                <tr>
                    <td><b>Assign this site to an office:</b></td>
                    <td><asp:DropDownList ID="ddlOffice" runat="server" DataValueField="office_id" DataTextField="office_nm" /></td>
                </tr>
                <tr>
                    <td colspan="2"><br />
                        <asp:Button ID="btnAdd" runat="server" Text="Add" OnCommand="btnAdd_Command" CommandArgument="ConfirmSite" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlConfirmSite" runat="server">
            <asp:HiddenField ID="hfNWISSiteID" runat="server" />
            <table cellpadding="5">
                <tr>
                    <td><span class="SITitleFontSmall">Adding site number:</span></td>
                    <td><b><asp:Literal ID="ltlSiteNo" runat="server" /></b></td>
                </tr>
                <tr>
                    <td><span class="SITitleFontSmall">For office:</span></td>
                    <td><b><asp:Literal ID="ltlOffice" runat="server" /></b></td>
                </tr>
                <tr>
                    <td><span class="SITitleFontSmall">Short site name:</span></td>
                    <td><b><asp:Literal ID="ltlSiteNm" runat="server" /></b></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span class="SITitleFontSmall">Please modify the site name to the published, full site name.</span><br />
                        <asp:TextBox ID="tbSiteFullNm" runat="server" Width="400px" /><br />
                        Refer to the following example:<br />
                        <span style="font-weight:bold;font-style:italic;">Salt Fk Red Rv nr Wellington, TX</span> is the short name.<br />
                        <span style="font-weight:bold;font-style:italic;">Salt Fork Red River near Wellington, TX</span> is the full name.
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnAdd2" runat="server" Text="Add" OnCommand="btnAdd_Command" CommandArgument="AddSite" />
                        <asp:Button ID="btnCancel2" runat="server" Text="Cancel" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
    </telerik:RadAjaxPanel>
</asp:Content>
