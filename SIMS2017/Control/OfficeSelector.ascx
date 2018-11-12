<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OfficeSelector.ascx.cs" Inherits="SIMS2017.Control.OfficeSelector" %>
<div class="selector">
    <asp:Panel ID="pnl1" runat="server" CssClass="filters" DefaultButton="btnSiteNo">
        <table>
            <tr>
                <td><b>By office</b></td>
                <td><telerik:RadDropDownList ID="rddlOffice" runat="server" AutoPostBack="true" DataValueField="office_id" DataTextField="office_nm" OnSelectedIndexChanged="Filter_SelectedIndexChanged" Width="345px" /></td>
            </tr>
            <tr>
                <td><b>By field trip</b></td>
                <td><telerik:RadDropDownList ID="rddlFieldTrip" runat="server" AutoPostBack="true" DataValueField="trip_id" DataTextField="TripName" OnSelectedIndexChanged="Filter_SelectedIndexChanged" Width="345px" /></td>
            </tr>
            <tr>
                <td><b><asp:Literal ID="ltlSiteNo" runat="server" Text="By site number" /></b></td>
                <td><asp:TextBox ID="tbSiteNo" runat="server" /> <asp:TextBox ID="tbAgencyCd" runat="server" Text="USGS" Width="80px" /> <asp:Button ID="btnSiteNo" runat="server" OnClick="btnSiteNo_Click" Text="Go!" /></td>
            </tr>
        </table>
    </asp:Panel>
    <div class="officeinfo">
        <b>Responsible Office</b><br />
        U.S. Geological Survey<br />
        <asp:Literal ID="ltlOfficeInfo" runat="server" />
    </div>
</div>