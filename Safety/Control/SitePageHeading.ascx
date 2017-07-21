<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SitePageHeading.ascx.cs" Inherits="Safety.Control.SitePageHeading" %>
<asp:Panel ID="pnlFull" runat="server">
<div class="selector">
    <div class="headers">
        <h2><asp:Literal ID="ltlPageTitle" runat="server" /></h2>
        <h3><asp:HyperLink ID="hlPageSubTitle" runat="server" /></h3>
    </div>
    <asp:Panel ID="pnlOffice" runat="server" CssClass="officeinfo">
        <b>Responsible Office</b><br />
        U.S. Geological Survey<br />
        <asp:Literal ID="ltlOfficeInfo" runat="server" />
    </asp:Panel>
</div>
</asp:Panel>
<asp:Panel ID="pnlPart" runat="server">
    <div style="padding-left:50px">
    <h2><asp:Literal ID="ltlPageTitle2" runat="server" /></h2>
    </div>
</asp:Panel>