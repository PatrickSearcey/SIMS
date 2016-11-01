<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OfficeSelector.ascx.cs" Inherits="SIMS2017.Control.OfficeSelector" %>
<div class="selector">
    <div class="filters">
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
                <td><b>By site number</b></td>
                <td><asp:TextBox ID="tbSiteNo" runat="server" /> <asp:TextBox ID="tbAgencyCd" runat="server" Text="USGS" Width="80px" /> <asp:Button ID="btnSiteNo" runat="server" OnCommand="btnSiteNo_Command" Text="Go!" /></td>
            </tr>
        </table>
    </div>
    <div class="officeinfo">
        <b>Responsible Office</b><br />
        U.S. Geological Survey<br />
        <asp:Literal ID="ltlOfficeInfo" runat="server" />
    </div>
</div>