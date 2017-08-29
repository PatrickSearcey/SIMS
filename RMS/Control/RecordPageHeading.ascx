<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecordPageHeading.ascx.cs" Inherits="RMS.Control.RecordPageHeading" %>
<div class="selector">
    <div class="headers">
        <h2><asp:Literal ID="ltlPageTitle" runat="server" /></h2>
        <h3><asp:Literal ID="ltlRecordType" runat="server" /><br />
        <asp:HyperLink ID="hlPageSubTitle" runat="server" /></h3>
    </div>
    <div class="officeinfo">
        <b>Responsible Office</b><br />
        U.S. Geological Survey<br />
        <asp:Literal ID="ltlOfficeInfo" runat="server" />
    </div>
</div>