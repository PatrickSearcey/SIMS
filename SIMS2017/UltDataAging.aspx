<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="UltDataAging.aspx.cs" Inherits="SIMS2017.UltDataAging" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <telerik:RadAjaxPanel ID="rap" runat="server">

        <asp:GridView ID="gvCRP" runat="server" EnableViewState="true" AutoGenerateColumns="False" Width="100%">
            <Columns>
                <asp:BoundField DataField="office_cd" HeaderText="office cd" ItemStyle-BackColor="White" SortExpression="office_cd" ItemStyle-VerticalAlign="Top" />
                <asp:BoundField DataField="site_no" HeaderText="site number" SortExpression="site_no" ItemStyle-BackColor="White" ItemStyle-VerticalAlign="Top" />
                <asp:BoundField DataField="station_nm" HeaderText="station name" ItemStyle-Wrap="false" ItemStyle-BackColor="White" SortExpression="station_nm" ItemStyle-VerticalAlign="Top" />
                <asp:BoundField DataField="dd_nu" HeaderText="DD" SortExpression="dd_nu" ItemStyle-BackColor="White" />
                <asp:BoundField DataField="parm_cd" HeaderText="param cd" ItemStyle-BackColor="White" SortExpression="parm_cd" />
                <asp:BoundField DataField="type_cd" HeaderText="record-type" SortExpression="type_cd" ItemStyle-BackColor="White" />
                <asp:BoundField DataField="category_no" HeaderText="cat no" ItemStyle-BackColor="White" SortExpression="category_no" />
                <asp:BoundField DataField="work_period_dt" HeaderText="last worked period in RMS" SortExpression="work_period_beg_dt" ItemStyle-Wrap="false" />
                <asp:BoundField DataField="check_period_dt" HeaderText="last checked period in RMS" SortExpression="check_period_beg_dt" ItemStyle-Wrap="false" />
                <asp:BoundField DataField="rev_period_dt" HeaderText="last reviewed period in RMS" SortExpression="rev_period_beg_dt" ItemStyle-Wrap="false" />
                <asp:BoundField DataField="last_aging_dt" HeaderText="date" HeaderStyle-BackColor="Olive" SortExpression="last_aging_dt" DataFormatString="{0:MM/dd/yyyy}" />
                <asp:BoundField DataField="DaysSinceAging" HeaderText="days ago" HeaderStyle-BackColor="Olive" SortExpression="DaysSinceAging" ItemStyle-BackColor="White" />
            </Columns>
            <EmptyDataTemplate>
                <i>No sites returned</i>
            </EmptyDataTemplate>
            <RowStyle Wrap="false" />
        </asp:GridView>
    </telerik:RadAjaxPanel>
</asp:Content>
