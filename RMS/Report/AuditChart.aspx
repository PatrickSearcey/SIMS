<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="AuditChart.aspx.cs" Inherits="RMS.Report.AuditChart" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/recordprogress.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <br />
    <div class="mainContent">
        <div class="Filters">
            <p>
                This report shows the percent of records by record type that have been audited within the past 15 months. Hover your cursor over the bar in the charts to view more details.
            </p>
        </div>
        <telerik:RadGrid RenderMode="Lightweight" ID="rgRecordTypes" runat="server" GridLines="None" Skin="Bootstrap" OnNeedDataSource="rgRecordTypes_NeedDataSource"
            OnItemDataBound="rgRecordTypes_ItemDataBound" AllowPaging="False" PageSize="10" AllowSorting="True">
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="record_type_id">
                <Columns>
                    <telerik:GridBoundColumn DataField="RecordType" HeaderText="Record-Type" SortExpression="type_ds" UniqueName="RecordType" />
                    <telerik:GridTemplateColumn DataField="record_type_id" UniqueName="ProgressChartColumn" HeaderText="Current Record Progress">
                        <ItemTemplate>
                            <div style="width: 400px; height: 170px;">
                                <telerik:RadHtmlChart ID="rhcProgress" runat="server" Width="400" Height="170" Skin="Bootstrap">
                                    <Legend>
                                        <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                                    </Legend>
                                    <ChartTitle>
                                        <Appearance Visible="false"></Appearance>
                                    </ChartTitle>
                                    <PlotArea>
                                        <Series>
                                            <telerik:BarSeries Name="Audited" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentAudited">
                                                <Appearance>
                                                    <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                                </Appearance>
                                                <LabelsAppearance Visible="false"></LabelsAppearance>
                                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                    <ClientTemplate>
                                                        #=dataItem.PercentAuditedString# | #=dataItem.Audited# records audited of #=dataItem.TotalRecords# total records within the last 15 months.
                                                    </ClientTemplate>
                                                </TooltipsAppearance>
                                            </telerik:BarSeries>
                                        </Series>
                                        <Appearance>
                                            <FillStyle BackgroundColor="Transparent"></FillStyle>
                                        </Appearance>
                                        <XAxis AxisCrossingValue="0" Color="black" MajorTickType="Outside" MinorTickType="Outside"
                                            Reversed="false">
                                            <LabelsAppearance DataFormatString="{0}" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>
                                            <TitleAppearance Position="Center" RotationAngle="0" Text="Status"></TitleAppearance>
                                        </XAxis>
                                        <YAxis AxisCrossingValue="0" Color="black" MajorTickSize="1" MajorTickType="Outside" MaxValue="100" MinValue="0"
                                            MinorTickType="None" Reversed="false">
                                            <LabelsAppearance DataFormatString="{0}%" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>
                                            <TitleAppearance Position="Center" RotationAngle="0" Text="Progress Percentage"></TitleAppearance>
                                        </YAxis>
                                    </PlotArea>
                                </telerik:RadHtmlChart>
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
        </telerik:RadGrid>
    </div>
</asp:Content>
