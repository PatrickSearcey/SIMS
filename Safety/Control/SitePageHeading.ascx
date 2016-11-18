<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SitePageHeading.ascx.cs" Inherits="Safety.Control.SitePageHeading" %>
<div class="selector">
    <div class="headers">
        <h2><asp:Literal ID="ltlPageTitle" runat="server" /></h2>
        <h3><asp:HyperLink ID="hlPageSubTitle" runat="server" /></h3>
    </div>
    <div class="officeinfo">
        <b>Responsible Office</b><br />
        U.S. Geological Survey<br />
        <asp:Literal ID="ltlOfficeInfo" runat="server" />
    </div>
</div>