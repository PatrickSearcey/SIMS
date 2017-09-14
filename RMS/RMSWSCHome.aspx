<%@ Page Title="" Language="C#" MasterPageFile="~/RMS.Master" AutoEventWireup="true" CodeBehind="RMSWSCHome.aspx.cs" Inherits="RMS.RMSWSCHome" %>
<%@ Register Src="~/Control/OfficeSelector.ascx" TagName="OfficeSelector" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/default.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <telerik:RadAjaxPanel ID="rap" runat="server" LoadingPanelID="ralp">
    <telerik:RadPageLayout runat="server" ID="rplTop">
        <Rows>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn CssClass="jumbotron">
                        <h2>Welcome to the <asp:Literal ID="ltlWSCName" runat="server" /></h2>
                        <img src="images/RMSTitle.png" alt="SIMS" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn HiddenMd="true" HiddenSm="true" HiddenXs="true">
                        <uc:OfficeSelector id="osHome" runat="server" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

     <telerik:RadPageLayout runat="server" ID="rplContent">
        <Rows>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
                        <h3><asp:Literal ID="ltlOfficeName" runat="server" /></h3>

                        <asp:Panel ID="pnlFieldTrip" runat="server">
                            <h4><asp:Literal ID="ltlFieldTrip" runat="server" /></h4>
                            <b><asp:HyperLink ID="hlMapTrip" runat="server" Text="Map Trip" /> | <asp:HyperLink ID="hlRealtimeGraphs" runat="server" Text="View Realtime Graphs for Field Trip" Target="_blank" /></b>
                        </asp:Panel>
                        
                        <div style="float:left;">
                            &nbsp;
                        </div>

                        <div style="float:right;font-weight:bold;margin-top:-20px;">
                            <asp:Literal ID="ltlRecordCount" runat="server" />
                        </div>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
                        <telerik:RadGrid ID="rgRecords" runat="server"
                            OnNeedDataSource="rgRecords_NeedDataSource"
                            OnPreRender="rgRecords_PreRender"
                            OnItemDataBound="rgRecords_ItemDataBound">
                            <ClientSettings EnableAlternatingItems="false" />
                            <GroupingSettings CaseSensitive="false" />
                            <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="false" AllowFilteringByColumn="true" AllowSorting="true">
                                <ColumnGroups>
                                    <telerik:GridColumnGroup Name="Analyzed" HeaderText="Analyzed" HeaderStyle-HorizontalAlign="Center" />
                                    <telerik:GridColumnGroup Name="Approved" HeaderText="Approved" HeaderStyle-HorizontalAlign="Center" />
                                </ColumnGroups>
                                <Columns>
                                    <telerik:GridTemplateColumn AllowSorting="true" AllowFiltering="true" SortExpression="site_no" HeaderText="Site Number" FilterControlWidth="90px">
                                        <ItemTemplate>
                                            <b><a href='<%# String.Format("{0}StationInfo.aspx?site_id={1}", Eval("SIMSURL"), Eval("site_id")) %>'><%# Eval("site_no") %></a></b>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="station_nm" AllowSorting="true" AllowFiltering="true" HeaderText="Station Name" />

                                    <telerik:GridBoundColumn DataField="Analyzer" AllowSorting="true" AllowFiltering="true" HeaderText="Assigned To" ColumnGroupName="Analyzed" FilterControlWidth="80px" />
                                    <telerik:GridDateTimeColumn DataField="AnalyzedDt" DataFormatString="{0:MM/dd/yyyy}" AllowSorting="true" AllowFiltering="false" HeaderText="Through" ColumnGroupName="Analyzed" />
                                    <telerik:GridBoundColumn DataField="AnalyzedBy" AllowSorting="true" AllowFiltering="true" HeaderText="By" ColumnGroupName="Analyzed" FilterControlWidth="80px" />

                                    <telerik:GridBoundColumn DataField="Approver" AllowSorting="true" AllowFiltering="true" HeaderText="Assigned To" ColumnGroupName="Approved" FilterControlWidth="80px" />
                                    <telerik:GridDateTimeColumn DataField="ApprovedDt" DataFormatString="{0:MM/dd/yyyy}" AllowSorting="true" AllowFiltering="false" HeaderText="Through" ColumnGroupName="Approved" />
                                    <telerik:GridBoundColumn DataField="ApprovedBy" AllowSorting="true" AllowFiltering="true" HeaderText="By" ColumnGroupName="Approved" FilterControlWidth="80px" />

                                    <telerik:GridBoundColumn DataField="RecordType" AllowFiltering="true" AllowSorting="true" HeaderText="Record Type Code" FilterControlWidth="40px" />
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />
</asp:Content>
